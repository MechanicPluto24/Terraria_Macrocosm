using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class HellBlood : ModProjectile
    {
        private LunarBloodTrail trail;
        private Color color1;
        private Color color2;

        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        private bool spawned;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile=true;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            trail?.Draw(Projectile, Projectile.Size / 2f);

            return false;
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
            }

            switch ((LuminiteStyle)Projectile.ai[0])
            {
                case LuminiteStyle.Luminite:
                    color1 = new Color(255, 0, 20);
                    color2 = new Color(150, 40, 50);
                    break;

            }

            trail = new LunarBloodTrail { BloodColour1 = color1.WithAlpha(60), BloodColour2 = color2.WithAlpha(120)};
            Projectile.velocity.Y += 0.2f;
        }
    }
}