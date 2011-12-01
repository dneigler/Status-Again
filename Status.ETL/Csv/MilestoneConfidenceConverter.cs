using FileHelpers;
using Status.Model;

namespace Status.Etl.Csv
{
    public class MilestoneConfidenceConverter : ConverterBase
    {
        public override object StringToField(string @from)
        {
            return MilestoneConfidenceLevels.High;
        }

        public override string FieldToString(object from)
        {
            if (from != null)
                return from.ToString();
            else return "High";
        }
    }
}