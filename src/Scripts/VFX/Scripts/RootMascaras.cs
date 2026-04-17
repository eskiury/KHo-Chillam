using Godot;
using System;

public partial class RootMascaras : Node3D
{
    [ExportGroup("Configuración de Rotación")]
    [Export] public bool IsActive = true;
    
    [ExportSubgroup("Velocidades")]
    [Export] public float OrbitSpeed = 90.0f;      // Velocidad de la vuelta completa (Orbita)
    [Export] public float SelfRotationSpeed = 180.0f; // Velocidad de giro de la máscara sobre sí misma

    [ExportGroup("Referencias")]
    // Referencias a los Nodos que contienen las mallas (MeshInstance3D)
    [Export] public MeshInstance3D Mask1;
    [Export] public MeshInstance3D Mask2;
    [Export] public MeshInstance3D Mask3;

    [Export] private Node3D Root;

    public override void _Process(double delta)
    {
        if (!IsActive) return;
        // 1. ROTACIÓN ORBITAL (El padre gira, arrastrando a los hijos)
        float orbitStep = Mathf.DegToRad(OrbitSpeed) * (float)delta;
        Root.RotateY(orbitStep);

        // 2. ROTACIÓN LOCAL (Cada hijo gira sobre su propio eje)
        float selfStep = Mathf.DegToRad(SelfRotationSpeed) * (float)delta;

        if (Mask1 != null) Mask1.RotateY(selfStep);
        if (Mask2 != null) Mask2.RotateY(selfStep);
        if (Mask3 != null) Mask3.RotateY(selfStep);
    }

    // He simplificado este método porque ahora las mallas se gestionan desde TowerBaseScene
    // Aquí solo nos interesa activar o desactivar el giro.
    public void SetActive(bool active)
    {
        IsActive = active;
        
        // Opcional: Si quieres ocultar todo el sistema cuando no está activo
        this.Visible = active;
    }

    // Método público que llamarás desde fuera (desde TowerBaseScene, por ejemplo)
    public void UpdateMaterials(ShaderMaterial newMaterial)
    {
        // Aplicamos el material a cada una de las 3 ramas
        ApplyMaterialRecursive(Mask1, newMaterial);
        ApplyMaterialRecursive(Mask2, newMaterial);
        ApplyMaterialRecursive(Mask3, newMaterial);
    }

    // Función auxiliar que busca mallas dentro de los nodos y sus hijos
    private void ApplyMaterialRecursive(Node node, Material mat)
    {
        if (node == null) return;

        // 1. Si el nodo actual es una Malla, le aplicamos el override
        if (node is MeshInstance3D meshInstance)
        {
            meshInstance.MaterialOverride = mat;
        }

        // 2. Buscamos también en todos los hijos (por si la máscara es una escena con varias piezas)
        foreach (Node child in node.GetChildren())
        {
            ApplyMaterialRecursive(child, mat);
        }
    }

    public void showLevel(int level)
    {
        switch (level)
        {
            case 1:
                Mask1.Visible = true;
                Mask2.Visible = false;
                Mask3.Visible = false;
            break;
            case 2:
                Mask1.Visible = true;
                Mask2.Visible = true;
                Mask3.Visible = false;
            break;
            case 3:
                Mask1.Visible = true;
                Mask2.Visible = true;
                Mask3.Visible = true;
            break;
        }
    }
}