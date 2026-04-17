using Godot;
using System;

public partial class TowerBaseScene : StaticBody3D
{
    // --- REFERENCIAS GENERALES ---
    [ExportCategory("Configuración Base")]
    [Export] private Marker3D MaskPosition;
    [Export] private Marker3D CamPosition;
    [Export] private Node3D NodeLights;
    
    // IMPORTANTE: Asigna aquí el nodo raíz de tu escena "root_mascaras"
    [Export] private RootMascaras NodeAnimation; 
    
    [Export] private PackedScene escenaPanelTorre;

    [Export] private ShaderMaterial IceColor;
    [Export] private ShaderMaterial FireColor;
    [Export] private ShaderMaterial PosionColor;
    [Export] private ShaderMaterial NormalColor;



    // --- REFERENCIAS A LAS ESCENAS VISUALES (MODELOS 3D) ---
    [ExportGroup("Visuals - Masks")] 
    [Export] private PackedScene VisualNormalDirect;
    [Export] private PackedScene VisualNormalBomb;
    [Export] private PackedScene VisualNormalRebound;
    
    [Export] private PackedScene VisualFireDirect;
    [Export] private PackedScene VisualFireBomb;
    [Export] private PackedScene VisualFireRebound;
    
    [Export] private PackedScene VisualPoisonDirect;
    [Export] private PackedScene VisualPoisonBomb;
    [Export] private PackedScene VisualPoisonRebound;
    
    [Export] private PackedScene VisualIceDirect;
    [Export] private PackedScene VisualIceBomb;
    [Export] private PackedScene VisualIceRebound;

    // --- ESTADO INTERNO ---
    public bool IsOccupied { get; private set; } = false;
    public StructureClass CurrentTower = null;

    // --- INPUT ---
    public override void _InputEvent(Camera3D camera, InputEvent @event, Vector3 clickPosition, Vector3 clickNormal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        //GD.Print("Clickando torre");
        if (IsOccupied)
        {
            var panel = escenaPanelTorre.Instantiate<MaskStatsPanel>();
            panel.SetMaskStatPanel(CurrentTower, CamPosition);
            GameManager.Instance.OpenMaskStatPanel(panel);
        }
        else
        {
            if(GameManager.Instance.MaskSelected != null)
            {
                GameManager.Instance.BuildTower(this);
            }
        }
    }

    // --- CONSTRUCCIÓN ---

    public void BuildStructure(StructureClass towerScene)
    {
        GD.Print("Contruyo torre");
        IsOccupied = true;
        CurrentTower = towerScene;      
        
        towerScene.Position = MaskPosition.Position;
        NodeLights.Visible = true;
        NodeAnimation.Visible = true;

        // Cargar los modelos visuales
        LoadMaskVisual(CurrentTower.MaskType);
        switch (CurrentTower.MaskType)
        {
            case MaskEnum.FireBomb:
            case MaskEnum.FireDirect:
            case MaskEnum.FireRebound:
                (NodeAnimation as RootMascaras).UpdateMaterials(FireColor);
            break;

            case MaskEnum.IceBomb:
            case MaskEnum.IceDirect:
            case MaskEnum.IceRebound:
                (NodeAnimation as RootMascaras).UpdateMaterials(IceColor);
            break;

            case MaskEnum.PoisonBomb:
            case MaskEnum.PoisonDirect:
            case MaskEnum.PoisonRebound:
                (NodeAnimation as RootMascaras).UpdateMaterials(PosionColor);
            break;

            case MaskEnum.NormalBomb:
            case MaskEnum.NormalDirect:
            case MaskEnum.NormalRebound:
                (NodeAnimation as RootMascaras).UpdateMaterials(NormalColor);
            break;
        }
        
        (NodeAnimation as RootMascaras).showLevel(CurrentTower.Level);
        AddChild(towerScene); 
    }

    public void ResetTower()
    {
        IsOccupied = false;
        RemoveChild(CurrentTower);
        CurrentTower = null;
        
        NodeLights.Visible = false;
        NodeAnimation.Visible = false;

        ClearVisuals();
    }

    // --- GESTIÓN DE MODELOS VISUALES ---

    private void LoadMaskVisual(MaskEnum type)
    {
        // 1. Limpiamos visuales anteriores
        ClearVisuals();

        // 2. Seleccionamos la escena
        PackedScene sceneToLoad = null;

        switch (type)
        {
            case MaskEnum.NormalDirect:  sceneToLoad = VisualNormalDirect; break;
            case MaskEnum.NormalBomb:    sceneToLoad = VisualNormalBomb; break;
            case MaskEnum.NormalRebound: sceneToLoad = VisualNormalRebound; break;
            case MaskEnum.FireDirect:    sceneToLoad = VisualFireDirect; break;
            case MaskEnum.FireBomb:      sceneToLoad = VisualFireBomb; break;
            case MaskEnum.FireRebound:   sceneToLoad = VisualFireRebound; break;
            case MaskEnum.PoisonDirect:  sceneToLoad = VisualPoisonDirect; break;
            case MaskEnum.PoisonBomb:    sceneToLoad = VisualPoisonBomb; break;
            case MaskEnum.PoisonRebound: sceneToLoad = VisualPoisonRebound; break;
            case MaskEnum.IceDirect:     sceneToLoad = VisualIceDirect; break;
            case MaskEnum.IceBomb:       sceneToLoad = VisualIceBomb; break;
            case MaskEnum.IceRebound:    sceneToLoad = VisualIceRebound; break;
            
            case MaskEnum.None:
            default:
                //GD.PrintErr($"No hay visual asignado para el tipo: {type}");
                return;
        }

        // 3. BUSCAMOS LOS MESH INSTANCE Y AÑADIMOS EL VISUAL
        if (sceneToLoad != null)
        {
            // CAMBIO AQUÍ: Buscamos dentro de cada RootMascara su hijo MeshInstance3D
            var slot1 = NodeAnimation.Mask1;
            var slot2 = NodeAnimation.Mask2;
            var slot3 = NodeAnimation.Mask3;

            // Verificación de seguridad
            if (slot1 == null || slot2 == null || slot3 == null)
            {
                //GD.PrintErr("¡ERROR! No se encuentran los nodos 'MeshInstance3D' dentro de los RootMascara. Revisa la estructura.");
                return;
            }

            SpawnVisualInSlot(sceneToLoad, slot1);
            SpawnVisualInSlot(sceneToLoad, slot2);
            SpawnVisualInSlot(sceneToLoad, slot3);
        }
    }

    private void SpawnVisualInSlot(PackedScene scene, MeshInstance3D slot)
    {
        if (slot == null) return;

        var instance = scene.Instantiate<StructureClass>();
        slot.Mesh = instance.GetNodeOrNull<MeshInstance3D>("Mesh").Mesh;
        slot.Rotation = instance.GetNodeOrNull<MeshInstance3D>("Mesh").Rotation;
        instance.Position = Vector3.Zero; 
        
        PlaySpawnAnimation(instance);
    }

    private void ClearVisuals()
    {
        // También necesitamos buscarlos para limpiarlos
        var slot1 = NodeAnimation.Mask1;
        var slot2 = NodeAnimation.Mask2;
        var slot3 = NodeAnimation.Mask3;

        slot1.Mesh = null;
        slot2.Mesh = null;
        slot3.Mesh = null;
    }
    
    private void PlaySpawnAnimation(Node3D visual)
    {
        // CAMBIO: No uses Vector3.Zero absoluto, usa un valor muy pequeño (epsilon)
        // Esto evita que la matriz se rompa (det == 0) al intentar invertirla.
        visual.Scale = new Vector3(0.001f, 0.001f, 0.001f);
        
        Tween tween = CreateTween();
        tween.TweenProperty(visual, "scale", Vector3.One, 0.5f)
                .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
    }
}