using System.Collections.Generic;
using System.IO;
using Status.Etl.Csv;
using Status.Model;

namespace Status.Etl
{
    public interface IStatusEtl
    {
        IList<StatusCsvItem> ImportStatus(TextReader file);

        void ExportStatus(TextWriter file, IList<StatusReport> reports);

        void ExportStatus(TextWriter file, IList<StatusCsvItem> items);
    }
}