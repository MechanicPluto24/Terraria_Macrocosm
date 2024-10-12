using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class LunarBlood : ModProjectile
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
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
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

            if (Projectile.ai[0] == 1f)//Luminite
            {
                color1 = new Color(94, 229, 163);
                color2 = new Color(213, 155, 148);
            }

            if (Projectile.ai[0] == 2f)//Heavanforge
            {
                color1 = new Color(89, 114, 141);
                color2 = new Color(114, 107, 122);
            }

            if (Projectile.ai[0] == 3f)//Lunar Rust
            {
                color1 = new Color(112, 242, 243);
                color2 = new Color(227, 136, 177);
            }

            if (Projectile.ai[0] == 4f)//Astra
            {
                color1 = new Color(132, 255, 221);
                color2 = new Color(60, 135, 238);
            }

            if (Projectile.ai[0] == 5f)//Dark Celestial
            {
                color1 = new Color(145, 253, 180);
                color2 = new Color(120, 87, 153);
            }

            if (Projectile.ai[0] == 6f)//Mercury
            {
                color1 = new Color(148, 187, 187);
                color2 = new Color(212, 209, 209);
            }

            if (Projectile.ai[0] == 7f)//Star royale
            {
                color1 = new Color(240, 198, 96);
                color2 = new Color(3, 129, 247);
            }

            if (Projectile.ai[0] == 8f)//Cryocore
            {
                color1 = new Color(121, 245, 231);
                color2 = new Color(144, 146, 221);
            }

            if (Projectile.ai[0] == 9f)//Cosmic Ember
            {
                color1 = new Color(255, 129, 97);
                color2 = new Color(127, 200, 155);
            }

            trail = new LunarBloodTrail { BloodColour1 = color1.WithAlpha(60), BloodColour2 = color2.WithAlpha(60) };
            Projectile.velocity.Y += 0.2f;
        }
    }
}