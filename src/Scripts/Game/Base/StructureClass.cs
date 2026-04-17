using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;

public enum MaskEnum
{
	NormalDirect = 0,
	NormalBomb,
	NormalRebound,
	PoisonDirect,
	PoisonBomb,
	PoisonRebound,
	FireDirect,
	FireBomb,
	FireRebound,
	IceDirect,
	IceBomb,
	IceRebound,
	None
}

public partial class StructureClass : Node3D
{
	[Export] private PackedScene _ProjectileScene;
	[Export] private Area3D _Area;
	[Export] private Marker3D _Cannon;

	[Export] public MaskEnum MaskType = MaskEnum.None;
	[Export] public Texture2D MaskSprite;
	[Export] public String MaskName;
	[Export] public int Level = 1;
	[Export] public float AttackCooldown = 1.0f;
	[Export] public float AttackRadius = 5.0f;
	[Export] public int AttackDamage = 10;
	[Export] private AudioStream SFXnormal;
	private List<EnemyClass> ValidEnemies = new List<EnemyClass>();
	private float AcumulatedTime = 0.0f;




	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		if (_Area == null)
		{
			//GD.PrintErr("Estructura sin area asociada.");
		}
		else
		{
			_Area.GetNodeOrNull<CollisionShape3D>("CollisionShape3D").Shape = new CylinderShape3D() { Radius = AttackRadius };
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print($"Los enemigos valid de {this} son: ");
		for (int i = 0; i < ValidEnemies.Count; i++)
		{
			var enemy = ValidEnemies[i];
			// Imprimimos el índice, su HP y su velocidad para diferenciarlos
			if(this.IsInsideTree())GD.Print($"Enemigo [{i}] {enemy} | HP: {enemy.Hp} | Velocidad: {enemy.SpeedMS} m/s | Anim: {enemy.WalkAnimationName}");
		}
		// GD.Print("Soy nodo ", this," Accumulated time: ",AcumulatedTime," Delta: ",(float) delta);
		AcumulatedTime += (float)delta;
		if (AcumulatedTime >= AttackCooldown)
		{
			AcumulatedTime = 0.0f;
			AttackEnemy();
		}
	}

	private void onBodySpoted(Node3D body)
	{
		// //GD.Print("He encontrado un body.");
		if (body.GetParent<Node3D>() is EnemyClass enemy)
		{
			GD.Print("Es un enemigo.");
			if(ValidEnemies.Find(e=>e == enemy) == null) ValidEnemies.Add(enemy);
		}
	}

	private void onBodyExited(Node3D body)
	{
		if (body.GetParent<Node3D>() is EnemyClass enemy)
		{
			// //GD.Print("Enemigo borrado.");
			foreach(var enemyInSlot in ValidEnemies.FindAll(e=>e == enemy)){
				ValidEnemies.Remove(enemyInSlot);
			}
			
		}
	}

	private void AttackEnemy()
	{
		// GD.Print("Creo proyetil");
		//Limpiar la lista de enemigos no validos
		ValidEnemies.RemoveAll(e => !GodotObject.IsInstanceValid(e));

		//Si no hay enemigos validos, salir
		if (ValidEnemies.Count == 0) return;

		MusicManager.Instance.PlaySFX(SFXnormal);
		//PROVISIONAL: Atacar al primer enemigo de la lista. Luego se puede mejorar con prioridades, etc. Este método es general, solo hay que cambiar el tipo de proyectil.
		EnemyClass targetEnemy = SelectEnemyToAttack();
		ProjectileClass projectile = _ProjectileScene.Instantiate<ProjectileClass>();
		projectile.Position = _Cannon != null ? _Cannon.GlobalPosition : this.GlobalPosition;
		projectile.SetProjectile(targetEnemy, AttackDamage, Level);
		GetTree().Root.AddChild(projectile);
	}

	private EnemyClass SelectEnemyToAttack()
	{
		//Por ahora, simplemente devolver el primer enemigo de la lista
		return ValidEnemies[0];
	}

	public void increaseLevel()
	{
		if (Level >= 3)
		{
			return;
		}
		Level++;


		AttackDamage = Mathf.RoundToInt(AttackDamage * 2.2f);

		AttackRadius *= 1.15f;

		AttackCooldown *= 0.85f;
	}
}
