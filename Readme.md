# Prueba Analyticalways 

## Objetivo

Tienes que desarrollar un programa de consola .NET Core en C#, que lea un fichero .csv almacenado en una cuenta de almacenamiento de Azure e inserte su contenido en una BD SQL Server local.

La empresa que quiere el programa se llama Acme Corporation (un cl�sico) y tiene decidido que el nombre del programa sea CsvImporter. Adem�s, le gustar�a que el c�digo est� subido a un repositorio de github con el nombre PruebaCsvImporter_<Autor>.

El fichero .csv est� disponible en https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV

Antes de insertar en la BD, tendr�s que eliminar el contenido de una posible previa importaci�n.

Adem�s de un c�digo bien escrito, siguiendo las mejoras pr�cticas, nos importa el tiempo de proceso y el consumo de recursos (RAM, CPU, etc.) �tenlo en cuenta!

En la inserci�n puedes asumir que no es necesaria una transacci�n.

Si usas sabiamente el framework, te ayudar� con la configuraci�n, registro de dependencias, logging, etc.

Por supuesto, si acompa�as tu c�digo de testing, ser�s nuestro mejor amigo.

Podr�s usar las librer�as que creas oportuno.

Tambi�n nos gustar�a saber el porqu� de las decisiones que tomaste (y tambi�n de las que descartaste).

Si acompa�as el proyecto con un buen README.md, �nos har�as muy felices!

Por �ltimo, si consideras neces�rio agregar algo de testing automatizado para ganar m�s confianza, �nunca viene mal!

## Exposici�n

### Arquitectura

Aunque la soluci�n se encuentra estructurada dentro de un �nico proyecto, este esta dise�ado para respetar una arquitectura de 3 capas:

* Program.cs: Capa de presentaci�n, en este caso una aplicaci�n de consola.
* Managers/Files: Capa de reglas de negocio, en esta carpeta se encuentran los archivos para la conexi�n a Azure Files, la lectura del archivo CSV y el manejador hac�a BD.
* Dao: Capa de acceso a datos, en esta carpeta se encuentran los archivos necesarios para el acceso a la base de datos.   

Adem�s de la soluci�n principal, tambi�n se incluye un proyecto de las pruebas unitaras b�sicas para la lectura de datos del CSV.

### Configuraci�n appsettings.json

Para llevar a cabo las pruebas y ejecuci�n del proceso es necesario el actualizar las credenciales del Storage y Base de Datos, as� como definir los directorios donde reside el archivo en Azure Files y donde ser� descargado en el equipo local, esto es modificando los valores correspondientes dentro del archivo appsettings.json

```json
{
  "StorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=winbugsmxstorage;AccountKey=sNtvnPFeoePlU7PsWybqceg/Fkd2B36c7TTxiBRyRYnQqn2fiZGtWYFzzMTJHQP6R+e2y/CNq1r89AEGf9psCw==;EndpointSuffix=core.windows.net",
  "LocalDirectoryPath": "c:\\temp",
  "ShareClientName": "analyticalwaysdata",
  "ShareClientDirectory": "data",
  "SQLConnectionString": "Data Source=BMXDDT051987-LP;Initial Catalog=AnalyticAlways;User ID=sa;Password=Misato18;Connect Timeout=0;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True"
}
```

### Ejecuci�n de Scritp de Base de datos

Antes de ejecutar el programa es necesario el crear una Base de Datos de forma local a donde ser� almacenada la informaci�n del archivo CSV, para ello crearemos dentro de nuestro SQL Server una nueva base de datos llamada "AnalyticAlways", en caso de querer cambiar el nombre de esta es posible pero deber� de actualizar la cadena de conexi�n dentro del archivo appsettings.json para el acceso a la Base de datos.

```sql
USE [AnalyticAlways]
GO
/****** Object:  Table [dbo].[AnalyticAlwaysData]    Script Date: 04/05/2021 08:13:13 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnalyticAlwaysData](
	[PointOfSale] [nvarchar](50) NULL,
	[Product] [nvarchar](50) NULL,
	[Date] [nvarchar](50) NULL,
	[Stock] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[LoadCsv]    Script Date: 04/05/2021 08:13:13 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Gilberto Juarez
-- Create date: 03/05/2021
-- Description:	StoredProcedure para la carga de informaci�n de un archivo .CSV en una tabla
-- =============================================
CREATE PROCEDURE [dbo].[LoadCsv]
	@PathCsv NVARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	DECLARE @Query NVARCHAR(500);

	TRUNCATE TABLE AnalyticAlwaysData
	
	SET @Query = '
	BULK
	INSERT AnalyticAlwaysData
	FROM ''' + @PathCsv + '''
	WITH
	(
		FIELDTERMINATOR = '';'',
		ROWTERMINATOR = ''\n''
	)
	';
	
	EXECUTE sp_executesql @Query
END
GO

```

### Toma de Decisiones:

Esta soluci�n presenta 2 formas de abordar la carga de datos del archivo CSV, la primera es mediante el uso de librer�as de terceros, la segunda es mediante el propio motor de base de datos de SQL Server.

#### Uso de Librer�as

Al abordar este enfoque se analizar�n 2 librer�as para la lectura de archivo CSV:

* CSVHelper
* Tiny CSV Parser

Se investigo los pro y contra de cada una de estas librer�as, llegando a la conclusi�n que "Tiny CSV Parser" cuenta con un mejor desempe�o durante la carga de archivos pesados, tomando como referencia el siguiente �rticulo publicado en la web: https://dotnetcoretutorials.com/2018/08/04/csv-parsing-in-net-core/

#### Uso de Manejador de BD

No conforme con el desempe�o e implementaci�n de la primera soluci�n decid� abordar un segundo enfoque y aprovechar los beneficios que integra el propio manejador del SQL Server, por lo que decid� crear un nuevo proceso de carga, donde despu�s de descargar el archivo del Azure Files se manda a ejecutar un Stored Procedure con la ruta donde se encuentra el archivo CSV y aprovechar que sea el propio manejador quien lea y guarde la informaci�n del archivo en la tabla destino. 


### Conclusi�n

Despu�s de haber implementado ambas soluciones y enfoques debo de decir que la forma mas limpia y que mejor rendimiento tiene es aquella donde usamos los beneficios del propio manejador de Base de Datos, siendo menor el tiempo de procesamiento, as� como del uso de recursos del equipo. 