using System;
using System.Threading;
using Godot;

public partial class InputText : TextEdit
{
	private RichTextLabel outputText;
	private string prompt;
	private Control terminal;
	private TerminalController tc;
	private int exit_levels = 0;

	private void Reset()
	{
		Text = "";
		InsertTextAtCaret(prompt);
	}

	private void AppendOutput(string output)
	{
		GD.Print("Saída do comando adicionada");
		outputText.AppendText(output);
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

				switch (command.Split(" ")[0])
				{
					case "":
						break;
					case "su" or "telnet" or "ssh":
						exit_levels++;
						ProcessCommand(command);
						break;
					case "exit":
						if (exit_levels == 0)
						{
							terminal._ExitTree();
						}
						else
						{
							exit_levels--;
							ProcessCommand(command);
						}
						break;
					case "clear":
						outputText.Text = "";
						break;
					default:
						ProcessCommand(command);
						break;

				}

				Reset();
			}
		}

	}



	public override void _Ready()
	{
		outputText = GetNode<RichTextLabel>($"../OutputText");
		terminal = GetNode<Control>($"..");

		tc = new TerminalController();
		AddChild(tc);
		tc.Connect(
		TerminalController.SignalName.OutputReceivedWithArgument, // nome do sinal
		new Callable(this, nameof(AppendOutput))                 // método callback
		);
	}
}
