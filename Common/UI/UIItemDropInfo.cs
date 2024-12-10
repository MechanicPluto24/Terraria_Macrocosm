using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI;

// Adapted from Terraria.GameContent.UI.Elements.UIBestiaryInfoItemLine
public class UIItemDropInfo : UIPanel
{
    private readonly bool hideMouseOver = false;

    private readonly UIInventoryItemIcon itemIcon;
    private readonly UITextPanel<string> textPanel;

    public bool Blacklisted
    {
        get => itemIcon.Blacklisted;
        set => itemIcon.Blacklisted = value;
    }

    public int OrderInUIList { get; set; }
    public Item Item { get; }

    public UIItemDropInfo(DropRateInfo info, bool locked = false, float textScale = 1f)
    {
        Item = new Item();
        Item.SetDefaults(info.itemId);
        SetDropConditionDescriptionOnItem(info);
        SetPadding(0f);
        PaddingLeft = 10f;
        PaddingRight = 10f;
        Width.Set(-14f, 1f);
        Height.Set(32f, 0f);
        Left.Set(5f, 0f);
        OnMouseOver += MouseOver;
        OnMouseOut += MouseOut;
        BorderColor = new Color(89, 116, 213, 255);
        info.GetInfoText(locked, out var stackRange, out var dropRate);

        if (locked)
        {
            hideMouseOver = true;
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("Images/UI/Bestiary/Icon_Locked", AssetRequestMode.ImmediateLoad);
            UIElement element = new()
            {
                Height = new StyleDimension(0f, 1f),
                Width = new StyleDimension(0f, 1f),
                HAlign = 0.5f,
                VAlign = 0.5f
            };

            element.SetPadding(0f);
            UIImage lockedImage = new(texture)
            {
                ImageScale = 0.55f,
                HAlign = 0.5f,
                VAlign = 0.5f
            };

            element.Append(lockedImage);
            Append(element);
        }
        else
        {
            itemIcon = new(Item)
            {
                IgnoresMouseInteraction = true,
                HAlign = 0f,
                Left = new StyleDimension(4f, 0f)
            };

            Append(itemIcon);

            if (!string.IsNullOrEmpty(stackRange))
                dropRate = stackRange + " " + dropRate;

            textPanel = new(dropRate, textScale)
            {
                IgnoresMouseInteraction = true,
                DrawPanel = false,
                HAlign = 1f,
                Top = new StyleDimension(-4f, 0f)
            };

            Append(textPanel);
        }
    }

    public bool ToggleBlacklisted()
    {
        return false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (IsMouseHovering && !hideMouseOver)
            DrawMouseOver();
    }

    private void DrawMouseOver()
    {
        Main.HoverItem = Item;
        Main.instance.MouseText("", 0, 0);
        Main.mouseText = true;
    }

    public override int CompareTo(object obj)
    {
        if (obj is IManuallyOrderedUIElement manuallyOrderedUIElement)
            return OrderInUIList.CompareTo(manuallyOrderedUIElement.OrderInUIList);

        return base.CompareTo(obj);
    }

    private void SetDropConditionDescriptionOnItem(DropRateInfo info)
    {
        List<string> list = new();
        if (info.conditions == null)
            return;

        foreach (IItemDropRuleCondition condition in info.conditions)
        {
            if (condition != null)
            {
                string conditionDescription = condition.GetConditionDescription();
                if (!string.IsNullOrWhiteSpace(conditionDescription))
                    list.Add(conditionDescription);
            }
        }

        Item.BestiaryNotes = string.Join("\n", list);
    }

    private void MouseOver(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
        BorderColor = Colors.FancyUIFatButtonMouseOver;
    }

    private void MouseOut(UIMouseEvent evt, UIElement listeningElement)
    {
        BorderColor = new Color(89, 116, 213, 255);
    }
}
