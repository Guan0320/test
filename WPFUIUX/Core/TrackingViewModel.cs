using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WPFUIUX.Core
{
    /// <summary>
    /// 泛型變更追蹤 ViewModel 基類
    /// 可以包裝任何 Model 類型 T，並自動提供變更追蹤功能
    /// </summary>
    /// <typeparam name="T">要包裝的 Model 類型</typeparam>
    public class TrackingViewModel<T> : BindableBase where T : class
    {
        
        private Dictionary<string, object?>? _originalValues;
        private Dictionary<string, object?> _currentValues;

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            private set { SetProperty(ref _isDirty, value); }
        }
        private readonly T _model;
        /// <summary>
        /// 取得底層的 Model
        /// </summary>
        public T Model => _model;

        /// <summary>
        /// 索引器屬性，用於 XAML 綁定
        /// 例如：{Binding [Name]} 會呼叫 this["Name"]
        /// </summary>
        public object? this[string propertyName]
        {
            get => GetPropertyValue(propertyName);
            set => SetPropertyValue(propertyName, value);
        }

        public TrackingViewModel(T model)
        {
            if(model == null) throw new ArgumentNullException(nameof(model));
            _model = model;
            _currentValues = new Dictionary<string, object?>();

            // 初始化：從 Model 讀取所有屬性值
            InitializeFromModel(model);
        }

        /// <summary>
        /// 從 Model 初始化所有屬性值
        /// </summary>
        private void InitializeFromModel(T model)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(model);
                _currentValues[prop.Name] = value;
            }
        }
        private void InitializeFromModel()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(_model);
                _currentValues[prop.Name] = value;
            }
        }
        /// <summary>
        /// 取得屬性值（供索引器使用）
        /// </summary>
        private object? GetPropertyValue(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            if (_currentValues.TryGetValue(propertyName, out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 設定屬性值（供索引器使用）
        /// </summary>
        private void SetPropertyValue(string propertyName, object? value)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            // 取得舊值
            var oldValue = _currentValues.TryGetValue(propertyName, out var existing)
                ? existing
                : null;

            // 檢查是否真的有變更
            if (Equals(oldValue, value))
                return;

            // 更新當前值
            _currentValues[propertyName] = value;

            // 追蹤變更
            UpdateDirtyState(propertyName, oldValue, value);

            // 通知屬性變更
            RaisePropertyChanged($"Item[]");
        }
        private void UpdateDirtyState(string propertyName, object? oldValue, object? newValue)
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

        /// <summary>
        /// 將當前的 ViewModel 值同步回 Model
        /// </summary>
        //public void SyncToModel()
        //{
        //    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //        .Where(p => p.CanRead && p.CanWrite);

        //    foreach (var prop in properties)
        //    {
        //        if (_currentValues.TryGetValue(prop.Name, out var value))
        //        {
        //            prop.SetValue(_model, value);
        //        }
        //    }
        //}

        private DelegateCommand _reloadFromModelCommand ;
        public DelegateCommand ReloadFromModelCommand =>
            _reloadFromModelCommand  ?? (_reloadFromModelCommand  = new DelegateCommand(ReloadFromModel));
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
            RaisePropertyChanged(string.Empty);
            //RaisePropertyChanged("Item[]");
        }

        /// <summary>
        /// 取得屬性值（從內部字典）
        /// </summary>
        protected TValue? GetValue<TValue>([CallerMemberName] string? propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return default(TValue);

            if (_currentValues.TryGetValue(propertyName, out var value))
            {
                return value == null ? default(TValue) : (TValue)value;
            }

            return default(TValue);
        }

        /// <summary>
        /// 設定屬性值並追蹤變更
        /// </summary>
        protected bool SetValue<TValue>(TValue value, [CallerMemberName] string? propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;

            // 取得舊值
            var oldValue = _currentValues.TryGetValue(propertyName, out var existing)
                ? existing
                : default(TValue);

            // 檢查是否真的有變更
            if (EqualityComparer<TValue>.Default.Equals((TValue?)oldValue, value))
                return false;

            // 更新當前值
            _currentValues[propertyName] = value;

            // 追蹤變更
            UpdateDirtyState(propertyName, oldValue, value);

            // 通知屬性變更
            RaisePropertyChanged(propertyName);
            return true;
        }

    }
}
