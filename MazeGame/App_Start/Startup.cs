using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MazeGame.App_Start.Startup))]

namespace MazeGame.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Register signalR routing
            app.MapSignalR();
        }
    }
}