using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Common.UI
{
    public class UIConnectors
    {
        const string Path = Macrocosm.TexturesPath + "UI/Connectors/";

        public static void DrawConnectorHorizontal(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, Color borderColor, out Rectangle leftEndpoint, out Rectangle rightEndpoint)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Path + "ConnectorHorizontal").Value;
            spriteBatch.Draw(texture, destinationRectangle, color);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorHorizontal_Border").Value, destinationRectangle, borderColor);

            int connectorCorrection = 10;
            int edgeSize = destinationRectangle.Height; 
            leftEndpoint = new Rectangle(destinationRectangle.X - connectorCorrection, destinationRectangle.Y, edgeSize, destinationRectangle.Height);
            rightEndpoint = new Rectangle(destinationRectangle.X + destinationRectangle.Width - edgeSize + connectorCorrection, destinationRectangle.Y, edgeSize, destinationRectangle.Height);
        }

        public static void DrawConnectorVertical(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, Color borderColor, out Rectangle topEndpoint, out Rectangle bottomEndpoint)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Path + "ConnectorVertical").Value;
            spriteBatch.Draw(texture, destinationRectangle, color);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorVertical_Border").Value, destinationRectangle, borderColor);

            int connectorCorrection = 10;
            int edgeSize = destinationRectangle.Width; 
            topEndpoint = new Rectangle(destinationRectangle.X, destinationRectangle.Y - connectorCorrection, destinationRectangle.Width, edgeSize);
            bottomEndpoint = new Rectangle(destinationRectangle.X, destinationRectangle.Y + destinationRectangle.Height - edgeSize + connectorCorrection, destinationRectangle.Width, edgeSize);
        }

        public static void DrawConnectorTJunction(SpriteBatch spriteBatch, Vector2 position, Color color, Color borderColor, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorT").Value, position, null, color, 0f, Vector2.Zero, 1f, spriteEffects, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorT_Border").Value, position, null, borderColor, 0f, Vector2.Zero, 1f, spriteEffects, 0);
        }

        public static void DrawConnectorLCorner(SpriteBatch spriteBatch, Vector2 position, Color color, Color borderColor, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorL").Value, position, null, color, 0f, Vector2.Zero, 1f, spriteEffects, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorL_Border").Value, position, null, borderColor, 0f, Vector2.Zero, 1f, spriteEffects, 0);
        }

        public static void DrawConnectorTJunction(SpriteBatch spriteBatch, Rectangle endpoint, Color color, Color borderColor, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorT").Value, new Rectangle(endpoint.X, endpoint.Y, endpoint.Width, endpoint.Height), null, color, 0, Vector2.Zero, spriteEffects, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorT_Border").Value, new Rectangle(endpoint.X, endpoint.Y, endpoint.Width, endpoint.Height), null, borderColor, 0, Vector2.Zero, spriteEffects, 0);
        }

        public static void DrawConnectorLCorner(SpriteBatch spriteBatch, Rectangle endpoint, Color color, Color borderColor, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorL").Value, new Rectangle(endpoint.X, endpoint.Y, endpoint.Width, endpoint.Height), null, color, 0, Vector2.Zero, spriteEffects, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Path + "ConnectorL_Border").Value, new Rectangle(endpoint.X, endpoint.Y, endpoint.Width, endpoint.Height), null, borderColor, 0, Vector2.Zero, spriteEffects, 0);
        }
    }
}
