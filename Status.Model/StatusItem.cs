using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class StatusItem
    {
        private string _caption = null;
        public virtual int Id { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Milestone Milestone { get; set; }
        public virtual string Caption
        {
            get
            {
                if (_caption == null) return this.Topic.Caption;
                return _caption;
            }
            set
            {
                _caption = value;
            }
        }
        public virtual List<Note> Notes { get; set; }
    }
}
