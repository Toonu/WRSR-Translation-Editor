using System.Text;
using System.Windows;

namespace WRLang.UI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            // This enables legacy code pages like 1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.OnStartup(e);
        }
    }
}
