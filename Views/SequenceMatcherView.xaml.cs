using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace BioLabManager.Views;

public partial class SequenceMatcherView : UserControl
{
	private static readonly Regex _validChars = new("^[A-Za-z]+$");

	public SequenceMatcherView()
	{
		InitializeComponent();
	}

	private void InputSequence_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		e.Handled = !_validChars.IsMatch(e.Text);
		if (!e.Handled && sender is TextBox box)
		{
			int caret = box.CaretIndex;
			box.Text = box.Text.Insert(caret, e.Text.ToUpper());
			box.CaretIndex = caret + 1;
			e.Handled = true;
		}
	}
}
