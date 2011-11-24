using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.BLL
{
    public interface IRollStatusDateProcessor
    {
        DateTime GetStatusReportDate(DateTime sourceDate);
    }
}
