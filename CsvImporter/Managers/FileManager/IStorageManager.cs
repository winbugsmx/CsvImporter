using CsvImporter.Entities;
using System.Threading.Tasks;

namespace CsvImporter.Managers.FileManager
{
    public interface IStorageManager
    {
        public string FileName { get; set; }

        Task<ProcessStatus> ReadFrom();
    }
}