using CsvImporter.Entities;
using System.Data;
using System.Threading.Tasks;

namespace CsvImporter.Dao
{
    public interface ISqlDao
    {
        Task<ProcessStatus> SaveDataByCsvRead(DataTable data);

        Task<ProcessStatus> SaveDataByBulkCsv(string filePath);

        Task<int> GetTotalRows();
    }
}