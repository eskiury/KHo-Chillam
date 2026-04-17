using Godot;
using System;

public partial class SmRebound : Node3D
{
	[Export] public GpuParticles3D MainHit;
	[Export] public GpuParticles3D MainHit2;
	[Export] public GpuParticles3D MainHit3;
	[Export] public GpuParticles3D Particles;

	private const string ColorParamName = "ColorParameter";
	public void changeType(SecondaryEffectEnum type)
	{
		
		switch(type)
		{
			case SecondaryEffectEnum.Fire:
                SetParticleParam(MainHit,     new Color(0.8f, 0.1f, 0.0f)); // Rojo oscuro
                SetParticleParam(MainHit2,     new Color(1.0f, 0.6f, 0.1f)); // Naranja brillante
                SetParticleParam(MainHit3, new Color(1.0f, 0.4f, 0.0f)); // Naranja medio
                SetParticleParam(Particles,new Color(1.0f, 0.4f, 0.0f)); 
                break;

            case SecondaryEffectEnum.Ice:
                SetParticleParam(MainHit,     new Color(0.0f, 0.2f, 0.8f)); // Azul oscuro
                SetParticleParam(MainHit2,     new Color(0.6f, 0.9f, 1.0f)); // Cian casi blanco
                SetParticleParam(MainHit3, new Color(0.0f, 0.7f, 1.0f)); // Azul cielo
                SetParticleParam(Particles,new Color(0.0f, 0.7f, 1.0f));
                break;

            case SecondaryEffectEnum.Posion:
                SetParticleParam(MainHit,     new Color(0.1f, 0.6f, 0.1f)); // Verde moco
                SetParticleParam(MainHit2,     new Color(0.7f, 1.0f, 0.0f)); // Lima radiactiva
                SetParticleParam(MainHit3, new Color(0.3f, 0.8f, 0.2f)); 
                SetParticleParam(Particles,new Color(0.3f, 0.8f, 0.2f));
                break;
            case SecondaryEffectEnum.None:
                SetParticleParam(MainHit,     new Color(0.7f, 0.2f, 1.0f)); 
                SetParticleParam(MainHit2,     new Color(0.2f, 0.0f, 0.4f)); 
                SetParticleParam(MainHit3, new Color(1.0f, 0.0f, 0.7f)); 
                SetParticleParam(Particles, new Color(1.0f, 0.0f, 0.7f)); 
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
