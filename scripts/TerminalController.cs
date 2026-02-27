using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using MinimalisticTelnet;

public partial class TerminalController : Node
{
	[Signal]
	public delegate void OutputReceivedWithArgumentEventHandler(string output);

	private TelnetConnection telnet;
	private CancellationTokenSource cts;
	private Task telnetTask;
	private ConcurrentQueue<string> commandQueue = new();

	public override async void _Ready()
	{
		await ConnectWithRetry();
	}

	private async Task ConnectWithRetry()
	{
		GD.Print("Tentando conectar...");

		int maxAttempts = 10;
		int attempt = 0;

		while (attempt < maxAttempts)
		{
			try
			{
				attempt++;
				GD.Print($"Tentativa {attempt}");

				telnet = new TelnetConnection("127.0.0.1", 5000);

				telnet.Login("player", "player", 2000);

				GD.Print("Login deu certo :)");

				cts = new CancellationTokenSource();
				telnetTask = RunTelnetAsync(cts.Token);

				return;
			}
			catch (Exception e)
			{
				GD.PrintErr($"Falhou tentativa {attempt}: {e.Message}");
				await Task.Delay(1000);
			}
		}

		GD.PrintErr("Não conseguiu conectar após várias tentativas.");
	}

	private async Task RunTelnetAsync(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				while (commandQueue.TryDequeue(out string cmd))
				{
					GD.Print("Comando enviado");
					telnet.WriteLine(cmd);
				}

				string output = telnet.Read();

				if (!string.IsNullOrEmpty(output))
				{
					CallDeferred(MethodName.EmitSignal,
						SignalName.OutputReceivedWithArgument,
						output);
				}

				await Task.Delay(10, token);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception e)
			{
				GD.PrintErr($"Erro telnet: {e.Message}");
			}
		}
	}

	public void SendCommand(string command)
	{
		GD.Print("Comando enfileirado");
		commandQueue.Enqueue(command);
	}

	public override async void _ExitTree()
	{
		if (cts != null)
		{
			cts.Cancel();

			try
			{
				await telnetTask;
			}
			catch { }

			cts.Dispose();
		}
	}
}