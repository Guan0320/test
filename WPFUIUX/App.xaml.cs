using System.Configuration;
using System.Data;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using WPFUIUX.Views;

namespace WPFUIUX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //
        }
    }
}
