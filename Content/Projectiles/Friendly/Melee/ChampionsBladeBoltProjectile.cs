using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    internal class ChampionsBladeBoltProjectile : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.aiStyle = -1;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 80;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 999;

            trail = new MoonSwordTrail();
        }

        private ref float SkewMultiplier => ref Projectile.ai[0];
        private VertexTrail trail;
        public override void OnSpawn(IEntitySource source)
        {
            SkewMultiplier = (Main.rand.NextBool() ? 1f : -1f) * Main.rand.NextFloat(1.5f);
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(SkewMultiplier * 0.05f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var state = Main.spriteBatch.SaveState();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            trail?.Draw(Projectile, new Vector2(24 * Projectile.direction, 4).RotatedBy(Projectile.rotation) + Projectile.Size * 0.5f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            return false;
        }
    }
}
