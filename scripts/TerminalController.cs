using Godot;
using System.Threading;
using MinimalisticTelnet;
using System.Collections.Concurrent;
using System;

public partial class TerminalController : Node
{
    [Signal]
    public delegate string OutputReceivedEventHandler(string output);

    private TelnetConnection telnet;
    private Thread telnetThread;
    private ConcurrentQueue<string> commandQueue = new ConcurrentQueue<string>();
    private bool running = true;

    public TerminalController()
    {
        telnet = new TelnetConnection("127.0.0.1", 5001);
        telnetThread = new Thread(TelnetLoop);
        telnetThread.Start();
    }

    private void TelnetLoop()
    {
        GD.Print("Conexão Telnet estabelecida");
        while (running)
        {
            while (commandQueue.TryDequeue(out string command))
            {
                telnet.WriteLine(command);
                string output = telnet.Read();
                CallDeferred(nameof(EmitOutput), output);
            }

            Thread.Sleep(50);
        }
    }

    private void EmitOutput(string output)
    {
        EmitSignal(nameof(OutputReceived), output);
    }

    public void SendCommand(string command)
    {
        commandQueue.Enqueue(command);
    }

    public override void _ExitTree()
    {
        running = false;
        telnetThread.Join();
    }

}