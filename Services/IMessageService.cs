using System.Windows;

namespace BioLabManager.Services
{
    public interface IMessageService
    {
        void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information);
        bool Confirm(string message, string title = "Confirm");
    }
}
