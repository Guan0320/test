using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;

namespace BlankApp1.Models
{
    public class CustomerDto
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
    }
    public abstract class DirtyTrackingBase : BindableBase
    {
        private Dictionary<string, object?>? _originalValues;
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            private set => SetProperty(ref _isDirty, value);
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null!)
        {
            // 原本的值
            var oldValue = storage;

            // 先讓 Prism 處理變更通知
            bool changed = base.SetProperty(ref storage, value, propertyName);

            if (!changed)
                return false;

            // 再做 Dirty Tracking
            UpdateDirtyState(propertyName, oldValue, value);

            return true;
        }

        private void UpdateDirtyState(string propertyName, object? oldValue, object? newValue)
        {
            if (propertyName == nameof(IsDirty))
                return;

            _originalValues ??= new Dictionary<string, object?>();

            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues[propertyName] = oldValue;
            }

            object? originalValue = _originalValues[propertyName];

            if (Equals(originalValue, newValue))
            {
                _originalValues.Remove(propertyName);
            }

            if (_originalValues.Count == 0)
            {
                _originalValues = null;
                IsDirty = false;
            }
            else
            {
                IsDirty = true;
            }
        }

        private DelegateCommand _reloadFromModelCommand;
        public DelegateCommand ResetCommand =>
            _reloadFromModelCommand ?? (_reloadFromModelCommand = new DelegateCommand(Reset));

        /// <summary>
        /// 放棄所有變更
        /// </summary>
        public void Reset()
        {
            if (_originalValues == null) return;
            foreach (var key in _originalValues.Keys)
            {
                this.GetType().GetProperty(key).SetValue(this, _originalValues[key], null);
            }
            MarkAsClean();
            RaisePropertyChanged(string.Empty);
        }
        private void MarkAsClean()
        {
            _originalValues = null;
            IsDirty = false;
        }
    }

    public class CustomerViewModel : DirtyTrackingBase
    {
        public CustomerViewModel(CustomerDto dto)
        {
            // 將 DTO 值寫入 ViewModel
            _name = dto.Name;
            _age = dto.Age;
            _email = dto.Email;
            _address = dto.Address;
            _city = dto.City;
        }
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
            }
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
    }
}
