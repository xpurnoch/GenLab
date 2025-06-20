using CommunityToolkit.Mvvm.Messaging.Messages;

namespace BioLabManager.Messages
{
	public class SamplesImportedMessage : ValueChangedMessage<bool>
	{
		public SamplesImportedMessage(bool value) : base(value) { }
	}
}
