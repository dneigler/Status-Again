using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.BLL
{
    public interface IRollStatusProcessor
    {
        //IRollStatusProcessor DayMethodology(RollStatusDayMethodology dayMethodology);
        //IRollStatusProcessor Span(TimeSpan rollSpan);
        StatusItem MapStatusItem(StatusItem sourceStatusItem, DateTime statusReportDate);
        //IRollStatusProcessor Create();
        DateTime GetPeriodStart(StatusReport sourceReport);
        DateTime GetPeriodEnd(StatusReport sourceReport);
    }
}
