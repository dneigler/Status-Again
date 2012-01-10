using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.ETL.Csv
{
    public interface ICsvResourceAllocationBridge
    {
        void UpsertResourceAllocations(IList<ResourceAllocationCsvItem> items);
    }
}
