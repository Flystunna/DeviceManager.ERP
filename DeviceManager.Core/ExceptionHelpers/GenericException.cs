using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DeviceManager.Core.ExceptionHelpers
{
    [Serializable]
    public class GenericException : Exception
    {
        public string ErrorCode { get; set; }
        public int? StatusCode { get; set; }
        public GenericException(string message) : base(message)
        { }

        public GenericException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
        public GenericException(string message, int? errorCode) : base(message)
        {
            StatusCode = errorCode;
        }
    }
}
