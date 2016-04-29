using System;

namespace ChromeChauffeur.Core.Exceptions
{
    public class ChromeChauffeurTimeoutException : Exception
    {
        public ChromeChauffeurTimeoutException(string message) : base(message)
        {
        }

        public ChromeChauffeurTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}