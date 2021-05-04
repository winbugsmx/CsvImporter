using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CsvImporter.Entities;
using CsvImporter.Managers.CvsManager;
using CsvImporter.Managers.DatabaseManager;
using CsvImporter.Managers.FileManager;
using CsvImporter.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvImporter
{
    public class Program
    {
        public const string FileName = "Stock.CSV";

        /// <summary>
        /// Método principal
        /// Flujo:
        ///     1.- Se conecta al storage de Azure Files para descargar el archivo CSV a procesar
        ///     2.- Guarda el contenido del archivo en una BD Local, se cuenta con 2 enfoques
        ///         A) Se utiliza una librería para procesar el archivo, leerlo, almacenarlo en un objeto del tipo lista y por último se envía ese objeto a BD
        ///         B) Se utiliza el propio manejador de BD para que sea este quien leer, procese y guarde la información en BD.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var startup = new Startup();
            var spinner = new Spinner(1, 1);
            List<StockCsv> stockCsvs = new List<StockCsv>();

            //var summaryAzureStorageManager = BenchmarkRunner.Run<AzureStorageManager>();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Task<ProcessStatus> responseServiceAzureFile = ProcessAzureFile(startup, spinner);

            if (responseServiceAzureFile.Result == ProcessStatus.Success)
            {
                Console.WriteLine("¿Como desea cargar la información del archivo CSV?");
                Console.WriteLine("\t1 - Mediante librería TinyCsvParser");
                Console.WriteLine("\t2 - Mediante el motor de BD SQL");
                Console.Write("R= ");

                switch (Console.ReadLine())
                {
                    case "1":
                        {
                            //var summaryReadCsvManager = BenchmarkRunner.Run<ReadCsvManager>();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            ProcessStatus responseProcessCsvFile = ProcessCsvFile(startup, spinner, out stockCsvs);
                            if (responseProcessCsvFile == ProcessStatus.Success)
                            {
                                var responseByCsvRead = SaveDataByReadCsv(startup, spinner, stockCsvs);
                            }
                            break;
                        }
                    case "2":
                        {
                            //var summaryDatabaseManager = BenchmarkRunner.Run<DatabaseManager>();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            var responseByBulk = SaveDataByBulk(startup, spinner);
                            break;
                        }
                }
            }
            Console.ResetColor();
            Console.ReadKey();
        }

        /// <summary>
        /// Método para guardar el archivo CSV mediante una lectura de este
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <param name="stockCsvs"></param>
        /// <returns></returns>
        private static Task<ProcessStatus> SaveDataByReadCsv(Startup startup, Spinner spinner, List<StockCsv> stockCsvs)
        {
            var serviceDatabaseManager = startup.Provider.GetRequiredService<IDatabaseManager>();
            Console.WriteLine("Guardando datos en BD por CsvRead");
            spinner = new Spinner(1, 14);
            spinner.Start();
            var responseReadCsv = serviceDatabaseManager.SaveCsvByRead(stockCsvs);
            spinner.Stop();
            Console.SetCursorPosition(0, 15);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Información guardada \n");

            return responseReadCsv;
        }

        /// <summary>
        /// Método para guardar el archivo CVS dejando que el manejador de BD lo lea y guarde
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <returns></returns>
        private static Task<ProcessStatus> SaveDataByBulk(Startup startup, Spinner spinner)
        {
            var serviceDatabaseManager = startup.Provider.GetRequiredService<IDatabaseManager>();
            serviceDatabaseManager.FileName = FileName;
            Console.WriteLine("Guardando datos en BD por Bulk");
            spinner = new Spinner(1, 8);
            spinner.Start();
            var responseReadCsv = serviceDatabaseManager.SaveCsvByBulk();
            spinner.Stop();
            Console.SetCursorPosition(0, 9);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Información guardada \n");

            return responseReadCsv;
        }

        /// <summary>
        /// Método que lee y procesa el archivo CSV en un objeto del tipo Lista
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <param name="stockCsvs"></param>
        /// <returns></returns>
        private static ProcessStatus ProcessCsvFile(Startup startup, Spinner spinner, out List<StockCsv> stockCsvs)
        {
            var serviceReadCsv = startup.Provider.GetRequiredService<ICvsManager>();
            serviceReadCsv.FileName = FileName;
            Console.WriteLine("Procesando archivo CSV");
            spinner = new Spinner(1, 8);
            spinner.Start();
            var responseReadCsv = serviceReadCsv.ReadCsv();
            spinner.Stop();
            Console.SetCursorPosition(0, 9);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Archivo procesado \n");
            Console.WriteLine("Se encontraron " + responseReadCsv.Result.Count + " registros");
            stockCsvs = responseReadCsv.Result;
            return responseReadCsv.Result.Count > byte.MinValue ? ProcessStatus.Success : ProcessStatus.Fail;
        }

        /// <summary>
        /// Método que descarga el archivo CSC de una cuenta de Azure Files
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <returns></returns>
        private static Task<ProcessStatus> ProcessAzureFile(Startup startup, Spinner spinner)
        {
            var serviceAzureFile = startup.Provider.GetRequiredService<IStorageManager>();
            serviceAzureFile.FileName = FileName;
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Descargando archivo de datos CSV de Azure Files");
            spinner.Start();
            var responseServiceAzureFile = serviceAzureFile.ReadFrom();
            spinner.Stop();
            Console.SetCursorPosition(0, 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Archivo descargado \n");
            return responseServiceAzureFile;
        }
    }
}