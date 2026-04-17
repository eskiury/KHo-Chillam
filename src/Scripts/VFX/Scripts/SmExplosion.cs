using Godot;
using System;

public partial class SmExplosion : Node3D
{
    // Referencias a tus 4 nodos de partículas
    [Export] public GpuParticles3D Debris;
    [Export] public GpuParticles3D SmokeMesh;
    [Export] public GpuParticles3D Fire;
    [Export] public GpuParticles3D Ember;

    private const string ColorParamName = "Color1";

    // Método principal para iniciar la explosión
    public void Explode(SecondaryEffectEnum type)
    {
        // 1. Configuramos los colores según el tipo
        ApplyColors(type);

        // 2. Activamos la emisión en todos los hijos
        if(Debris != null) Debris.Emitting = true;
        if(SmokeMesh != null) SmokeMesh.Emitting = true;
        if(Fire != null) Fire.Emitting = true;
        if(Ember != null) Ember.Emitting = true;

        // 3. Autodestrucción: Calculamos cuándo borrar el nodo para no dejar basura en memoria
        // Tomamos el tiempo de vida más largo (por ejemplo 2 segundos) y borramos el nodo
        GetTree().CreateTimer(2.5f).Timeout += () => QueueFree();
    }

    private void ApplyColors(SecondaryEffectEnum type)
    {
        switch(type)
        {
            case SecondaryEffectEnum.Fire:
                // Asignación sugerida (puedes cambiar los colores a tu gusto)
                SetParticleParam(Fire,      new Color(0.8f, 0.1f, 0.0f)); // Fuego Rojo
                SetParticleParam(Ember,     new Color(1.0f, 0.6f, 0.1f)); // Chispas Naranjas
                SetParticleParam(SmokeMesh, new Color(0.2f, 0.2f, 0.2f)); // Humo gris oscuro
                SetParticleParam(Debris,    new Color(0.1f, 0.1f, 0.1f)); // Escombros oscuros
                break;

            case SecondaryEffectEnum.Ice:
                SetParticleParam(Fire,      new Color(0.0f, 0.2f, 0.8f)); // Explosión Azul
                SetParticleParam(Ember,     new Color(0.6f, 0.9f, 1.0f)); // Chispas Cian
                SetParticleParam(SmokeMesh, new Color(0.8f, 0.9f, 1.0f)); // Niebla blanca fría
                SetParticleParam(Debris,    new Color(0.5f, 0.7f, 1.0f)); // Hielo
                break;

            case SecondaryEffectEnum.Posion:
                SetParticleParam(Fire,      new Color(0.1f, 0.6f, 0.1f)); // Explosión Verde
                SetParticleParam(Ember,     new Color(0.7f, 1.0f, 0.0f)); // Chispas Lima
                SetParticleParam(SmokeMesh, new Color(0.1f, 0.2f, 0.1f)); // Gas tóxico oscuro
                SetParticleParam(Debris,    new Color(0.2f, 0.4f, 0.2f)); // Limo
                break;

            case SecondaryEffectEnum.None:
                SetParticleParam(Fire,      new Color(0.7f, 0.2f, 1.0f)); 
                SetParticleParam(Ember,     new Color(1.0f, 0.0f, 0.7f)); 
                SetParticleParam(SmokeMesh, new Color(0.3f, 0.1f, 0.4f)); 
                SetParticleParam(Debris,    new Color(0.5f, 0.5f, 0.5f)); 
                break;
        }
    }

    // Tu función helper original (se mantiene igual)
    private void SetParticleParam(GpuParticles3D particle, Color targetColor)
    {
        if (particle == null) return;

        Material matToEdit = particle.MaterialOverride;
        // Si no tiene override, intenta coger el material de proceso (DrawPass o ProcessMaterial según tu shader)
        // Asumo que usas MaterialOverride como en tu script anterior.
        
        if(matToEdit is ShaderMaterial shaderMat)
        {
            // Importante: Duplicamos para que no cambie el color de todas las explosiones futuras
            ShaderMaterial uniqueMat = (ShaderMaterial)shaderMat.Duplicate();
            uniqueMat.SetShaderParameter(ColorParamName, targetColor);
            particle.MaterialOverride = uniqueMat;
        }
    }
}