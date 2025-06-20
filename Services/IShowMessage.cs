using System.Windows;
namespace BioLabManager.Services
{
    public interface IShowMessage
    {
        void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information);
    }
}
