using CsvImporter.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Managers.DatabaseManager
{
    public interface IDatabaseManager
    {
        public string FileName { get; set; }

        Task<ProcessStatus> SaveCsvByRead(List<StockCsv> stockCsv);

        Task<ProcessStatus> SaveCsvByBulk();
    }
}