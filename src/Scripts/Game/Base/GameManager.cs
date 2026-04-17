using Godot;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection;
using System.Runtime.Serialization;

public partial class GameManager : Node3D
{
	[Signal]
	public delegate void OnMoneyChangeEventHandler(int actualMoney);

	[Signal]
	public delegate void OnLiveChangeEventHandler(int actualLives);

	public int LivesLeft = 0;
	private int money = 0;
	private int LivesLeftDefault = 15;
	private int moneyDefault = 200;

	public static GameManager Instance;
	public List<StructureClass> MaskInventory = new List<StructureClass>();
	public Hotbar HotbarRef = null;
	public StructureClass MaskSelected = null;
	private PackedScene SelectedStructurePackedScene;

	private PackedScene directNormal_scene = null;
	private PackedScene directBomb_scene = null;
	private PackedScene directRebound_scene = null;

	private PackedScene fireNormal_scene = null;
	private PackedScene fireBomb_scene = null;
	private PackedScene fireRebound_scene = null;

	private PackedScene iceNormal_scene = null;
	private PackedScene iceBomb_scene = null;
	private PackedScene iceRebound_scene = null;

	private PackedScene poisonNormal_scene = null;
	private PackedScene poisonBomb_scene = null;
	private PackedScene poisonRebound_scene = null;

	private PackedScene win_scene = null;
	private PackedScene lose_scene = null;
	private MaskStatsPanel actualPanel = null;

	private LevelClass actualLevel = null;
	private Camera3D GameCamref;
	private Transform3D InitCamTransform;
	private Transform3D EndCamTransform;

	private bool IsClosingUp = false;
	private bool IsOpeningUp = false;


	public override void _Ready()
	{
		Instance = this;
		// //GD.Print("mi dinero es:", money);
		SelectedStructurePackedScene = GD.Load<PackedScene>("res://Scenes/Game/Base/structure_scene.tscn");

		//Referencias a mascaras
		directNormal_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/normal_direct_mask_scene.tscn");
		directBomb_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/normal_bomb_mask_scene.tscn");
		directRebound_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/normal_rebound_mask_scene.tscn");

		fireNormal_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/fire_direct_mask_scene.tscn");
		fireBomb_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/fire_bomb_mask_scene.tscn");
		fireRebound_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/fire_rebound_mask_scene.tscn");

		iceNormal_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/ice_direct_mask_scene.tscn");
		iceBomb_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/ice_bomb_mask_scene.tscn");
		iceRebound_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/ice_rebound_mask_scene.tscn");

		poisonNormal_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/poison_direct_mask_scene.tscn");
		poisonBomb_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/poison_bomb_mask_scene.tscn");
		poisonRebound_scene = GD.Load<PackedScene>("res://Scenes/Game/Masks/poison_rebound_mask_scene.tscn");

		win_scene = GD.Load<PackedScene>("res://Scenes/Game/UI/win.tscn");
		lose_scene = GD.Load<PackedScene>("res://Scenes/Game/UI/gameOver.tscn");

		ResetGame();
	}



	public void ResetGame()
	{
		LivesLeft = LivesLeftDefault;
		money = moneyDefault;
		GameCamref = null;
		actualLevel = null;
		MaskInventory.Clear();

		EmitSignal(SignalName.OnLiveChange, LivesLeft);
	}

	public void BuildTower(TowerBaseScene baseScene)
	{
		// Lógica para construir una torre en la base dada
		//GD.Print("Construyendo torre en la base seleccionada.");

		baseScene.BuildStructure(MaskSelected);
		resetSelection();
	}

	public void resetSelection()
	{
		SelectedStructurePackedScene = null;
		HotbarRef.removeFromHotbar(MaskSelected);
		MaskSelected = null;
	}

	public int getMoney()
	{
		return money;
	}

	public void subtractMoney(int quantity)
	{
		money = money - quantity;

		if (money < 0)
		{
			money = 0;
		}
		EmitSignal(SignalName.OnMoneyChange,money);
	}

	public void addMoney(int quantity)
	{
		money = money + quantity;
		EmitSignal(SignalName.OnMoneyChange,money);
	}

	public StructureClass createMask(MaskEnum type)
	{
		StructureClass actualStructure = null;
		switch (type)
		{
			case MaskEnum.NormalDirect:
				actualStructure = directNormal_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.NormalBomb:
				actualStructure = directBomb_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.NormalRebound:
				actualStructure = directRebound_scene.Instantiate<StructureClass>();
				break;


			case MaskEnum.FireDirect:
				actualStructure = fireNormal_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.FireBomb:
				actualStructure = fireBomb_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.FireRebound:
				actualStructure = fireRebound_scene.Instantiate<StructureClass>();
				break;


			case MaskEnum.IceDirect:
				actualStructure = iceNormal_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.IceBomb:
				actualStructure = iceBomb_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.IceRebound:
				actualStructure = iceRebound_scene.Instantiate<StructureClass>();
				break;


			case MaskEnum.PoisonDirect:
				actualStructure = poisonNormal_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.PoisonBomb:
				actualStructure = poisonBomb_scene.Instantiate<StructureClass>();
				break;

			case MaskEnum.PoisonRebound:
				actualStructure = poisonRebound_scene.Instantiate<StructureClass>();
				break;


		}

		MaskInventory.Add(actualStructure);
		return actualStructure;
	}

	public void deleteMask(StructureClass item)
	{
		MaskInventory.Remove(MaskInventory.Find(e => item == e));
	}



	public void OnEnemyDied(int reward)
	{
		addMoney(reward);
	}

	public void OnEnemyFinishPath()
	{
		LivesLeft--;
		(actualLevel as LevelClass).Sparks.Emitting = true;
		//GD.Print("Vidas actuales: ", LivesLeft);
		EmitSignal(SignalName.OnLiveChange, LivesLeft);
		if (LivesLeft <= 0)
		{
			Node loseUI = lose_scene.Instantiate();
			GetTree().Root.AddChild(loseUI);
			GetTree().Paused = true;
			//GD.Print("El jugador se ha quedado sin vidas, incluyendo lógica del reinicio de partida");

		}
	}

	public void OnWin()
	{
		Node gameWinUI = win_scene.Instantiate();
		GetTree().Root.AddChild(gameWinUI);
		GetTree().Paused = true;
	}


	public void OnBotonVolverAlMenuPressed()
	{
		ResetGame();
		GetTree().Paused = false;
		if (GetTree().Root.HasNode("GameOver"))
		{
			GetTree().Root.RemoveChild(GetTree().Root.GetNode("GameOver"));
		}

		if (GetTree().Root.HasNode("Win"))
		{
			GetTree().Root.RemoveChild(GetTree().Root.GetNode("Win"));
		}

		GetTree().ChangeSceneToFile("res://Scenes/Menu/MainMenu_scene.tscn");

	}

	public void OnBotonReiniciarPartidaPressed()
	{
		GetTree().Paused = false;

		var level = GetTree().Root.FindChild("Level_Example_Class", true, false) as LevelClass;

		int indiceDeReinicio = 0;

		if (level != null)
		{

			int rondaActual = level.GetRondaActualIndice();


			if (rondaActual >= 10)
			{
				indiceDeReinicio = rondaActual - 10;
			}
		}
		LivesLeft = LivesLeftDefault;

		level.rondaActualIndice = indiceDeReinicio;


		Node gameOverNode = GetTree().Root.GetNodeOrNull("GameOver");
		if (gameOverNode != null)
		{
			GetTree().Root.RemoveChild(gameOverNode);
		}

		GetTree().ChangeSceneToFile("res://Scenes/Game/Base/game_scene.tscn");

		EmitSignal(SignalName.OnLiveChange, LivesLeft);
	}

	public void SetCamRef(Camera3D camera)
	{
		GameCamref = camera;
	}

	public void SetLevelRef(LevelClass level)
	{
		actualLevel = level;
	}

	public void SetHotbarRef(Hotbar hotbar)
	{
		HotbarRef = hotbar;
	}

	public void OpenMaskStatPanel(MaskStatsPanel panelToOpen)
	{
		//Movimiento de cámara y colocamos el control
		actualPanel = panelToOpen;
		actualPanel.OnCloseButton += ClosePanel;
		actualPanel.OnPickUpButton += TryPickUpMachine;
		GetTree().Root.GetNodeOrNull<Node3D>("Game").GetNodeOrNull<Control>("Control").AddChild(panelToOpen);
		StartCloseUpPan();
	}

	private void StartCloseUpPan()
	{
		InitCamTransform = GameCamref.GlobalTransform;
		EndCamTransform = actualPanel.CloseUpPosition.GlobalTransform;
		IsClosingUp = true;
		float peso = 0.1f; // Ajusta este valor para la velocidad (0.0 a 1.0)
		GameCamref.GlobalTransform = GameCamref.GlobalTransform.InterpolateWith(EndCamTransform, peso);
	}

	public void ClosePanel()
	{
		GetTree().Root.GetNodeOrNull<Node3D>("Game").GetNodeOrNull<Control>("Control").RemoveChild(actualPanel);
		actualPanel.QueueFree();
		IsClosingUp = false;
		IsOpeningUp = true;
	}

	public void TryPickUpMachine(StructureClass structureToPick)
	{
		if (HotbarRef.checkAlgunoVacio())
		{
			structureToPick.GetParent<TowerBaseScene>().ResetTower();
			HotbarRef.SafeStructure(structureToPick);
			//Se puede meter en la hotbar
			ClosePanel();
		}
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if (IsClosingUp)
		{
			float peso = 0.1f; // Ajusta este valor para la velocidad (0.0 a 1.0)
			GameCamref.GlobalTransform = GameCamref.GlobalTransform.InterpolateWith(EndCamTransform, peso);
		}else if(IsOpeningUp){
			float peso = 0.1f; // Ajusta este valor para la velocidad (0.0 a 1.0)
			GameCamref.GlobalTransform = GameCamref.GlobalTransform.InterpolateWith(InitCamTransform, peso);
		}
    }


}
