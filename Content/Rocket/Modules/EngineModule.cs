using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket.Modules
{
    public class EngineModule : RocketModule
    {
        public override void Draw(SpriteBatch spriteBatch, Color ambientColor)
        {
            // Draw the rear booster behind the engine module (no paintjobs applicable)
            Texture2D boosterRear = ModContent.Request<Texture2D>(TexturePath + "_BoosterRear").Value;
            spriteBatch.Draw(boosterRear, Position + new Vector2(0, 18), null, ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

            // Draw the engine module with the base logic
            base.Draw(spriteBatch, ambientColor);
        }
    }
}
