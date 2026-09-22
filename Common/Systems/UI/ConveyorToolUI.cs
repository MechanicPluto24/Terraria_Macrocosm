using Macrocosm.Common.Players;
using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Content.Items.Connectors;
using Macrocosm.Content.Projectiles.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Systems.UI;

public class ConveyorToolUI : ModSystem
{
    private static readonly string[] Icons = { "InletIcon", "RedChuteIcon", "OutletIcon", "BlueChuteIcon", "PipeWrenchIcon", "YellowChuteIcon", "HopperIcon", "GreenChuteIcon", "DropperIcon" };
    private static readonly string[] Labels = { "Inlet", "Red", "Outlet", "Blue", "Place", "Yellow", "Hopper", "Green", "Dropper" };
    private readonly Dictionary<string, Asset<Texture2D>> textures = new();
    private static bool open;
    private static bool consumeUse;
    private int heldType;
    private int focus = 4;
    private Vector2 position;
    public static bool CancelDrag { get; private set; }
    public static bool BlockUse => open || consumeUse || CancelDrag;

    public static bool IsBlocked(Player player) => player.dead || player.CCed || player.noItems || player.noBuilding
        || Main.playerInventory || Main.ingameOptionsWindow || Main.InGameUI.IsVisible || Main.drawingPlayerChat
        || player.talkNPC >= 0 || !Main.mouseItem.IsAir || UISystem.Active;

    public override void OnWorldUnload() { open = consumeUse = CancelDrag = false; heldType = 0; }
    public override void Unload() { textures.Clear(); open = consumeUse = CancelDrag = false; }

    public override void PostUpdateInput()
    {
        if (Main.dedServ || Main.gameMenu) return;
        Player player = Main.LocalPlayer;
        CancelDrag = false;
        bool use = PlayerInput.Triggers.Current.MouseLeft;
        if (!use) consumeUse = false;
        if (player.HeldItem.type != heldType)
        { open = false; CancelDrag = true; heldType = player.HeldItem.type; }
        if (player.HeldItem.ModItem is not MulticoloredPipeWrench tool || IsBlocked(player))
        { open = false; CancelDrag = true; return; }
        bool right = PlayerInput.Triggers.JustPressed.MouseRight;
        bool dragging = player.ownedProjectileCounts[ModContent.ProjectileType<ConveyorToolDrag>()] > 0;
        if (right)
        {
            consumeUse = true;
            PlayerInput.Triggers.Current.MouseRight = false;
            if (dragging) { CancelDrag = true; return; }
            open = !open;
            focus = 4;
            position = Main.MouseScreen / Main.UIScale;
        }
        if (!open) return;
        player.mouseInterface = true;
        Vector2 size = new(Main.screenWidth / Main.UIScale, Main.screenHeight / Main.UIScale);
        position = Vector2.Clamp(position, new Vector2(72), Vector2.Max(new Vector2(72), size - new Vector2(72)));
        if (PlayerInput.UsingGamepad)
        {
            var pressed = PlayerInput.Triggers.JustPressed;
            if (pressed.Inventory)
            {
                open = false;
                consumeUse = true;
                PlayerInput.Triggers.Current.Inventory = false;
                return;
            }
            int dx = pressed.Right ? 1 : pressed.Left ? -1 : 0;
            int dy = pressed.Down ? 1 : pressed.Up ? -1 : 0;
            if (dx != 0 || dy != 0)
            {
                int x = focus % 3, y = focus / 3;
                for (int i = 0; i < 3; i++)
                {
                    x = (x + dx + 3) % 3; y = (y + dy + 3) % 3;
                    int next = y * 3 + x;
                    if (Available(next, tool.IsTablet)) { focus = next; break; }
                }
            }
        }
        else focus = HitTest(tool.IsTablet);
        if (PlayerInput.Triggers.JustPressed.MouseLeft || PlayerInput.UsingGamepad && PlayerInput.Triggers.JustPressed.Jump)
        {
            consumeUse = true;
            if (focus < 0) open = false;
            else Select(player, tool.IsTablet, focus);
        }
        if (PlayerInput.UsingGamepad)
        {
            // Navigation and confirmation belong to the menu, not player movement.
            var current = PlayerInput.Triggers.Current;
            current.Up = current.Down = current.Left = current.Right = current.Jump = false;
        }
    }

    private static bool Available(int index, bool tablet) => tablet || index is 1 or 3 or 4 or 5 or 7;
    private Vector2 ButtonPosition(int index) => position + new Vector2(index % 3 - 1, index / 3 - 1) * 44;
    private int HitTest(bool tablet)
    {
        Vector2 mouse = Main.MouseScreen / Main.UIScale;
        for (int i = 0; i < 9; i++)
            if (Available(i, tablet) && Vector2.Distance(mouse, ButtonPosition(i)) < 21) return i;
        return -1;
    }
    private static int ColorBit(int index) => index switch { 1 => 1, 3 => 4, 5 => 8, 7 => 2, _ => 0 };
    private static ConveyorToolComponent Component(int index) => index switch
    { 0 => ConveyorToolComponent.Inlet, 2 => ConveyorToolComponent.Outlet, 6 => ConveyorToolComponent.Hopper, 8 => ConveyorToolComponent.Dropper, _ => ConveyorToolComponent.Pipes };

    private static void Select(Player player, bool tablet, int index)
    {
        var settings = player.GetModPlayer<ConveyorToolPlayer>();
        var state = tablet ? settings.Tablet : settings.Wrench;
        if (index == 4) state.Cutting = !state.Cutting;
        else if (ColorBit(index) != 0)
        {
            if (state.Component == ConveyorToolComponent.Pipes) state.Colors ^= (byte)ColorBit(index);
            else state.Component = ConveyorToolComponent.Pipes;
        }
        else state.Component = Component(index);
        if (tablet) settings.Tablet = state; else settings.Wrench = state;
    }

    private Texture2D Texture(string name)
    {
        if (!textures.TryGetValue(name, out var asset))
            textures[name] = asset = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/ConveyorTools/" + name);
        return asset.Value;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        if (index >= 0) layers.Insert(index, new LegacyGameInterfaceLayer("Macrocosm: Conveyor Tools", Draw, InterfaceScaleType.UI));
    }

    private bool Draw()
    {
        Player player = Main.LocalPlayer;
        if (player.HeldItem.ModItem is not MulticoloredPipeWrench tool || IsBlocked(player)) return true;
        var settings = player.GetModPlayer<ConveyorToolPlayer>();
        var state = tool.IsTablet ? settings.Tablet : settings.Wrench;
        for (int i = 0; i < 9; i++)
        {
            if (!Available(i, tool.IsTablet)) continue;
            bool selected = i == 4 || (ColorBit(i) != 0 ? state.Component == ConveyorToolComponent.Pipes && (state.Colors & ColorBit(i)) != 0 : state.Component == Component(i));
            if (!open && !selected) continue;
            string icon = i == 4 && state.Cutting ? "PipeCutterIcon" : Icons[i];
            Vector2 at = open ? ButtonPosition(i) : Main.MouseScreen / Main.UIScale + new Vector2(22 + i % 3 * 12, 24 + i / 3 * 12);
            if (open)
            {
                string background = i == 4 ? "PipeUICenter" : i is 0 or 6 ? "PipeUIin" : i is 2 or 8 ? "PipeUIout" : "PipeUI";
                if (state.Cutting) background += "Red";
                // The supplied in/out red backgrounds have no separate selected variant.
                if (focus == i && !(state.Cutting && i is 0 or 2 or 6 or 8)) background += "Selected";
                Texture2D bg = Texture(background);
                Main.spriteBatch.Draw(bg, at, null, selected ? Color.White : Color.Gray, 0, bg.Size() / 2, 1, SpriteEffects.None, 0);
            }
            Texture2D texture = Texture(icon);
            Main.spriteBatch.Draw(texture, at, null, selected ? Color.White : Color.White * 0.4f, 0, texture.Size() / 2, open ? 1 : 0.35f, SpriteEffects.None, 0);
        }
        if (open && focus >= 0)
            Main.instance.MouseText(Language.GetTextValue("Mods.Macrocosm.ConveyorTools." + (focus == 4 && state.Cutting ? "Cut" : Labels[focus])));
        return true;
    }
}
