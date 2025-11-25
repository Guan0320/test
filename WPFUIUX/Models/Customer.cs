using Prism.Mvvm;

namespace WPFUIUX.Models
{
    /// <summary>
    /// Customer 業務邏輯 Model - 範例
    /// 純資料類，不包含變更追蹤邏輯
    /// </summary>
    public class Customer
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
    

}
