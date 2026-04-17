using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] public Button PlayButton;
	[Export] public Button QuitButton;

	public override void _Ready()
	{

		if (PlayButton != null)
		{
			PlayButton.Pressed += OnPlayPressed;
		}
		if (QuitButton != null)
		{
			QuitButton.Pressed += OnQuitPressed;
		}
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Game/Base/game_scene.tscn");
	}
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
