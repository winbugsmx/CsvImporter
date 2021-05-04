using CsvImporter.Entities;
using CsvImporter.Mappers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser;

namespace CsvImporter.Managers.CvsManager
{
    public class ReadCsvManager : ICvsManager
    {
        private IConfiguration configuration;
        public string FileName { get; set; }

        public ReadCsvManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Médoto que lee el archivo CSV y lo baja a un objeto del tipo lista
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<List<StockCsv>> ReadCsv()
        {
            var response = new List<StockCsv>();
            try
            {
                //Creamos el objeto para el procesamiento del archivo CSV
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');

                //Obtenemos el directorio local donde se descargará el archivo .CSV
                string localDirectoryPath = configuration["LocalDirectoryPath"];

                //Creamos el path final donde se guardará el archivo .CSV descargado
                string localFilePath = string.Format("{0}\\{1}", localDirectoryPath, FileName);

                //Creamos el mapeo de la entidad donde bajaremos la información del CSV
                var csvParser = new CsvParser<StockCsv>(csvParserOptions, new StockMapping());

                //Leemos el archivo
                var records = csvParser.ReadFromFile(localFilePath, Encoding.UTF8).ToList();

                //Regresamos el listado de los registros leidos del archivo
                return records.Select(x => x.Result).ToList();
            }
            catch (Exception ex)
            {
            }
            return response;
        }
    }
}