using NanoswarmHive.Presentation.Services;
using NanoswarmHive.Presentation.ViewModel;

namespace NanoswarmHive.Presentation.Windows
{
    public sealed class DialogButtonInfo : AViewModelBase
    {
        private object _content;
        private bool _isCancel;
        private bool _isDefault;
        private DialogResultType _result;
        private string _key = string.Empty;

        public object Content { get => _content; set => SetValue(ref _content, value); }
        public bool IsCancel { get => _isCancel; set => SetValue(ref _isCancel, value); }
        public bool IsDefault { get => _isDefault; set => SetValue(ref _isDefault, value); }
        public DialogResultType Result { get => _result; set => SetValue(ref _result, value); }
        public string Key { get => _key; set => SetValue(ref _key, value); }

        public DialogButtonInfo()
        {
        }

        public DialogButtonInfo(object content, DialogResultType result, bool isDefault = false, bool isCancel = false)
        {
            _content = content;
            _result = result;
            _isDefault = isDefault;
            _isCancel = isCancel;
        }
    }
}
