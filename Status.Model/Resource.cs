﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Resource : IIdentityColumn
    {
        private string _firstName = null,
            _lastName = null,
            _fullName = null;

        public virtual int Id { get; set; }
        public virtual string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }

        public virtual string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }

        public virtual string FullName
        {
            get
            {
                return String.Format("{0} {1}", this.FirstName, this.LastName);
            }
            set {
                if (!value.Contains(" "))
                    this.LastName = value;
                else
                {
                    this.FirstName = value.Substring(0, value.IndexOf(" "));
                    this.LastName = value.Substring(value.IndexOf(" ") + 1);
                }
            }
        }

        public virtual string EmailAddress { get; set; }
    }
}
