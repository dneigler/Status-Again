using System.Collections.Generic;
using System.IO;
using Status.ETL.Csv;
using Status.Model;

namespace Status.Etl
{
    public interface IResourceAllocationEtl
    {
        IList<ResourceAllocationCsvItem> ImportAllocations(TextReader file);

        void ExportAllocations(TextWriter file, IList<ResourceAllocation> reports);

        void ExportAllocations(TextWriter file, IList<ResourceAllocationCsvItem> items);
    }
}