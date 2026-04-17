using Godot;
using System;

public partial class LifePanel : Godot.TextureRect
{
	[Export] Label LivesLabel = null;

    public override void _Ready()
    {
        base._Ready();
		GameManager.Instance.OnLiveChange +=  changeLabel;
		LivesLabel.Text = GameManager.Instance.LivesLeft.ToString();
    }

	private void changeLabel(int money)
	{
		LivesLabel.Text = money.ToString();
	}

}
