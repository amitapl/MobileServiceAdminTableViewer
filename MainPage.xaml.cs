using Microsoft.WindowsAzure.MobileServices;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MobileServices
{    
    public sealed partial class MainPage : Page
    {
        private MobileServiceClient adminMobileService;

        public MainPage()
        {
            this.InitializeComponent();

            this.TableSelector.ItemsSource = Constants.TableNames;
        }

        private async Task RefreshTableView()
        {
            var mobileServiceTable = GetSelectedTable();
            if (mobileServiceTable != null)
            {
                BusyProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                // Get all items as json array
                var tableItems = await mobileServiceTable.ReadAsync("$top=10");

                var gridViewItems = tableItems.GetArray().Select(jsonValue => new MobileServiceTableItem(jsonValue.GetObject()));
                var gridViewItemsHeaders = tableItems.GetArray().First().GetObject().Select(o => o.Key);

                ListItems.ItemsSource = gridViewItems;
                ItemsHeader.ItemsSource = gridViewItemsHeaders;

                BusyProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private IMobileServiceTable GetSelectedTable()
        {
            var selectedTableName = TableSelector.SelectedValue as string;
            if (selectedTableName != null)
            {
                return this.adminMobileService.GetTable(selectedTableName);
            }
            else
            {
                return null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string masterKey = e.Parameter as string;
            adminMobileService = App.MobileService.WithFilter(new AdminServiceFilter(true, masterKey));
            this.TableSelector.SelectedIndex = 0;
        }

        private async Task ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await this.RefreshTableView();
        }

        private async void TableSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await this.RefreshTableView();
        }

        private async void RefreshClick(object sender, RoutedEventArgs e)
        {
            await this.RefreshTableView();
        }

        private async void DeleteClick(object sender, RoutedEventArgs e)
        {
            var selectedTable = GetSelectedTable();
            if (selectedTable != null)
            {
                var selectedTableItem = ListItems.SelectedItem as MobileServiceTableItem;
                if (selectedTableItem != null)
                {
                    BusyProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    await selectedTable.DeleteAsync(selectedTableItem.JsonObject);
                    await this.RefreshTableView();

                    BusyProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        private async void UpdateClick(object sender, RoutedEventArgs e)
        {
            var selectedTable = GetSelectedTable();
            if (selectedTable != null)
            {
                var selectedTableItem = ListItems.SelectedItem as MobileServiceTableItem;
                if (selectedTableItem != null)
                {
                    BusyProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    await selectedTable.UpdateAsync(selectedTableItem.JsonObject);
                    await this.RefreshTableView();

                    BusyProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }
    }
}
