using Godot;
using System;

public partial class BombProjectile : ProjectileClass
{
    [Export] private Area3D ExplosionArea3D = null;
    [Export] protected SmProjectile actualShaderProjectile;

    // Variable para cargar la escena (asígnala en el inspector del proyectil o cárgala con GD.Load)
    [Export] public PackedScene ExplosionScene;

    public override void _Ready()
    {
        base._Ready();
        if(ExplosionArea3D == null)
        {
            //GD.PrintErr("Bala sin explosion area.");
        }
    }

    protected override void ApplyDamage()
    {
        TargetEnemy.TakeDamage(Damage);
        if(!IsInstanceValid(TargetEnemy)) return;
        ApplySecondaryEffect(TargetEnemy);
        ExplosionArea3D.Monitoring = true;
        var bodies = ExplosionArea3D.GetOverlappingBodies();
        foreach(var body in bodies)
        {
            if (body.GetParent<Node3D>() is EnemyClass enemy && enemy != TargetEnemy)
            {
                enemy.TakeDamage(Damage/2);
                ApplySecondaryEffect(enemy);
                
            }
        }

        //GD.Print("Explota la bomba");
        SpawnExplosion(this.Position ,SecundaryEffect);
        QueueFree();
    }

    public override void SetProjectile(EnemyClass targetEnemy, int damage, int level)
	{
		base.SetProjectile( targetEnemy,  damage,  level);
		actualShaderProjectile.changeType(SecundaryEffect);
	}

    // Función que llamas cuando el proyectil choca
    private void SpawnExplosion(Vector3 impactPosition, SecondaryEffectEnum type)
    {
        if (ExplosionScene != null)
        {
            // 1. Instanciar la escena
            SmExplosion newExplosion = (SmExplosion)ExplosionScene.Instantiate();

            // 2. Añadirla al árbol de nodos (a la escena actual)
            GetTree().Root.AddChild(newExplosion);

            // 3. Colocarla en el lugar del impacto
            newExplosion.GlobalPosition = impactPosition;

            // 4. ¡BANG! (Pasamos el tipo para que ponga los colores correctos)
            newExplosion.Explode(type);
        }
    }


}
