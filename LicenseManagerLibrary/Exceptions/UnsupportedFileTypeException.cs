using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseManagerLibrary.Exceptions
{
    public class UnsupportedFileTypeException :Exception
    {
        public UnsupportedFileTypeException(string message) : base(message){}
    }
}
