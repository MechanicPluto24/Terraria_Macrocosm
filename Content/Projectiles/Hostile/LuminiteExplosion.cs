using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class LuminiteExplosion : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public ref float AI_Timer => ref Projectile.ai[0];

        private int defWidth;
        private int defHeight;
        private int sizeScale;
        public override void SetDefaults()
        {
            sizeScale = 4;
            defWidth = defHeight = 4;
            Projectile.width = defWidth;
            Projectile.height = defHeight;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            AI_Timer++;
            Projectile.Resize((int)(defHeight * AI_Timer * sizeScale), (int)(defHeight * AI_Timer * sizeScale));
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare3").Value;
            float progress = Utility.BounceEaseInOut(MathHelper.Clamp(AI_Timer / 10f, 0f, 1f));

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, LuminiteSlime.EffectColor, 0f, texture.Size() / 2f, progress, SpriteEffects.None, 0f);
            texture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Star8").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(213, 155, 148, 80), 0f, texture.Size() / 2f, progress * 0.4f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}
