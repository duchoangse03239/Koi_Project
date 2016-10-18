using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KoiManagement.Startup))]
namespace KoiManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
