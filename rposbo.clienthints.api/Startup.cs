using Microsoft.Owin;
using Owin;
using rposbo.clienthints.api;

[assembly: OwinStartup(typeof(Startup))]

namespace rposbo.clienthints.api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
