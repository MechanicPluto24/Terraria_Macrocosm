using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class LuminiteRune : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        private float originalSpeed;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 280;
        }

        public ref float AI_Timer => ref Projectile.ai[0];

        bool spawned;
        float rotationSpeed;
        Color colour;
        private int targetNPC;
        private NPC TargetNPC => Main.npc[targetNPC];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!spawned)
            {
                rotationSpeed = 0.15f;
                Projectile.rotation += rotationSpeed;
                Projectile.frame = Main.rand.Next(0, 4);
                spawned = true;
                colour = Projectile.frame < 2 ? new Color(94, 229, 163, 255) : new Color(213, 155, 163, 148);
                Projectile.scale = 0.8f;
                originalSpeed = Projectile.velocity.Length();
            }

            AI_Timer++;

            Projectile.rotation += rotationSpeed;
            rotationSpeed *= 1f - 0.015f;
            Projectile.Opacity -= 0.001f;

            if (WorldGen.SolidTile(Projectile.Center.ToTileCoordinates()))
            {
                rotationSpeed *= 1f - 0.01f;
            }

            if (Projectile.Opacity < 0.01f)
            {
                Projectile.Kill();
            }

            if (AI_Timer > 40)
            {
                float closestDistance = 3000f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile))
                    {
                        float distance = Vector2.Distance(Projectile.Center, npc.Center);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            targetNPC = npc.whoAmI;
                        }
                    }
                }
            }

            if (TargetNPC is not null && Vector2.Distance(Projectile.Center, TargetNPC.Center) < 3000f && TargetNPC.CanBeChasedBy(Projectile))
            {
                Vector2 direction = (TargetNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * originalSpeed, 0.05f);
                Projectile.timeLeft--;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>(), Scale: Main.rand.NextFloat(0.5f, 1f));
                dust.velocity = Projectile.velocity.RotatedByRandom(MathHelper.Pi * 2) * Main.rand.NextFloat(0.5f, 1f);
                dust.noLight = false;
                dust.noGravity = true;
                if (Projectile.frame > 1)
                    dust.color = new Color(164, 101, 124);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255) * Projectile.Opacity;
        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            //Projectile.DrawMagicPixelTrail(Vector2.Zero,4f, 0.1f, colour * Projectile.Opacity, Color.Transparent);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);

            for (int i = 0; i < 8; i++)
            {
                Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    (colour * 0.2f * (1f - (i / 8f))).WithAlpha(0),
                    Projectile.rotation,
                    Projectile.Size / 2f,
                    Projectile.scale + (0.2f * i),
                    default,
                    0
                );
            }

            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float progress = (((float)i) / Projectile.oldPos.Length);
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Color color = (colour * (1f - progress) * 0.4f).WithAlpha(0);
                float scale = i > 1 ? Projectile.scale * 0.8f : Projectile.scale * 0.4f;
                Utility.DrawStar(drawPos, 1, color, scale, Projectile.velocity.ToRotation() - MathHelper.PiOver2, entity: true);
            }

            return true;
        }
    }
}