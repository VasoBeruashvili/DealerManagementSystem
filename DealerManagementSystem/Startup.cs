using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DealerManagementSystem.Startup))]
namespace DealerManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
