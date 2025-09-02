using System;
using System.Threading;
using Godot;

public partial class InputText : TextEdit
{
	private RichTextLabel outputText;
	private string prompt;
	private TerminalController tc;

	private void Reset()
	{
		Text = "";
		InsertTextAtCaret(prompt);
	}

	// public override void _Backspace(int caretIndex)
	// {
	//     int line = GetCaretLine();
	//     int column = GetCaretColumn();

	//     if (column > prompt.Length)
	//         RemoveText(line, column - 1, line, column);
	// }

	private void AppendOutput(string output)
	{
		outputText.AppendText(output);
		GD.Print("Retorno da conexão Telnet: " + output);
	}

	private void ProcessCommand(string command)
	{
		tc.SendCommand(command);
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Enter)
			{
				AcceptEvent();

				string command = GetLine(GetCaretLine())/*.Replace(prompt, "")*/.Trim();
				if (command != "")
					ProcessCommand(command);
				if (command == "clear")
					outputText.Text = "";

				Reset();
			}
			// if (eventKey.Pressed && eventKey.Keycode == Key.Backspace)
			// {
			//     _Backspace(0);
			// }
		}

	}



	public override void _Ready()
	{
		outputText = GetNode<RichTextLabel>($"../OutputText");

		tc = new TerminalController();
		tc.Connect(
		TerminalController.SignalName.OutputReceivedWithArgument, // nome do sinal
		new Callable(this, nameof(AppendOutput))                 // método callback
		);
	}
}
