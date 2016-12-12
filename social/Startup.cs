using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(social.Startup))]
namespace social
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
