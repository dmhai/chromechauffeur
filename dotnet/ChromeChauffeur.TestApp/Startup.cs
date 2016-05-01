using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChromeChauffeur.TestApp.Startup))]
namespace ChromeChauffeur.TestApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
