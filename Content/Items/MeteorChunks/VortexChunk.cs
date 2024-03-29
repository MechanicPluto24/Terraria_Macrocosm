using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.MeteorChunks
{
    public class VortexChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        override public void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Purple;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            int itemType = ItemID.FragmentVortex;
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), itemType, Main.rand.Next(20, 50));

        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.rand.NextBool(3))
            {
                int type = DustID.Vortex;
                Vector2 rotVector1 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * MathF.Abs(MathF.Sin((float)Main.timeForVisualEffects)) * 1.1f;
                Dust lightDust = Main.dust[Dust.NewDustPerfect(Item.Center - rotVector1, type).dustIndex];
                lightDust.noGravity = true;
                lightDust.position = Item.Center - rotVector1 * Main.rand.Next(10, 21);
                lightDust.velocity = rotVector1.RotatedBy(MathHelper.PiOver2) * 7f;
                lightDust.scale = 1.2f;
                lightDust.customData = Item.Center;
            }
        }
    }
}
