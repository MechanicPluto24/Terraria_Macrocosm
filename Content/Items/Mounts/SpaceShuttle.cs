using Macrocosm.Content.Mounts;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Mounts
{
    public class SpaceShuttle : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 300;
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<SpaceShuttleMount>();
        }


    }
}