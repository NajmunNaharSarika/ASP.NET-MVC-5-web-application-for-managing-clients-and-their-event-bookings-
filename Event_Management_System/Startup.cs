using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Event_Management_System.Startup))]
namespace Event_Management_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
