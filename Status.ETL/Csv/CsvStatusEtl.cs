using System.Collections.Generic;
using System.IO;
using FileHelpers;
using Status.Etl.Csv;
using Status.Model;

namespace Status.Etl.Csv
{
    public class CsvStatusEtl : IStatusEtl
    {
        public IList<StatusCsvItem> ImportStatus(TextReader file)
        {
            var engine = new FileHelperEngine<StatusCsvItem>();
            engine.Options.IgnoreFirstLines = 1;
            return engine.ReadStreamAsList(file, -1);
        }

        public void ExportStatus(TextWriter file, IList<StatusReport> reports)
        {
            var engine = new FileHelperEngine<StatusCsvItem>();
            var items = new List<StatusCsvItem>();
            // convert reports to items
            engine.WriteStream(file, items);
        }

        public void ExportStatus(TextWriter file, IList<StatusCsvItem> items)
        {
            var engine = new FileHelperEngine<StatusCsvItem>();
            engine.WriteStream(file, items);
        }

    }
}
