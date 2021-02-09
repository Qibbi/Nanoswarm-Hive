using NanoswarmHive.Presentation.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NanoswarmHive.Presentation.Windows
{
    public static class DialogHelper
    {
        public static Task<MessageBoxResultType> MessageBox(IDispatcherService dispatcher,
                                                            string message,
                                                            string caption,
                                                            MessageBoxButtonType buttons = MessageBoxButtonType.Ok,
                                                            MessageBoxImageType image = MessageBoxImageType.None,
                                                            string details = null)
        {
            return dispatcher.InvokeTask(() => Windows.MessageBox.Show(message, caption, buttons, image, details));
        }

        public static Task<MessageBoxResultType> MessageBox(IDispatcherService dispatcher,
                                                            string message,
                                                            string caption,
                                                            IEnumerable<DialogButtonInfo> buttons,
                                                            MessageBoxImageType image = MessageBoxImageType.None,
                                                            string details = null)
        {
            return dispatcher.InvokeTask(() => Windows.MessageBox.Show(message, caption, buttons, image, details));
        }
    }
}
