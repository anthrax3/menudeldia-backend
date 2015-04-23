using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MenuDelDia.Admin.Startup))]
namespace MenuDelDia.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
