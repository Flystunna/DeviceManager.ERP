using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceManager.Core.ExceptionHelpers
{
    [Serializable]
    public class ReadOnlyModeException
            : Exception
    {
        public ReadOnlyModeException()
        { }

        public ReadOnlyModeException(string message)
            : base(message)
        {
        }

        public ReadOnlyModeException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
