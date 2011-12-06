using System;
using System.Collections.Generic;

namespace StatusMvc.Models
{
    public class StatusReportViewModel
    {
        //private IList<StatusItem> _items = null;
        public int Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public string Caption { get; set; }
        public int NumberOfStatusItems { get; set; }
    }
}