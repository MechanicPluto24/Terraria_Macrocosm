﻿using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Debuffs.Weapons
{
    public class TotalityTag : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;

            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            DustEffects(player);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.dontTakeDamage)
                DustEffects(npc);
        }

        private void DustEffects(Entity entity)
        {
            int count = Math.Max(entity.width, entity.height) / 25;
            for (int i = 0; i < count; i++)
            {
                float rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                Color color = new List<Color>() {
                        new(44, 210, 91),
                        new(201, 125, 205),
                        new(114, 111, 207)
                    }.GetRandom();
                color.A = (byte)Main.rand.Next(120, 200);

                Vector2 position = entity.Center + Main.rand.NextVector2Circular(entity.width, entity.height) * 0.75f + entity.velocity * 8f;
                Particle.Create<TintableSlash>((p) =>
                {
                    p.Position = position;
                    p.Velocity = new Vector2(-0.4f * count, 0).RotatedBy((position - entity.Center).ToRotation());
                    p.Rotation = (position - entity.Center).ToRotation() - MathHelper.Pi;
                    p.Color = color;
                    p.SecondaryColor = (color * 2f).WithOpacity(0.2f);
                    p.FrameSpeed = 2;
                    p.Scale = new Vector2(0.3f, 0.4f) * Main.rand.NextFloat(0.2f, 0.7f);
                    p.ScaleVelocity = new Vector2(-0.01f, -0.01f);
                    p.FadeInNormalizedTime = 0.01f;
                    p.FadeOutNormalizedTime = 0.5f;
                });
            }
        }
    }
}
