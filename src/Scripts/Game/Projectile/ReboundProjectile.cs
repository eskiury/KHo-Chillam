using Godot;
using System;

public partial class ReboundProjectile : ProjectileClass
{
    [Export] private int ReboundQuantity = 0;
    [Export] private Area3D PosibleReboundArea = null;
    [Export] protected SmRebound actualShaderRebound;

    public override void _Ready()
    {
        base._Ready();
        if(PosibleReboundArea == null)
        {
            //GD.PrintErr("Proyectil Rebote sin area asignada");
        }
    }


    protected override void ApplyDamage()
    {
        TargetEnemy.TakeDamage(Damage);
        if(!IsInstanceValid(TargetEnemy)) return;
        ApplySecondaryEffect(TargetEnemy);
        ReboundQuantity--;
        if(ReboundQuantity == 0)
        {
            QueueFree();
            return;
        }

        //Se va al siguiente (el más cercano al actual)
        var bodies = PosibleReboundArea.GetOverlappingBodies();

        float minDistance = 99.9f;
        EnemyClass closestEnemy = null;

        foreach(var body in bodies)
        {
            if (body.GetParent<Node3D>() is EnemyClass enemy && enemy != TargetEnemy)
            {
                if (IsInstanceValid(enemy))
                {
                    if (enemy.GlobalPosition.DistanceTo(TargetEnemy.GlobalPosition) < minDistance)
                    {
                        closestEnemy = enemy;
                    }
                }
            }
        }
        if(closestEnemy == null){
            QueueFree();
            return;
        }
        ChangeTargetEnemy(closestEnemy);
    }

    private void ChangeTargetEnemy(EnemyClass enemy)
    {
        TargetEnemy = enemy;

        var bodies = areaCollisionProjectile.GetOverlappingBodies();

        foreach(var body in bodies)
        {
            if (body.GetParent<Node3D>() is EnemyClass collissionEnemy && collissionEnemy == TargetEnemy)
            {
               ApplyDamage();
               break;
            }
        }
            
    }
    
    public override void SetProjectile(EnemyClass targetEnemy, int damage, int level)
	{
		base.SetProjectile( targetEnemy,  damage,  level);
		actualShaderRebound.changeType(SecundaryEffect);
	}

}
