using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI;

// Adapted from Terraria.GameContent.UI.Elements.UIItemIcon
public class UICustomItemIcon : UIElement
{
    public Color Color = Color.White;

    public bool Blacklisted = false;
    public bool DisplayCrossMarkWhenBlacklisted = true;

    private Asset<Texture2D> crossmark;
    private readonly Item item;

    public UICustomItemIcon(Item item)
    {
        this.item = item;
        Width.Set(32f, 0f);
        Height.Set(32f, 0f);
    }

    public bool ToggleBlacklisted(Color? blacklistColor = null, Color? defaultColor = null)
    {
        Blacklisted = !Blacklisted;

        if (Blacklisted)
            Color = blacklistColor ?? Color.Gray;
        else
            Color = defaultColor ?? Color.White;

        return Blacklisted;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        ItemSlot.DrawItemIcon(screenPositionForItemCenter: GetDimensions().Center(), item: item, context: 31, spriteBatch: spriteBatch, scale: item.scale, sizeLimit: 32f, environmentColor: Color);

        if (Blacklisted && DisplayCrossMarkWhenBlacklisted)
        {
            crossmark ??= ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Symbols/CrossmarkRed");
            spriteBatch.Draw(crossmark.Value, GetDimensions().Center() + new Vector2(6), null, Color.White, 0f, crossmark.Size() / 2f, 0.6f, SpriteEffects.None, 0);
        }
    }
}
