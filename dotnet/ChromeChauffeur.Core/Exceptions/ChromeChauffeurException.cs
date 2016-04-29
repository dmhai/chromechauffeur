using System;

namespace ChromeChauffeur.Core.Exceptions
{
    public class ChromeChauffeurException : Exception
    {
        public ChromeChauffeurException(string message) : base(message)
        {
        }

        public ChromeChauffeurException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}