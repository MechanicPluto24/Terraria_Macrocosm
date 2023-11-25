using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Macrocosm.Common.UI
{
    public class UIConnectors
    {
        const string Path = Macrocosm.TextureAssetsPath + "UI/Connectors/";

        public static void DrawConnectorHorizontal(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, Color borderColor)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorHorizontal").Value, destinationRectangle, color);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorHorizontal_Border").Value, destinationRectangle, borderColor);
        }

        public static void DrawConnectorVertical(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, Color borderColor)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorVertical").Value, destinationRectangle, color);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorVertical_Border").Value, destinationRectangle, borderColor);
        }

        public static void DrawConnectorTJunction(SpriteBatch spriteBatch, Vector2 position, Color color, Color borderColor)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorT").Value, position, color);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorT_Border").Value, position, borderColor);
        }

        public static void DrawConnectorLCorner(SpriteBatch spriteBatch, Vector2 position, Color color, Color borderColor)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorL").Value, position, color);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorL_Border").Value, position, borderColor);
        }
    }
}
