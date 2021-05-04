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
    public class ProgramTest
    {
        public const string FileName = "Stock.CSV";

        /// <summary>
        /// Método para guardar el archivo CSV mediante una lectura de este
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <param name="stockCsvs"></param>
        /// <returns></returns>
        public static Task<ProcessStatus> SaveDataByReadCsv(Startup startup, List<StockCsv> stockCsvs)
        {
            var serviceDatabaseManager = startup.Provider.GetRequiredService<IDatabaseManager>();
            var responseReadCsv = serviceDatabaseManager.SaveCsvByRead(stockCsvs);

            return responseReadCsv;
        }

        /// <summary>
        /// Método para guardar el archivo CVS dejando que el manejador de BD lo lea y guarde
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <returns></returns>
        public static Task<ProcessStatus> SaveDataByBulk(Startup startup)
        {
            var serviceDatabaseManager = startup.Provider.GetRequiredService<IDatabaseManager>();
            serviceDatabaseManager.FileName = FileName;
            var responseReadCsv = serviceDatabaseManager.SaveCsvByBulk();

            return responseReadCsv;
        }

        /// <summary>
        /// Método que lee y procesa el archivo CSV en un objeto del tipo Lista
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <param name="stockCsvs"></param>
        /// <returns></returns>
        public static ProcessStatus ProcessCsvFile(Startup startup, out List<StockCsv> stockCsvs)
        {
            var serviceReadCsv = startup.Provider.GetRequiredService<ICvsManager>();
            serviceReadCsv.FileName = FileName;
            var responseReadCsv = serviceReadCsv.ReadCsv();
            stockCsvs = responseReadCsv.Result;

            return responseReadCsv.Result.Count > byte.MinValue ? ProcessStatus.Success : ProcessStatus.Fail;
        }

        /// <summary>
        /// Método que descarga el archivo CSC de una cuenta de Azure Files
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="spinner"></param>
        /// <returns></returns>
        public static Task<ProcessStatus> ProcessAzureFile(Startup startup)
        {
            var serviceAzureFile = startup.Provider.GetRequiredService<IStorageManager>();
            serviceAzureFile.FileName = FileName;
            var responseServiceAzureFile = serviceAzureFile.ReadFrom();

            return responseServiceAzureFile;
        }
    }
}