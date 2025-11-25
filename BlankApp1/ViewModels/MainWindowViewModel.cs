using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Prism.Mvvm;
using BlankApp1.Models;

namespace BlankApp1.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        // 直接使用 TrackingViewModel<Customer>
        public ObservableCollection<CustomerViewModel> Customers { get; set; }

        public ICommand AddCustomerCommand { get; private set; }
        public ICommand SaveChangesCommand { get; private set; }

        public MainWindowViewModel()
        {
            Customers = new ObservableCollection<CustomerViewModel>();
            AddCustomerCommand = new DelegateCommand(AddCustomer);
            //SaveChangesCommand = new DelegateCommand(SaveChanges);

            // 載入範例資料
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // 創建範例資料
            var customers = new[]
            {
                new CustomerDto { Name = "張三", Age = 30, Email = "zhang@example.com", Address = "台北市信義區", City = "台北" },
                new CustomerDto{ Name = "李四", Age = 25, Email = "li@example.com", Address = "新北市板橋區", City = "新北" },
                new CustomerDto{ Name = "王五", Age = 35, Email = "wang@example.com", Address = "台中市西屯區", City = "台中" }
            };

            foreach (var customer in customers)
            {
                // 直接使用 TrackingViewModel<Customer>
                //var vm = new Customer();
                //vm.MarkAsClean(); // 標記為乾淨狀態
                Customers.Add(new CustomerViewModel(customer));
            }
        }
        private CustomerViewModel _selectedCustomer;
        public CustomerViewModel SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value); }
        }
        private void AddCustomer()
        {
            var newCustomer = new CustomerViewModel(new CustomerDto()
            { Name = "老六", Age = 35, Email = "wang@example.com", Address = "高雄市三民區", City = "高雄" }
                );

            Customers.Add(newCustomer);
        }
    }
}
