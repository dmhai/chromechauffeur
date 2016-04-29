using System.IO;
using System.Reflection;

namespace ChromeChauffeur.Core.Infrastructure.IO
{
    public class EmbeddedResourceReader
    {
        public string Read(string resourceName, Assembly assembly)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        } 
    }
}