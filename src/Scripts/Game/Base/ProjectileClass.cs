using Godot;
using System;
using System.Collections.Generic;

public enum ProjectileTypeEnum
{
	Direct = 1,
	Rebound,
	Bomb
}

public enum SecondaryEffectEnum
{
	None = 1,
	Posion,
	Fire,
	Ice
}
[GlobalClass]
public partial class ProjectileClass : Node3D
{
    [Export] protected float Speed = 10.0f;
    [Export] protected SecondaryEffectEnum SecundaryEffect = SecondaryEffectEnum.None;
    [Export] private AudioStream SFXhielo;
    [Export] private AudioStream SFXfuego;
    [Export] private AudioStream SFXpoison;
    [Export] private AudioStream SFXnormal;
    [Export] protected Area3D areaCollisionProjectile;
    
	


	//[Export] protected  actualShaderExplosion;


    protected int Damage;
    protected int Level;


    protected EnemyClass TargetEnemy = null;


	public virtual void SetProjectile(EnemyClass targetEnemy, int damage, int level)
	{
		TargetEnemy = targetEnemy;
		Damage = damage;
		Level = level;
	}

	public override void _Process(double delta)
	{
		//Si el enemigo objetivo ya no existe, eliminar el proyectil
		if (IsInstanceValid(TargetEnemy) == false || TargetEnemy == null || !TargetEnemy.IsInsideTree())
		{
			QueueFree();
			return;
		}

		Vector3 targetPos = TargetEnemy.GlobalPosition;
		Vector3 myPos = this.GlobalPosition;

        if (targetPos != myPos) LookAt(targetPos, Vector3.Up);

        //Movimiento del proyectil hacia el enemigo
        Vector3 direction = (targetPos - myPos).Normalized();
        GlobalPosition += direction * Speed * (float)delta;

    }

    protected void OnBodyEntered(Node3D body)
    {
        if (body.GetParent<Node3D>() is EnemyClass enemy && enemy == TargetEnemy)
        {
            //Hacer swicht de tipo de efecto secundario para implementar los sonidos

            //Aplicar daño al enemigo
            switch (SecundaryEffect)
            {
                case SecondaryEffectEnum.Ice:
                    MusicManager.Instance.PlaySFX(SFXhielo);
                    //GD.Print("Proyectil hielo impactando");
                    break;
                case SecondaryEffectEnum.Fire:
                    MusicManager.Instance.PlaySFX(SFXfuego);
                    //GD.Print("Proyectil fuego impactando");
                    break;
                case SecondaryEffectEnum.Posion:
                    MusicManager.Instance.PlaySFX(SFXpoison);
                    //GD.Print("Proyectil poison impactando");
                    break;
                case SecondaryEffectEnum.None:
                    MusicManager.Instance.PlaySFX(SFXnormal);
                    //GD.Print("Proyectil normal impactando");
                    break;

            }
            ApplyDamage();
        }
    }

    protected virtual void ApplyDamage()
    { }

    protected void ApplySecondaryEffect(EnemyClass enemy)
    {
        switch (SecundaryEffect)
        {
            case SecondaryEffectEnum.None:
                break;

            case SecondaryEffectEnum.Posion:

                enemy.ApplySecondaryEffect(SecondaryEffectEnum.Posion, Level);
                break;

            case SecondaryEffectEnum.Fire:
                enemy.ApplySecondaryEffect(SecondaryEffectEnum.Fire, Level);
                break;

            case SecondaryEffectEnum.Ice:
                enemy.ApplySecondaryEffect(SecondaryEffectEnum.Ice, Level);
                break;
        }
    }

}
