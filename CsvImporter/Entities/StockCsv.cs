using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Entities
{
    public class StockCsv
    {
        public string PointOfSale { get; set; }
        public string Product { get; set; }
        public string Date { get; set; }
        public string Stock { get; set; }
    }
}