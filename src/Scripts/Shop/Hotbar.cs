using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

public partial class Hotbar : Control
{
    //Cosas tienda
    private RandomNumberGenerator randomNumber;
    [Export] private int towerPrice = 50;
    [Export] private int towerSellPrice = 25;
    [Export] private Label priceLabel;
    private StructureClass lastMaskSelected = null;
    private MaskEnum[] towerTypes =  new MaskEnum[] {MaskEnum.NormalDirect,
                                     MaskEnum.IceDirect,
                                     MaskEnum.FireDirect,
                                     MaskEnum.PoisonDirect,
                                     MaskEnum.NormalBomb,
                                     MaskEnum.IceBomb,
                                     MaskEnum.FireBomb,
                                     MaskEnum.PoisonBomb,
                                     MaskEnum.NormalRebound,
                                     MaskEnum.IceRebound,
                                     MaskEnum.FireRebound,
                                     MaskEnum.PoisonRebound};

    //-------------------
    private List<ItemButton> slots = new List<ItemButton>();
    private int quantityBuyedTowers = 0;
    [Export] GridContainer buttonFather;


    public override void _Ready()
    {
        randomNumber = new();
        foreach (Node hijo in buttonFather.GetChildren())
        {
            if (hijo is ItemButton boton)
            {
                slots.Add(boton);
                boton.RightClick += sellTower;
                boton.LeftClick += slotSelectedToChange;

            }   
        }

        GameManager.Instance.SetHotbarRef(this);
        priceLabel.Text = towerPrice.ToString();
    }

    private void sellTower(StructureClass maskToSell)
    {
        //GD.Print("Vendiendo torre");
        if (removeSlot(maskToSell))
        {
            GameManager.Instance.addMoney(towerPrice/2);
        }
    }

    private void slotSelectedToChange(StructureClass maskToChange)
    {
        //GD.Print("Mascara seleccionada: ", maskToChange.MaskType);
        GameManager.Instance.MaskSelected = maskToChange;
    }

    private ItemButton getIconByMask(StructureClass maskToSearch)
    {
        foreach (var item in slots)
        {
            if(item.getActualMask() == maskToSearch)
            {
                return item;
            }
        }
        return null;
    }

    public void removeFromHotbar(StructureClass maskToSearch)
    {
        getIconByMask(maskToSearch).clearMask();
    }

    private void pressPurchase()
    {
        //GD.Print("CLIK EN COMPRAR");
        if(GameManager.Instance.getMoney() >= towerPrice && checkAlgunoVacio())
        {
            GameManager.Instance.subtractMoney(towerPrice);
            buyTower();
            priceLabel.Text = towerPrice.ToString();
        }
    }

    private void buyTower()
    {
        var tipoGenerado = towerTypes[randomize()];
        //GD.Print(tipoGenerado);

        int primerEspacioVacio = -1;
        for (int i = 0; i < slots.Count; i++)
        {
            if (checkEstoyVacio(slots[i]))
            {
                primerEspacioVacio = i;
                break;
            }
        }

        if(primerEspacioVacio != -1)
        {
            quantityBuyedTowers++;
            towerPrice = 50 + (quantityBuyedTowers * 20);
            slots[primerEspacioVacio].setSlot(GameManager.Instance.createMask(tipoGenerado));
            checkUpgrades(slots[primerEspacioVacio]);
        }

    }

    private int randomize()
    {
        float potencia = randomNumber.Randf();

        potencia = MathF.Pow(potencia, 2.0f);

        potencia *= 12.0f;

        int res = (int)potencia;

        return res;
    }

    public bool checkAlgunoVacio()
    {
        foreach (var item in slots)
        {
            if(item.getActualMask() is null)
            {
                return true;
            }
        }
        return false;
    }

    private bool checkEstoyVacio(ItemButton item)
    {
        if(item.getActualMask() is null)
        {
            return true;
        }
        return false;
    }
    
    private bool removeSlot(StructureClass item)
    {
        var itemButtonToDelete = slots.Find(e=>item == e.getActualMask());

        if(itemButtonToDelete != null)
        {
            itemButtonToDelete.clearMask();
            GameManager.Instance.deleteMask(itemButtonToDelete.getActualMask());
            return true;
        }
        return false;
    }

    private void checkUpgrades(ItemButton updater)
    {
        foreach (var item in slots)
        {
            if(updater != item && item.getActualMask() != null && updater != null)
            {
                if(updater.getActualMask().MaskType == item.getActualMask().MaskType && updater.getActualMask().Level == item.getActualMask().Level)
                {
                    if(updater.getActualMask().Level < 3)
                    {
                        updater.getActualMask().increaseLevel();
                        removeSlot(item.getActualMask());

                        updater.setSlot(updater.getActualMask());
                        checkUpgrades(updater);
                        break;
                    }
                }
            }
        }
    }

    public void SafeStructure(StructureClass structureToSafe)
    {
        int primerEspacioVacio = -1;
        for (int i = 0; i < slots.Count; i++)
        {
            if (checkEstoyVacio(slots[i]))
            {
                primerEspacioVacio = i;
                break;
            }
        }

        if(primerEspacioVacio != -1)
        {
            slots[primerEspacioVacio].setSlot(structureToSafe);
            checkUpgrades(slots[primerEspacioVacio]);
        }
    }
}
