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
    public class ChandriumWhipProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {

            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 29;
            Projectile.WhipSettings.RangeMultiplier = 1.9f;
        }

        // AI timer for whip swing 
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // flag that stores if an npcs has already been hit in the current swing 
        private bool HitNPC
        {
            get => Projectile.ai[1] != 0f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public Vector2 WhipTipPosition;

        private ref int HitStacks => ref Main.player[Projectile.owner].GetModPlayer<MacrocosmPlayer>().ChandriumWhipStacks;

        // Extra AI data used for the on-hit effects 
        private bool onHitEffect = false;
        private bool empoweredHit = false;
        private int hitNpcId = 0;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // increase count only once per whip swing 
            if (!HitNPC && HitStacks < 3)
            {
                HitStacks++;   // this is a ref to a ModPlayer
                HitNPC = true; // set hit flag to true so stacks won't increase on every npc hit 
            }

            onHitEffect = true;
            hitNpcId = target.whoAmI;

            // if empowered hit is ready, ignore hit flag  
            if (HitStacks >= 3)
            {
                // increase damage 
                modifiers.FinalDamage *= 1.4f;

                // clear buff on successful hit 
                Main.player[Projectile.owner].ClearBuff(ModContent.BuffType<ChandriumWhipBuff>());

                empoweredHit = true;
            }
            else
            {
                empoweredHit = false;
            }

            Projectile.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(onHitEffect);
            writer.Write(empoweredHit);
            writer.Write(hitNpcId);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            onHitEffect = reader.ReadBoolean();
            empoweredHit = reader.ReadBoolean();
            hitNpcId = reader.ReadInt32();
        }

        /// <summary> Spawn dusts on hit, called on the owner client </summary>
        public void HitEffect(Entity target, bool empowered, bool update = false)
        {
            if (empowered)
            {
                for (int i = 0; i < 60; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(2.5f, 2.5f);
                    Dust dust;

                    if (i % 20 == 0)
                        Particle.Create<ChandriumCrescentMoon>(target.position, velocity, scale: new(Main.rand.NextFloat(0.5f, 0.9f)));

                    dust = Dust.NewDustDirect(target.position, target.width, target.height, ModContent.DustType<ChandriumBrightDust>(), velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    dust.noGravity = true;
                }
            }
            else
            {
                // fewer regular dust on non-empowered hit (regardless of hit flag) 
                for (int i = 0; i < 5; i++)
                {
                    Vector2 position = target.position;
                    Vector2 velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
                    Dust dust = Dust.NewDustDirect(position, target.width, target.height, ModContent.DustType<ChandriumBrightDust>(), velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    dust.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // minions will attack the npcs hit with this whip 
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // on spawn reset the hit npc flag 
            HitNPC = false;
        }

        public override void AI()
        {
            if (onHitEffect)
            {
                HitEffect(Main.npc[hitNpcId], empoweredHit);
                onHitEffect = false;
            }

            if (Main.player[Projectile.owner].HasBuff(ModContent.BuffType<ChandriumWhipBuff>()))
            {
                Lighting.AddLight(WhipTipPosition, new Vector3(0.607f, 0.258f, 0.847f));
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && HitNPC)
            {
                if (HitNPC)
                {
                    // reset stacks on end of successful empowered hit 
                    if (HitStacks >= 3)
                    {
                        HitStacks = 0;
                    }

                    if (HitStacks == 2)
                    {
                        Main.player[Projectile.owner].AddBuff(ModContent.BuffType<ChandriumWhipBuff>(), 60 * 5);
                    }
                }
            }
        }

        private readonly int frameWidth = 14;
        private readonly int frameHeight = 26;

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            Utility.DrawWhipLine(list, new Color(60, 27, 120));

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            bool empowered = Main.player[Projectile.owner].HasBuff(ModContent.BuffType<ChandriumWhipBuff>());

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

                if (tip)
                {
                    frame.Y = 4 * frameHeight;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.4f, 1.3f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));

                    if (empowered)
                        Utility.DrawStar(Vector2.Lerp(list[^1], list[^2], 0.8f) - Main.screenPosition, 1, new Color(177, 107, 219, 0), scale * 0.7f, rotation, flip, entity: true);

                    WhipTipPosition = pos;

                    // Depends on whip extenstion
                    float dustChance = Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true);

                    // Spawn dust
                    if (dustChance > 0.5f && Main.rand.NextFloat() < dustChance * 0.7f)
                    {
                        Vector2 outwardsVector = list[^2].DirectionTo(list[^1]).SafeNormalize(Vector2.Zero);

                        Dust dust = Dust.NewDustDirect(list[^1] - texture.Size() / 2, texture.Width, texture.Height, ModContent.DustType<ChandriumDust>(), 0f, 0f, 100, default, Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                        dust.velocity *= Main.rand.NextFloat() * 0.8f;
                        dust.velocity += outwardsVector * 0.1f;

                        if (empowered)
                        {
                            Dust dust2 = Dust.NewDustDirect(list[^1] - texture.Size() / 2, texture.Width, texture.Height, ModContent.DustType<ChandriumBrightDust>(), 0f, 0f, 100, default, Main.rand.NextFloat(1f, 1.5f));
                            dust2.noGravity = true;
                            dust2.velocity *= Main.rand.NextFloat() * 0.8f;
                            dust2.velocity += outwardsVector * 0.1f;
                        }
                    }
                }
                else if (i >= 19)
                {
                    frame.Y = 3 * frameHeight;
                }
                else if (i >= 10)
                {
                    frame.Y = 2 * frameHeight;
                }
                else if (i >= 1)
                {
                    frame.Y = frameHeight;
                }
                else
                {
                    frame.Y = 0;
                }

                if (Main.player[Projectile.owner].HasBuff(ModContent.BuffType<ChandriumWhipBuff>()) && !tip)
                    Utility.DrawStar(pos - Main.screenPosition, 1, new Color(177, 107, 219, 20), scale * 0.4f, rotation, flip, entity: true);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }

            return false;
        }
    }
}
