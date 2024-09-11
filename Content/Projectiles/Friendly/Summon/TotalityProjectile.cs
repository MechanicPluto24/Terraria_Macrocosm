using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Weapons;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class TotalityProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 24;
            Projectile.WhipSettings.RangeMultiplier = 1.6f;
        }

        // AI timer for whip swing 
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public Vector2 WhipTipPosition;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // minions will attack the npcs hit with this whip 
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }

        private readonly int frameWidth = 18;
        private readonly int frameHeight = 26;

        private int[] animFrames;

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            //Utility.DrawWhipLine(list, new Color(60, 27, 120));

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, frameWidth, frameHeight);
                Vector2 origin = new(frameWidth / 2, frameHeight / 2);
                float scale = 1;

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                bool tip = i == list.Count - 2;
                bool handle = i == 0;

                if (tip)
                {
                    frame.Y = 6 * frameHeight;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.4f, 1.3f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));

                    WhipTipPosition = pos;


                    /*
                    // Depends on whip extenstion
                    float dustChance = Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true);

                    // Spawn dust
                    if (dustChance > 0.5f && Main.rand.NextFloat() < dustChance * 0.7f)
                    {
                        Vector2 outwardsVector = list[^2].DirectionTo(list[^1]).SafeNormalize(Vector2.Zero);
                        Dust dust = Dust.NewDustDirect(list[^1] - texture.Size() / 2, texture.Width, texture.Height, ModContent.DustType<ChandriumBrightDust>(), 0f, 0f, 100, default, Main.rand.NextFloat(1f, 1.5f));

                        dust.noGravity = true;
                        dust.velocity *= Main.rand.NextFloat() * 0.2f;
                        dust.velocity += outwardsVector * 0.2f;
                    }
                    */
                }
                else if(handle) 
                {
                    frame.Y = 0;
                }
                else
                {
                    animFrames ??= new int[list.Count];
                    if (Timer % 8 == 0)
                        animFrames[i] = Main.rand.Next(1, 5 + 1);

                    frame.Y = animFrames[i] * frameHeight;
                }

                if(!handle && !tip)
                    for(float f = 0f; f < 1f; f+= 0.5f)
                        Main.spriteBatch.DrawStar(Vector2.Lerp(list[i], list[i + 1], f) + Main.rand.NextVector2Circular(6, 6) - Main.screenPosition, 1, new Color(253, 174, 248, 225), scale * 0.4f, rotation, flip, entity: true);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }

            return false;
        }
    }
}
