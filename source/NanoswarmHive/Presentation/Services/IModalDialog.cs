using System.Threading.Tasks;

namespace NanoswarmHive.Presentation.Services
{
    public interface IModalDialog
    {
        object DataContext { get; set; }

        Task<DialogResultType> ShowModal();

        void RequestClose(DialogResultType result);
    }
}
