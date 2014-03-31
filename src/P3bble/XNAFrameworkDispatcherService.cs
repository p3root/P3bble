using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace P3bble
{
    public class XNAFrameworkDispatcherService : IApplicationService
    {
        private DispatcherTimer frameworkDispatcherTimer;

        public XNAFrameworkDispatcherService()
        {
            this.frameworkDispatcherTimer = new DispatcherTimer();
            this.frameworkDispatcherTimer.Interval = TimeSpan.FromTicks(333333);
            this.frameworkDispatcherTimer.Tick += frameworkDispatcherTimer_Tick;
            FrameworkDispatcher.Update();
        }

        void frameworkDispatcherTimer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }

        void IApplicationService.StartService(ApplicationServiceContext context)
        {
            this.frameworkDispatcherTimer.Start();
        }

        void IApplicationService.StopService()
        {
            this.frameworkDispatcherTimer.Stop();
        }
    }
}
