using Godot;
using System;
using System.IO;

public partial class DirectProjectile : ProjectileClass
{
	[Export] protected SmProjectile actualShader;
	protected override void ApplyDamage()
	{
		TargetEnemy.TakeDamage(Damage);
		if(!IsInstanceValid(TargetEnemy)) return;
		ApplySecondaryEffect(TargetEnemy);
		QueueFree();
	}
	public override void SetProjectile(EnemyClass targetEnemy, int damage, int level)
	{
		base.SetProjectile( targetEnemy,  damage,  level);
		actualShader.changeType(SecundaryEffect);
	}
}
