﻿using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Exceptions
{
    public class ImportParseException : Exception
    {
        public string Error { get; set; }

        public ImportParseException(string message) : base(message) 
        {
            Error = message;
        }
    }
}
