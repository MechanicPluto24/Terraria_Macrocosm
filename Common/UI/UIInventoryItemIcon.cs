using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI;

// Adapted from Terraria.GameContent.UI.Elements.UIItemIcon
public class UIInventoryItemIcon : UIElement
{
    private static Asset<Texture2D> crossmark;

    public Color Color = Color.White;

    public bool Blacklisted = false;
    public bool DisplayCrossMarkWhenBlacklisted = true;

    public Item Item { get; set; }

    public UIInventoryItemIcon(Item item = null)
    {
        item ??= new();
        Item = item;
        Width.Set(32f, 0f);
        Height.Set(32f, 0f);
    }

    public bool ToggleBlacklisted(Color? blacklistColor = null, Color? defaultColor = null)
    {
        return false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Color = Blacklisted ? Color.Gray : Color.White;
        ItemSlot.DrawItemIcon(screenPositionForItemCenter: GetDimensions().Center(), item: Item, context: 31, spriteBatch: spriteBatch, scale: Item.scale, sizeLimit: 32f, environmentColor: Color);

        if (Blacklisted && DisplayCrossMarkWhenBlacklisted)
        {
            crossmark ??= ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CrossmarkRed");
            spriteBatch.Draw(crossmark.Value, GetDimensions().Center() + new Vector2(6), null, Color.White, 0f, crossmark.Size() / 2f, 0.6f, SpriteEffects.None, 0);
        }
    }
}
