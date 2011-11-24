using System;

namespace Status.BLL
{
    /// <summary>
    /// Rolls status to the following Monday from passed in date.
    /// </summary>
    public class DefaultRollStatusDateProcessor : IRollStatusDateProcessor
    {
        public DateTime GetStatusReportDate(DateTime sourceDate)
        {
            DateTime retVal = sourceDate.AddDays(1);
            while (retVal.DayOfWeek != DayOfWeek.Monday)
                retVal = retVal.AddDays(1);
            return retVal;
        }
    }
}