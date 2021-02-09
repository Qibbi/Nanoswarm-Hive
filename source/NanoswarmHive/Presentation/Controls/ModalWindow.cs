using NanoswarmHive.Presentation.Services;
using NanoswarmHive.Presentation.Windows;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace NanoswarmHive.Presentation.Controls
{
    public abstract class AModalWindow : Window, IModalDialogInternal
    {
        public DialogResultType Result { get; set; } = Services.DialogResultType.None;

        protected override void OnClosed(EventArgs args)
        {
            base.OnClosed(args);
            if (Result == Services.DialogResultType.None)
            {
                Result = Services.DialogResultType.Cancel;
            }
        }

        public virtual async Task<DialogResultType> ShowModal()
        {
            Loaded += (sender, args) =>
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                if (handle != IntPtr.Zero)
                {
                    NativeHelper.DisableMinimizeButton(handle);
                }
            };
            Owner = WindowManager.MainWindow?.Window ?? WindowManager.BlockingWindows.LastOrDefault()?.Window;
            WindowStartupLocation = Owner is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
            await Dispatcher.InvokeAsync(ShowDialog);
            return Result;
        }

        public void RequestClose(DialogResultType result)
        {
            Result = result;
            Close();
        }
    }
}
