using Godot;

public partial class Terminal : Control
{
	DockerManager docker;
	public override async void _Ready()
	{
		base._Ready();
		docker = new DockerManager("player_machine");
		await docker.StartAsync();
	}

	public override async void _ExitTree()
	{
		if (docker != null)
			await docker.StopAsync();

		base._ExitTree();
	}
}
