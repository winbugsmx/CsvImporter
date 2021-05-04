using BenchmarkDotNet.Attributes;
using CsvImporter.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CsvImporter.Dao
{
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    [MemoryDiagnoser]
    public class SqlDao : ISqlDao
    {
        private IConfiguration configuration;
        private string connectionString;
        private SqlConnection sqlConnection;

        public SqlDao(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Método para guardar en base de datos el contenido de un archivo CSV previamente leido y procesado mediante la aplicación de consola
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ProcessStatus> SaveDataByCsvRead(DataTable data)
        {
            try
            {
                if (TruncateTable() == ProcessStatus.Success)
                {
                    return InsertCSVRecords(data);
                }
                return ProcessStatus.Fail;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                return ProcessStatus.Fail;
            }
        }

        /// <summary>
        /// Método para guardar en base de datos el contenido de un archivo CSV dejando que sea el propio motor de BD quien lo lea y procese
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<ProcessStatus> SaveDataByBulkCsv(string filePath)
        {
            try
            {
                return BulkData(filePath);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                return ProcessStatus.Fail;
            }
        }

        public async Task<int> GetTotalRows()
        {
            try
            {
                connection();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT COUNT(*) FROM AnalyticAlwaysData";
                sqlConnection.Open();
                var count = (int)sqlCommand.ExecuteScalar();
                sqlConnection.Close();
                return count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #region Private Methods

        /// <summary>
        /// Método para inicializar la conexión a Sql Server
        /// </summary>
        private void connection()
        {
            connectionString = configuration["SQLConnectionString"];
            sqlConnection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// Método para registar en BD el contenido del archivo CSV
        /// </summary>
        /// <param name="csvdt"></param>
        /// <returns></returns>
        [Benchmark]
        private ProcessStatus InsertCSVRecords(DataTable csvdt)
        {
            try
            {
                connection();
                SqlBulkCopy objbulk = new SqlBulkCopy(sqlConnection);
                objbulk.BulkCopyTimeout = 0;
                objbulk.DestinationTableName = "AnalyticAlwaysData";
                objbulk.ColumnMappings.Add("PointOfSale", "PointOfSale");
                objbulk.ColumnMappings.Add("Product", "Product");
                objbulk.ColumnMappings.Add("Date", "Date");
                objbulk.ColumnMappings.Add("Stock", "Stock");
                sqlConnection.Open();
                objbulk.WriteToServer(csvdt);
                sqlConnection.Close();

                return ProcessStatus.Success;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                return ProcessStatus.Fail;
            }
        }

        /// <summary>
        /// Método para limpiar el contenido de la tabla destino
        /// </summary>
        /// <returns></returns>
        private ProcessStatus TruncateTable()
        {
            try
            {
                connection();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "TRUNCATE TABLE AnalyticAlwaysData;";
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                return ProcessStatus.Success;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                return ProcessStatus.Fail;
            }
        }

        /// <summary>
        /// Método para cargar el archivo CSV mediante el manejador de BD mediante un SP
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [Benchmark]
        private ProcessStatus BulkData(string filePath)
        {
            try
            {
                connection();
                SqlCommand sqlCommand = new SqlCommand("LoadCsv", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 0;
                sqlCommand.Parameters.Add("@PathCsv", SqlDbType.VarChar).Value = filePath;
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                return ProcessStatus.Success;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                return ProcessStatus.Fail;
            }
        }

        #endregion Private Methods
    }
}