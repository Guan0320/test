using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;

namespace BlankApp1.Models
{
    public class Customer : BindableBase
    {
        private Dictionary<string, object?>? _originalValues;
        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            private set { SetProperty(ref _isDirty, value); }
        }
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                var oldValue = _name;
                if(SetProperty(ref _name, value))
                {
                    UpdateDirtyState(nameof(Name),oldValue,value);
                }
            }
        }

        private void UpdateDirtyState(string propertyName, string oldValue, string newValue)
        {
            // Skip tracking for IsDirty itself to avoid infinite recursion
            if (propertyName == nameof(IsDirty))
                return;

            // 延遲初始化原始值字典
            if (_originalValues == null)
            {
                _originalValues = new Dictionary<string, object?>();
            }

            // 如果這個屬性還沒被追蹤過，記錄它的原始值
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues[propertyName] = oldValue;
            }

            // 檢查新值是否等於原始值
            object? originalValue = _originalValues[propertyName];

            if (Equals(originalValue, newValue))
            {
                // 改回原值了，移除追蹤
                _originalValues.Remove(propertyName);
            }

            bool currentDirtyState = _originalValues.Count > 0;

            if (!currentDirtyState)
            {
                _originalValues = null; // 釋放記憶體
            }

            // 更新 IsDirty 狀態
            IsDirty = currentDirtyState;
        }

        private int _age;
        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _address = string.Empty;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _city = string.Empty;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }


        private DelegateCommand _markAsCleanCommand;
        public DelegateCommand MarkAsCleanCommand =>
            _markAsCleanCommand ?? (_markAsCleanCommand = new DelegateCommand(MarkAsClean));

        /// <summary>
        /// 標記為乾淨狀態
        /// </summary>
        public void MarkAsClean()
        {
            _originalValues = null;
            IsDirty = false;
        }
        private DelegateCommand _reloadFromModelCommand;
        public DelegateCommand ReloadFromModelCommand =>
            _reloadFromModelCommand ?? (_reloadFromModelCommand = new DelegateCommand(ReloadFromModel));
        /// <summary>
        /// 從 Model 重新載入值（放棄所有變更）
        /// </summary>
        public void ReloadFromModel()
        {
            if (_originalValues == null) return;

            foreach (var key in _originalValues.Keys)
            {
                _currentValues[key] = _originalValues[key];
            }


            //_currentValues.Clear();
            //InitializeFromModel();
            MarkAsClean();

            // 通知所有屬性變更
            //RaisePropertyChanged(string.Empty);
            RaisePropertyChanged("string.Empty");
        }
    }
}
