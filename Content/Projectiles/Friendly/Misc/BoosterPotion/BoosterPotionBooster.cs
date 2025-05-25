using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Debuffs.Radiation;

namespace Macrocosm.Content.Projectiles.Friendly.Misc.BoosterPotion
{
   public class BoosterPotionBooster : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
     
        bool spawned =false;

        public int BoostType
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.scale=1f;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.Opacity=0f;
        }

        public override void AI()
        {
            if(!spawned)
            {
                if(Projectile.owner == Main.myPlayer)
                {
                    spawned=true;
                    BoostType = Main.rand.Next(0,3);
                }
            }
            Player player = Main.player[Projectile.owner];
            int dist = 3;
            Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
            float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
            float radians = (Projectile.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>((p =>
                {
                    p.DustType = 43;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Color = (BoostType == 0 ? new Color(255,228,90) : (BoostType == 1 ? new Color(209,90,255) : new Color(107,125,255)));
                    p.Acceleration = velocity * 1f;
                    p.Scale = new(Main.rand.NextFloat(1f, 1.5f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                    p.NoLightEmittence = false;
                    p.lifeTime=300;
                }));
            if(Vector2.Distance(player.Center,Projectile.Center)<200f)
            {
                Projectile.velocity += (player.Center-Projectile.Center).SafeNormalize(Vector2.UnitX)*1f;
            }
            else
                Projectile.velocity*=0.95f;

            if(Vector2.Distance(player.Center,Projectile.Center)<30f)
            {
                if(BoostType==0)
                {
                    if(player.HasBuff(ModContent.BuffType<BoosterRegen1>())||player.HasBuff(ModContent.BuffType<BoosterRegen2>()))
                    {
                        player.ClearBuff(ModContent.BuffType<BoosterRegen1>());
                        player.AddBuff(ModContent.BuffType<BoosterRegen2>(),900);
                    }
                    else
                    {
                        player.AddBuff(ModContent.BuffType<BoosterRegen1>(),900);
                    }
                }
                if(BoostType==1)
                {
                    if(player.HasBuff(ModContent.BuffType<BoosterMana1>())||player.HasBuff(ModContent.BuffType<BoosterMana2>()))
                    {
                        player.ClearBuff(ModContent.BuffType<BoosterMana1>());
                        player.AddBuff(ModContent.BuffType<BoosterMana2>(),900);
                    }
                    else
                    {
                        player.AddBuff(ModContent.BuffType<BoosterMana1>(),900);
                    }
                }
                if(BoostType==2)
                {
                    if(player.HasBuff(ModContent.BuffType<BoosterDamage1>())||player.HasBuff(ModContent.BuffType<BoosterDamage2>()))
                    {
                        player.ClearBuff(ModContent.BuffType<BoosterDamage1>());
                        player.AddBuff(ModContent.BuffType<BoosterDamage2>(),900);
                    }
                    else
                    {
                        player.AddBuff(ModContent.BuffType<BoosterDamage1>(),900);
                    }
                }
                Projectile.Kill();
            }

        }
    }
    public class BoosterRegen1 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
			player.lifeRegen +=5;
       
            int dist = 200;
            Vector2 dustPosition = player.Center + new Vector2(dist,0).RotatedBy((float)((Main.time/100)+2.094f));
            float distFactor = (Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist));
            float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>((p =>
                {
                    
                    p.DustType = 43;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Color = (new Color(255,228,90));
                    p.Acceleration = velocity * 1f;
                    p.Scale = new(Main.rand.NextFloat(0.8f, 1f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                    p.NoLightEmittence = false;
                    p.lifeTime=300;

                }));
            
        }
    }
    public class BoosterRegen2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
			player.lifeRegen +=10;
         

            int dist = 200;
            Vector2 dustPosition = player.Center + new Vector2(dist,0).RotatedBy((float)((Main.time/100)+2.094f));
            float distFactor = (Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist));
            float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>((p =>
                {
                    p.DustType = 43;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Color = (new Color(255,228,90));
                    p.Acceleration = velocity * 1f;
                    p.Scale = new(Main.rand.NextFloat(1.8f, 2f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                    p.NoLightEmittence = false;
                    p.lifeTime=300;
                }));
            
        }
    }
    public class BoosterMana1 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
			player.manaRegen +=5;
     

            int dist = 200;
            Vector2 dustPosition = player.Center + new Vector2(dist,0).RotatedBy((float)((Main.time/100)+4.188f));
            float distFactor = (Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist));
            float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>((p =>
                {
                    p.DustType = 43;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Color = (new Color(209,90,255));
                    p.Acceleration = velocity * 1f;
                    p.Scale = new(Main.rand.NextFloat(0.8f, 1f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                    p.NoLightEmittence = false;
                    p.lifeTime=300;
                }));
            
        }
    }
    public class BoosterMana2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
			player.manaRegen +=10;
            

            int dist = 200;
            Vector2 dustPosition = player.Center + new Vector2(dist,0).RotatedBy((float)((Main.time/100)+4.188f));
            float distFactor = (Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist));
            float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>((p =>
                {
                    p.DustType = 43;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Color = (new Color(209,90,255));
                    p.Acceleration = velocity * 1f;
                    p.Scale = new(Main.rand.NextFloat(1.8f, 2f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                    p.NoLightEmittence = false;
                    p.lifeTime=300;
                }));
            
        }
    }
    public class BoosterDamage1 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
			player.GetDamage<GenericDamageClass>() *=1.05f;
            

            int dist = 200;
            Vector2 dustPosition = player.Center + new Vector2(dist,0).RotatedBy((float)((Main.time/100)));
            float distFactor = (Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist));
            float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>((p =>
                {
                    p.DustType = 43;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Color = (new Color(107,125,255));
                    p.Acceleration = velocity * 1f;
                    p.Scale = new(Main.rand.NextFloat(0.8f, 1f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                    p.NoLightEmittence = false;
                    p.lifeTime=300;
                }));
            
        }
    }
    public class BoosterDamage2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
			player.GetDamage<GenericDamageClass>() *=1.1f;
    

            int dist = 200;
            Vector2 dustPosition = player.Center + new Vector2(dist,0).RotatedBy((float)((Main.time/100)));
            float distFactor = (Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist));
            float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>((p =>
                {
                    p.DustType = 43;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Color = (new Color(107,125,255));
                    p.Acceleration = velocity * 1f;
                    p.Scale = new(Main.rand.NextFloat(1.8f, 2f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                    p.NoLightEmittence = false;
                    p.lifeTime=300;
                }));
            
        }
    }
}