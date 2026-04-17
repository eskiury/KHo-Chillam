using Godot;
using System;

public partial class BotonPasarRonda : TextureButton
{
	[Export] Label ActualRoundLabel;

	public void UpdateText(int round)
	{
		ActualRoundLabel.Text = "Round " + round.ToString();
	}

}
