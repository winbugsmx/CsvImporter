using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvImporter.Entities;
using Microsoft.Extensions.Configuration;
using CsvImporter.Utils;
using CsvImporter.Dao;

namespace CsvImporter.Managers.DatabaseManager
{
    public class DatabaseManager : IDatabaseManager
    {
        private ISqlDao sqlDao;
        private IConfiguration configuration;

        public string FileName { get; set; }

        public DatabaseManager(ISqlDao _sqlDao, IConfiguration _configuration)
        {
            this.sqlDao = _sqlDao;
            this.configuration = _configuration;
        }

        /// <summary>
        /// Método que guarda en BD la lectura del archivo CSV almacenado en un objeto del tipo lista
        /// </summary>
        /// <param name="stockCsv"></param>
        /// <returns></returns>
        public async Task<ProcessStatus> SaveCsvByRead(List<StockCsv> stockCsv)
        {
            try
            {
                var dataTable = stockCsv.ToDataTable();
                return await sqlDao.SaveDataByCsvRead(dataTable);
            }
            catch (Exception ex)
            {
                return ProcessStatus.Fail;
            }
        }

        /// <summary>
        /// Método que guarda en BD el archivo CSV dejando que sea el propio manejador quien lo lea y procese
        /// </summary>
        /// <returns></returns>
        public async Task<ProcessStatus> SaveCsvByBulk()
        {
            try
            {
                //Obtenemos el directorio local donde se descargará el archivo .CSV
                string localDirectoryPath = configuration["LocalDirectoryPath"];

                //Creamos el path final donde se guardará el archivo .CSV descargado
                string localFilePath = string.Format("{0}\\{1}", localDirectoryPath, FileName);

                return await sqlDao.SaveDataByBulkCsv(localFilePath);
            }
            catch (Exception ex)
            {
                return ProcessStatus.Fail;
            }
        }
    }
}