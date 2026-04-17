using Godot;
using System;

public partial class Options : Control
{
	[Export] public HSlider VolumeSlider;
	[Export] public Button BackButton;

	private string _mainMenuPath = "res://Scenes/Menu/MainMenu_scene.tscn";

	public override void _Ready()
	{
		if (BackButton != null)
			BackButton.Pressed += OnBackButtonPressed;

		if (VolumeSlider != null)
		{
			VolumeSlider.ValueChanged += OnVolumeChanged;
		}
	}

	private void OnBackButtonPressed()
	{
		GetTree().ChangeSceneToFile(_mainMenuPath);
	}

	private void OnVolumeChanged(double value)
	{
		//GD.Print("volumennnnn");
	}




}
