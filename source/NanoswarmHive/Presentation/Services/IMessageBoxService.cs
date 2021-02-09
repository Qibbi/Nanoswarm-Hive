using System.Threading.Tasks;

namespace NanoswarmHive.Presentation.Services
{
    public enum MessageBoxResultType
    {
        None,
        Ok,
        Cancel,
        Yes,
        No
    }

    public enum MessageBoxButtonType
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxImageType
    {
        None,
        Error,
        Warning,
        Question,
        Information
    }

    public interface IMessageBoxService
    {
        Task<MessageBoxResultType> MessageBox(string message,
                                              string caption = null,
                                              MessageBoxButtonType buttons = MessageBoxButtonType.Ok,
                                              MessageBoxImageType image = MessageBoxImageType.None,
                                              string details = null);
    }
}
