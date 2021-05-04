using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using CsvImporter.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Managers.FileManager
{
    public class AzureStorageManager : IStorageManager
    {
        private IConfiguration configuration;
        public string FileName { get; set; }

        public AzureStorageManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Mérodo que descarga el archivo CSV desde una cuenta de storage de Azure Files
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<ProcessStatus> ReadFrom()
        {
            try
            {
                //Obtenemos el directorio local donde se descargará el archivo .CSV
                string localDirectoryPath = configuration["LocalDirectoryPath"];

                //Creamos el path final donde se guardará el archivo .CSV descargado
                string localFilePath = string.Format("{0}\\{1}", localDirectoryPath, FileName);

                //Obtenemos la cadena de conexión del storage en azure
                string connectionString = configuration["StorageConnectionString"];

                //Creamos un share client a ser usado para manipular el archivo .CSV
                ShareClient share = new ShareClient(connectionString, configuration["ShareClientName"]);

                //Especificamos el directorio donde esta almacenado el archivo en azure
                ShareDirectoryClient shareDirectoryClient = share.GetDirectoryClient(configuration["ShareClientDirectory"]);

                //Generamos la solicitud de acceso al archivo CSV dentro de azure
                ShareFileClient shareFileClient = shareDirectoryClient.GetFileClient(FileName);

                //Cremos el stream de descarga del archivo
                ShareFileDownloadInfo shareFileDownloadInfo = shareFileClient.Download();

                //Generamos un stream para el guardado del archivo a descargar, se reemplazará si ya existe una copia previa
                using (FileStream stream = File.OpenWrite(localFilePath))
                {
                    //Copiamos el stream de descarga al archivo local
                    shareFileDownloadInfo.Content.CopyTo(stream);
                    //Liberamos el recurso del stream
                    await stream.FlushAsync();
                    stream.Close();
                    return ProcessStatus.Success;
                }
            }
            catch (Exception ex)
            {
                return ProcessStatus.Fail;
            }
        }
    }
}