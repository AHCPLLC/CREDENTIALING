﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHC.CD.Exceptions.Profiles
{
    public class DuplicateFederalDEALicenseNumberException : ApplicationException
    {
        public DuplicateFederalDEALicenseNumberException(string message = "", Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
