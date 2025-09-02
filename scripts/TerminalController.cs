using Godot;
using System.Threading;
using MinimalisticTelnet;
using System.Collections.Concurrent;
using System;

public partial class TerminalController : Node
{
    [Signal]
    public delegate void OutputReceivedWithArgumentEventHandler(string output);

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
        string output = "";
        GD.Print("Conexão iniciada");
        while (running)
        {
            while (commandQueue.TryDequeue(out string command))
            {
                telnet.WriteLine(command);
            }

            output = telnet.Read();
            if (output != "")
            {
                GD.Print(output);
                CallDeferred("emit_signal", SignalName.OutputReceivedWithArgument, output);
                output = "";
            }

            Thread.Sleep(50);
        }
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
