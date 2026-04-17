using Godot;
using System;

public partial class EndPanel : Control
{
	private void OnBotonVolverAlMenuPressed()
	{
		//GD.Print("Volviendo al menú principal...");
		GameManager.Instance.OnBotonVolverAlMenuPressed();
	}

	private void OnBotonReiniciarNivelPressed()
	{
		//GD.Print("Reiniciando el nivel...");
		GameManager.Instance.OnBotonReiniciarPartidaPressed();
	}

}
