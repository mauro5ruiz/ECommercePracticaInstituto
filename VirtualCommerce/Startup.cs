using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VirtualCommerce.Startup))]
namespace VirtualCommerce
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
