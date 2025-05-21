using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Macrocosm.Common.CrossMod;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class StardustWormPlayer : ModPlayer
    {
        public List<Projectile> StardustWorms = new();
    }
    public class StardustWormProjectile : ModProjectile
    {
        public override string Texture => "Macrocosm/Content/Projectiles/Friendly/Magic/StardustWormHead";
        public override void SetStaticDefaults()
        {
            MoRHelper.AddElementToProjectile(Type, MoRHelper.Arcane);
            MoRHelper.AddElementToProjectile(Type, MoRHelper.Celestial);
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;

            Projectile.width = 20;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        int segmentType=0;

        public override void OnKill(int timeLeft)
        { 
            Player player = Main.player[Projectile.owner];//uh so idk
            player.GetModPlayer<StardustWormPlayer>().StardustWorms.Remove(Projectile);
            foreach(Projectile p in player.GetModPlayer<StardustWormPlayer>().StardustWorms)
            {
                p.ai[0]-=1f;
            }
        }

        public override void AI()
        {
            //Determine the type
            int numberLessThanMe=0;
            Player player = Main.LocalPlayer;

            foreach(Projectile p in player.GetModPlayer<StardustWormPlayer>().StardustWorms)
            {
                if(p.ai[0]<Projectile.ai[0])
                    numberLessThanMe++;
            }
            if(numberLessThanMe==0)//Head
                segmentType=0;
            if(numberLessThanMe==player.GetModPlayer<StardustWormPlayer>().StardustWorms.Capacity)//tail
                segmentType=2;
            else//segment
                segmentType=1;
            
            if(player.GetModPlayer<StardustWormPlayer>().StardustWorms.Capacity==1)
            {
                Projectile.Kill();
            }
            
            if(segmentType==0)
            {
                Projectile.friendly = true;
                float distanceFromTarget = 700f;
                Vector2 targetCenter = player.position;
                bool foundTarget = false;
                if (!foundTarget)
                {
                    // This code is required either way, used for finding a target
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.CanBeChasedBy())
                        {
                            float between = Vector2.Distance(npc.Center, Projectile.Center);
                            bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                            bool inRange = between < distanceFromTarget;


                            if (((closest && inRange) || !foundTarget))
                            {
                                distanceFromTarget = between;
                                targetCenter = npc.Center;
                                foundTarget = true;
                            }
                        }
                    }
                }
                float speed = 30f;
                float inertia = 40f;
                Vector2 direction = targetCenter - Projectile.Center;
                direction.Normalize();
                direction *= speed;

                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
              
                Projectile following = player.GetModPlayer<StardustWormPlayer>().StardustWorms[(int)(Projectile.ai[0]-1f)];

                if (following is not null)
                {
                    // Follow behind the segment "in front" of this NPC
                    // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
                    float dirX = following.Center.X - Projectile.Center.X;
                    float dirY = following.Center.Y - Projectile.Center.Y;
                    // We then use Atan2 to get a correct rotation towards that parent NPC.
                    // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
                    Projectile.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;
                    // We also get the length of the direction vector.
                    float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                    // We calculate aw new, correct distance.
                    float dist = (length - Projectile.width) / length;
                    float posX = dirX * dist;
                    float posY = dirY * dist;

                    // Reset the velocity of this NPC, because we don't want it to move on its own
                    Projectile.velocity = Vector2.Zero;
                    // And set this NPCs position accordingly to that of this NPCs parent NPC.
                    Projectile.position.X += posX;
                    Projectile.position.Y += posY;
                    }
                }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            if(segmentType==1)
            {
                texture=ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/StardustWormBody").Value;
            }
            if(segmentType==2)
            {
                texture=ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/StardustWormTail").Value;
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White*Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White*0.5f*Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, (float)(Projectile.scale*((Math.Sin(Main.time*3f)*0.2f)+1.2f)), Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);

            return false;
        }
       
    }
}
