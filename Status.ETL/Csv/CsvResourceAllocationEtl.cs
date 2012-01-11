using System.Collections.Generic;
using System.IO;
using FileHelpers;
using Status.Etl;
using Status.Model;

namespace Status.ETL.Csv
{
    public class CsvResourceAllocationEtl : IResourceAllocationEtl
    {
        public IList<ResourceAllocationCsvItem> ImportAllocations(TextReader file)
        {
            var engine = new FileHelperEngine<ResourceAllocationCsvItem>();
            engine.Options.IgnoreFirstLines = 1;
            return engine.ReadStreamAsList(file, -1);
        }   

        public void ExportAllocations(TextWriter file, IList<ResourceAllocation> reports)
        {
            var engine = new FileHelperEngine<ResourceAllocationCsvItem>();
            var items = new List<ResourceAllocationCsvItem>();
            // convert reports to items
            engine.WriteStream(file, items);
        }

        public void ExportAllocations(TextWriter file, IList<ResourceAllocationCsvItem> items)
        {
            var engine = new FileHelperEngine<ResourceAllocationCsvItem>();
            engine.WriteStream(file, items);
        }
    }
}