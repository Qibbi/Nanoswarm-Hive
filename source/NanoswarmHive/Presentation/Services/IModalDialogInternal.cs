namespace NanoswarmHive.Presentation.Services
{
    public interface IModalDialogInternal : IModalDialog
    {
        DialogResultType Result { get; set; }
    }
}
