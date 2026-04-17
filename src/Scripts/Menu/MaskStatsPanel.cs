using Godot;
using System;

public partial class MaskStatsPanel : Control
{
	[Signal]
	public delegate void OnCloseButtonEventHandler();

	[Signal]
	public delegate void OnPickUpButtonEventHandler(StructureClass structureClass);

	[Export] private Label MachineLabel;
	[Export] private Label LevelLabel;
	[Export] private Label DamageLabel;
	[Export] private Label AttackCooldownLabel;
	[Export] private Label AttackRadiusLabel;
	[Export] public TextureButton CloseButton;
	[Export] public TextureButton PickUpButton;


	public StructureClass structure;
	public Marker3D CloseUpPosition;
	
	public void SetMaskStatPanel(StructureClass estructura, Marker3D finalPos)
	{
		structure = estructura;
		CloseUpPosition = finalPos;
		UpdateLabels();
	}

	private void UpdateLabels()
	{
		MachineLabel.Text = structure.MaskName;
		LevelLabel.Text = structure.Level.ToString();
		DamageLabel.Text = structure.AttackDamage.ToString();
		AttackCooldownLabel.Text = structure.AttackCooldown.ToString() + "s";
		AttackRadiusLabel.Text = structure.AttackRadius.ToString() + "m";
	}

	public void OnCloseButtonPressed()
	{
		EmitSignal(SignalName.OnCloseButton);
	}

	public void OnPickUpButtonPressed()
	{
		EmitSignal(SignalName.OnPickUpButton, structure);
	}
}
