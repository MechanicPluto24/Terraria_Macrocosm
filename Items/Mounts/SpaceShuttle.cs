using Terraria.ModLoader;

namespace Macrocosm.Items.Mounts
{
    public class SpaceShuttle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("In memory of STS-51-L and STS-107.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.value = 300;
            item.rare = 5;
            item.noMelee = true;
            item.mountType = mod.MountType("SpaceShuttle");
        }


    }
}