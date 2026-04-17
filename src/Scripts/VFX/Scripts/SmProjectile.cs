using Godot;
using System;

public partial class SmProjectile : Node3D
{
	[Export] public GpuParticles3D OuterAura;
	[Export] public GpuParticles3D InnerAura;
	[Export] public GpuParticles3D TrailVertical;
	[Export] public GpuParticles3D TrailVertical2;

	private const string ColorParamName = "Color1";
	public void changeType(SecondaryEffectEnum type)
	{
		
		switch(type)
		{
			case SecondaryEffectEnum.Fire:
                SetParticleParam(OuterAura,     new Color(0.8f, 0.1f, 0.0f)); // Rojo oscuro
                SetParticleParam(InnerAura,     new Color(1.0f, 0.6f, 0.1f)); // Naranja brillante
                SetParticleParam(TrailVertical, new Color(1.0f, 0.4f, 0.0f)); // Naranja medio
                SetParticleParam(TrailVertical2,new Color(1.0f, 0.4f, 0.0f)); 
                break;

            case SecondaryEffectEnum.Ice:
                SetParticleParam(OuterAura,     new Color(0.0f, 0.2f, 0.8f)); // Azul oscuro
                SetParticleParam(InnerAura,     new Color(0.6f, 0.9f, 1.0f)); // Cian casi blanco
                SetParticleParam(TrailVertical, new Color(0.0f, 0.7f, 1.0f)); // Azul cielo
                SetParticleParam(TrailVertical2,new Color(0.0f, 0.7f, 1.0f));
                break;

            case SecondaryEffectEnum.Posion:
                SetParticleParam(OuterAura,     new Color(0.1f, 0.6f, 0.1f)); // Verde moco
                SetParticleParam(InnerAura,     new Color(0.7f, 1.0f, 0.0f)); // Lima radiactiva
                SetParticleParam(TrailVertical, new Color(0.3f, 0.8f, 0.2f)); 
                SetParticleParam(TrailVertical2,new Color(0.3f, 0.8f, 0.2f));
                break;
            case SecondaryEffectEnum.None:
                SetParticleParam(OuterAura,     new Color(0.7f, 0.2f, 1.0f)); 
                SetParticleParam(InnerAura,     new Color(0.2f, 0.0f, 0.4f)); 
                SetParticleParam(TrailVertical, new Color(1.0f, 0.0f, 0.7f)); 
                SetParticleParam(TrailVertical2, new Color(1.0f, 0.0f, 0.7f)); 
                break;
		}
	}
	private void SetParticleParam(GpuParticles3D particle, Color targetColor)
	{
		if (particle == null) return;

		Material matToEdit = particle.MaterialOverride;
		if(matToEdit is ShaderMaterial shaderMat)
		{
            ShaderMaterial uniqueMat = (ShaderMaterial)shaderMat.Duplicate();
            uniqueMat.SetShaderParameter(ColorParamName, targetColor);
            particle.MaterialOverride = uniqueMat;
		}
	}
}
