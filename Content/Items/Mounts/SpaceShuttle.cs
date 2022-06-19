using Terraria.ModLoader;
using Macrocosm.Content.Mounts;
using Terraria.ID;

namespace Macrocosm.Content.Items.Mounts
{
    public class SpaceShuttle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("In memory of STS-51-L and STS-107.");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 300;
            Item.rare = ItemRarityID.Pink;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<SpaceShuttleMount>();
        }


    }
}