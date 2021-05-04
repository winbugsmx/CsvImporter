using CsvImporter.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using TinyCsvParser.Mapping;

namespace CsvImporter.Mappers
{
    public class StockMapping : CsvMapping<StockCsv>
    {
        public StockMapping() : base()
        {
            MapProperty(0, x => x.PointOfSale);
            MapProperty(1, x => x.Product);
            MapProperty(2, x => x.Date);
            MapProperty(3, x => x.Stock);
        }
    }
}