using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
	private readonly Item infoDisplayItem;
	private readonly bool hideMouseOver = false;

	private readonly UICustomItemIcon itemIcon;
	private readonly UITextPanel<string> textPanel;

    public int OrderInUIList { get; set; }

	public UIItemDropInfo(DropRateInfo info, bool locked = false, float textScale = 1f)
	{
		infoDisplayItem = new Item();
		infoDisplayItem.SetDefaults(info.itemId);
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
		GetDropInfo(info, locked, out var stackRange, out var droprate);

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
			itemIcon = new(infoDisplayItem) 
			{
				IgnoresMouseInteraction = true,
				HAlign = 0f,
				Left = new StyleDimension(4f, 0f)
			};

			Append(itemIcon);

			if (!string.IsNullOrEmpty(stackRange))
				droprate = stackRange + " " + droprate;

			textPanel = new(droprate, textScale) 
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
		bool blacklisted = itemIcon.ToggleBlacklisted();
        textPanel.TextColor = blacklisted ? Color.Gray : Color.White;
		return blacklisted;
    }

    protected void GetDropInfo(DropRateInfo dropRateInfo, bool locked, out string stackRange, out string droprate)
	{
		if (dropRateInfo.stackMin != dropRateInfo.stackMax)
			stackRange = $" ({dropRateInfo.stackMin}-{dropRateInfo.stackMax})";
		else if (dropRateInfo.stackMin == 1)
			stackRange = "";
		else
			stackRange = " (" + dropRateInfo.stackMin + ")";

		string originalFormat = "P";
		if (dropRateInfo.dropRate < 0.001)
			originalFormat = "P4";

		if (dropRateInfo.dropRate != 1f)
			droprate = Terraria.Utils.PrettifyPercentDisplay(dropRateInfo.dropRate, originalFormat);
		else
			droprate = "100%";

		if (locked) 
		{
			droprate = "???";
			stackRange = "";
		}
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);
		if (IsMouseHovering && !hideMouseOver)
			DrawMouseOver();
	}

	private void DrawMouseOver()
	{
		Main.HoverItem = infoDisplayItem;
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
		List<string> list = [];
		if (info.conditions == null)
			return;

		foreach (IItemDropRuleCondition condition in info.conditions) {
			if (condition != null) {
				string conditionDescription = condition.GetConditionDescription();
				if (!string.IsNullOrWhiteSpace(conditionDescription))
					list.Add(conditionDescription);
			}
		}

		infoDisplayItem.BestiaryNotes = string.Join("\n", list);
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
