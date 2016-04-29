using System;

namespace ChromeChauffeur.Core.Extensions
{
    internal static class TimeSpanExtensions
    {
         public static DateTime AsDeadline(this TimeSpan timeSpan)
         {
             return DateTime.Now.Add(timeSpan);
         }
    }
}