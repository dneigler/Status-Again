using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using Status.Model;

namespace Status.ETL.Csv
{
    public class ProjectTypeConverter : ConverterBase
    {
        public override string FieldToString(object from)
        {
            var mt = (ProjectType)from;

            switch (mt)
            {
                case ProjectType.Run:
                    return "Run";
                case ProjectType.Grow:
                    return "Grow";
            }
            return "Run";
        }

        public override object StringToField(string @from)
        {
            switch (@from)
            {
                case "Run":
                    return ProjectType.Run;
                case "Grow":
                    return ProjectType.Grow;
            }
            return ProjectType.Run;
        }
    }
}
