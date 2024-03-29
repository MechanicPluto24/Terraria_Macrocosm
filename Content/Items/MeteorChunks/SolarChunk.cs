using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.MeteorChunks
{
    public class SolarChunk : ModItem
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
            int itemType = ItemID.FragmentSolar;
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), itemType, Main.rand.Next(20, 50));

        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.NextFromList<int>(DustID.Flare, DustID.SolarFlare, DustID.Torch);

                Dust dust = Dust.NewDustDirect(Item.position, Item.width, Item.height, dustType);
                dust.velocity.X = Main.rand.NextFloat(-1.2f, 1.2f);
                dust.velocity.Y = -0.8f;
                dust.scale = 1.4f;
                dust.noGravity = true;
            }
        }
    }
}
