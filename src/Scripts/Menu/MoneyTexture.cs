using Godot;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

public partial class MoneyTexture : Godot.TextureRect
{
	[Export] Label MoneyLabel = null;
	[Export] Label DifferenceLabel = null;
	private int actualMoney;
	private int modMoney;
	private bool IsChangingMoney = false;

    public override void _Ready()
    {
        base._Ready();
		GameManager.Instance.OnMoneyChange +=  OnMoneyChanged;
		actualMoney = GameManager.Instance.getMoney();
		modMoney = actualMoney;
		updateLabel();
    }

	private void updateLabel()
	{
		MoneyLabel.Text = actualMoney.ToString();
	}

	private void OnMoneyChanged(int money)
	{
		//GD.Print("Dinero Cambiado");
		IsChangingMoney = true;
		int previousMoney = modMoney;
		modMoney = money;

		int direference = modMoney - previousMoney;
		if (direference > 0)
		{
			//GD.Print("Dinero positivo");
			DifferenceLabel.Modulate = Colors.Green;
			DifferenceLabel.Text = "+" + direference.ToString();
		}
		else if (direference < 0)
		{
			//GD.Print("Dinero Segativo");
			DifferenceLabel.Modulate = Colors.Red;
			DifferenceLabel.Text = direference.ToString();
		}
		DifferenceLabel.Visible = true;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if (IsChangingMoney)
		{
			int difference = modMoney - actualMoney;
			if (difference > 0)
			{
				actualMoney++;
				updateLabel();
			}else if (difference < 0){
				actualMoney--;
				updateLabel();
			}else if (difference == 0)
			{
				IsChangingMoney = false;
				DifferenceLabel.Visible = false;
			}
		}
    }
}
