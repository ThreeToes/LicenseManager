using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseManagerLibrary.Exceptions
{
    public class InvalidLicenseException : Exception
    {
        public InvalidLicenseException(string message):base(message)
        {
        }
    }
}
