using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.BLL
{
    public class DefaultRollStatusProcessor : IRollStatusProcessor
    {
        public Model.StatusItem MapStatusItem(Model.StatusItem sourceStatusItem)
        {
            StatusItem si = new StatusItem();
            si.Caption = sourceStatusItem.Caption;
            // this is a good place for windows workflow to simplify management of transfering milestone types
            if (si.Milestone.Date<DateTime.Today.AddDays(-7))
                        return null;
            //if (si.Milestone.Date < DateTime.Today
            return si;      
        }

        public DateTime GetPeriodStart(Model.StatusReport sourceReport)
        {
            throw new NotImplementedException();
        }

        public DateTime GetPeriodEnd(Model.StatusReport sourceReport)
        {
            throw new NotImplementedException();
        }
    }
}
