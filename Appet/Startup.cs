using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Appet.Startup))]
namespace Appet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
