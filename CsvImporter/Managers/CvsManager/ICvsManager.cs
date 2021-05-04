using CsvImporter.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvImporter.Managers.CvsManager
{
    public interface ICvsManager
    {
        public string FileName { get; set; }

        Task<List<StockCsv>> ReadCsv();
    }
}