using Godot;
using System;

public partial class ItemButton : TextureButton
{
    [Signal] public delegate void RightClickEventHandler(StructureClass buttonMask);
    [Signal] public delegate void LeftClickEventHandler(StructureClass buttonMask);

    private StructureClass ActualMask;
    [Export] TextureRect icon;
    [Export] HBoxContainer starsContainer;

    [Export] private Texture2D emptyStarPath;
    [Export] private Texture2D completeStarPath;


    public void setSlot(StructureClass item)
    {
        ActualMask = item;
        icon.Texture = ActualMask.MaskSprite;
        int level = ActualMask.Level;

        foreach(var textures in starsContainer.GetChildren())
        {
            if(textures is TextureRect text)
            {
                if(level == 0) break;
                text.Texture = completeStarPath;
                level--;
            }
        }
    }

    public StructureClass getActualMask()
    {
        return ActualMask;
    }

    public void clearMask()
    {
        ActualMask = null;
        icon.Texture = null;
        foreach(var textures in starsContainer.GetChildren())
        {
            if(textures is TextureRect text)
            {
                text.Texture = emptyStarPath;
            }
        }

    }

    public void _on_gui_input(InputEvent @event)
    {
        // Verificamos si el evento es un clic de ratón
        if (@event is InputEventMouseButton mouseEvent)
        {
            // Verificamos si el botón fue presionado (y no soltado)
            if (mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Right)
                {
                    //GD.Print("CLickando derecho");

                    if(ActualMask != null)
                    {
                        EmitSignal(SignalName.RightClick, ActualMask);
                        clearMask();
                    }                    
                }
                else if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    if(ActualMask != null)
                    {
                        EmitSignal(SignalName.LeftClick, ActualMask);
                    }     
                }
            }
        }
    }
}
