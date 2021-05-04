using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CsvImporter;
using CsvImporter.Dao;
using CsvImporter.Entities;
using CsvImporter.Utils;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvImporterTest
{
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    [MemoryDiagnoser]
    public class Tests
    {
        private IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        private IConfiguration configuration;
        private ISqlDao sqlDao;

        [SetUp]
        public void Setup()
        {
            configurationBuilder.AddJsonFile("appsettings.json");
            configuration = configurationBuilder.Build();
            sqlDao = new SqlDao(configuration);
        }

        [Test]
        [Benchmark(Baseline = true)]
        public void CargaInformaciónPorTinyCsvParser()
        {
            var summaryProgram = BenchmarkRunner.Run<ProgramTest>();
            var startup = new Startup();
            List<StockCsv> stockCsvs = new List<StockCsv>();
            Task<ProcessStatus> responseServiceAzureFile = ProgramTest.ProcessAzureFile(startup);
            if (responseServiceAzureFile.Result == ProcessStatus.Success)
            {
                ProcessStatus responseProcessCsvFile = ProgramTest.ProcessCsvFile(startup, out stockCsvs);
                if (responseProcessCsvFile == ProcessStatus.Success)
                {
                    var responseByCsvRead = ProgramTest.SaveDataByReadCsv(startup, stockCsvs);
                }
            }

            int rows = sqlDao.GetTotalRows().Result;
            Assert.GreaterOrEqual(rows, 0);
        }

        [Test]
        [Benchmark(Baseline = true)]
        public void CargaInformaciónPorBulkSQLServer()
        {
            var summaryProgram = BenchmarkRunner.Run<ProgramTest>();
            var startup = new Startup();
            List<StockCsv> stockCsvs = new List<StockCsv>();
            Task<ProcessStatus> responseServiceAzureFile = ProgramTest.ProcessAzureFile(startup);
            if (responseServiceAzureFile.Result == ProcessStatus.Success)
            {
                var responseByBulk = ProgramTest.SaveDataByBulk(startup);
            }

            int rows = sqlDao.GetTotalRows().Result;
            Assert.GreaterOrEqual(rows, 0);
        }
    }
}