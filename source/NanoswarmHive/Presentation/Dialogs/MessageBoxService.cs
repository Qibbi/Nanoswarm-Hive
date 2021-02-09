using NanoswarmHive.Presentation.Services;
using NanoswarmHive.Presentation.Windows;
using System.Threading.Tasks;

namespace NanoswarmHive.Presentation.Dialogs
{
    public class MessageBoxService : IMessageBoxService
    {
        protected IDispatcherService Dispatcher { get; }

        public string ApplicationName { get; }

        public MessageBoxService(IDispatcherService dispatcher, string applicationName)
        {
            Dispatcher = dispatcher;
            ApplicationName = applicationName;
        }

        public Task<MessageBoxResultType> MessageBox(string message, string caption = null, MessageBoxButtonType buttons = MessageBoxButtonType.Ok, MessageBoxImageType image = MessageBoxImageType.None, string details = null)
        {
            return DialogHelper.MessageBox(Dispatcher, message, caption ?? ApplicationName, buttons, image, details);
        }
    }
}
