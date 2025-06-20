using CommunityToolkit.Mvvm.Messaging;

public class DatabaseService
{
	private readonly IMessenger _messenger;

	public DatabaseService(IMessenger messenger)
	{
		_messenger = messenger;
	}
}