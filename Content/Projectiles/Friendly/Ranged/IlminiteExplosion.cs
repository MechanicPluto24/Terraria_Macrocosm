using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class IlmeniteExplosion : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public Color colour1 = new(188, 89, 134);
        public Color colour2 = new(33, 188, 190);

        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(50, 50);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.CritChance = 16;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Size *= 1f + (0.5f * Projectile.ai[0]);
        }

        SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            for (int i = 0; i < 20 - Projectile.timeLeft; i++)
            {
                Color NColor = Main.rand.NextBool() == true ? colour1 : colour2;
                Vector2 NPosition = Projectile.Center + Main.rand.NextVector2Circular(40 - Projectile.timeLeft * 2, 40 - Projectile.timeLeft) * 2;
                Lighting.AddLight(NPosition, NColor.ToVector3());
                Utility.DrawStar(NPosition - Main.screenPosition, Main.rand.Next(1, 4), NColor, Main.rand.NextFloat(Projectile.ai[0]), 0, entity: true);
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            return false;
        }
    }
}
