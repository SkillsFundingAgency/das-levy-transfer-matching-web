using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Exceptions
{
    public class NullModelException : Exception
    {
        private const string DefaultMessageFormat = "The {0} model could not be found";

        public NullModelException()
        {
            
        }

        public NullModelException(string modelName) : base(string.Format(DefaultMessageFormat, modelName))
        {
            
        }

        public NullModelException(string modelName, Exception innerException) : base(string.Format(DefaultMessageFormat, modelName), innerException)
        {
            
        }
    }
}
