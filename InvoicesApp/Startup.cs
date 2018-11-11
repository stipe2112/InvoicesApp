using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InvoicesApp.Startup))]
namespace InvoicesApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
