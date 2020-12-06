using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Find_Your_Petrol1.Startup))]
namespace Find_Your_Petrol1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
