using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MobileServices
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MasterKeyInputPage : Page
    {
        public MasterKeyInputPage()
        {
            this.InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), MasterKeyTextBox.Text);
        }
    }
}
