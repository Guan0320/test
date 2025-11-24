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
        public ObservableCollection<Customer> Customers { get; set; }

        public ICommand AddCustomerCommand { get; private set; }
        public ICommand SaveChangesCommand { get; private set; }

        public MainWindowViewModel()
        {
            Customers = new ObservableCollection<Customer>();
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
                new Customer { Name = "張三", Age = 30, Email = "zhang@example.com", Address = "台北市信義區", City = "台北" },
                new Customer { Name = "李四", Age = 25, Email = "li@example.com", Address = "新北市板橋區", City = "新北" },
                new Customer { Name = "王五", Age = 35, Email = "wang@example.com", Address = "台中市西屯區", City = "台中" }
            };

            foreach (var customer in customers)
            {
                // 直接使用 TrackingViewModel<Customer>
                //var vm = new Customer();
                //vm.MarkAsClean(); // 標記為乾淨狀態
                Customers.Add(customer);
            }
        }
        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value); }
        }
        private void AddCustomer()
        {
            var newCustomer = new Customer
            {
                Name = "新客戶",
                Age = 20,
                Email = "new@example.com",
                Address = "地址",
                City = "城市"
            };

            Customers.Add(newCustomer);
        }

        //private void SaveChanges()
        //{
        //    // 找出所有被修改的項目
        //    var modifiedItems = Customers.Where(c => c.IsDirty).ToList();

        //    if (modifiedItems.Count == 0)
        //    {
        //        MessageBox.Show("沒有資料需要儲存。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }

        //    // 這裡可以呼叫 API 或資料庫儲存
        //    // 透過 Model 屬性存取底層資料
        //    string message = $"找到 {modifiedItems.Count} 筆修改的資料：\n\n";
        //    foreach (var item in modifiedItems.Take(5))
        //    {
        //        var customer = item.Model;
        //        message += $"- {customer.Name} (Age: {customer.Age})\n";
        //    }
        //    if (modifiedItems.Count > 5)
        //        message += "...";

        //    MessageBox.Show(message, "儲存變更", MessageBoxButton.OK, MessageBoxImage.Information);

        //    // 儲存成功後，標記為乾淨狀態
        //    foreach (var item in modifiedItems)
        //    {
        //        item.MarkAsClean();
        //    }
        //}
    }
}
