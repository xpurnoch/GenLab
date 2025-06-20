using System.Windows;

namespace BioLabManager.Services
{
    public class MessageService : IMessageService
    {
        public void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        public bool Confirm(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }

}
