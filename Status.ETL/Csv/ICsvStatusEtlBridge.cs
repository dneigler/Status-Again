using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Etl.Csv;

namespace Status.ETL.Csv
{
    public interface ICsvStatusEtlBridge
    {
        void UpsertStatus(IList<StatusCsvItem> items);
    }
}
