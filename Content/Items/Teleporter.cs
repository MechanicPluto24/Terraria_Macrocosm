using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Terraria.ID;
using Macrocosm.Content.Subworlds.Moon;

namespace Macrocosm.Content.Items
{
    class Teleporter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Teleports the user to and from the Moon");
        }
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.rare = ItemRarityID.Purple;
            Item.value = 100000;
            Item.maxStack = 1;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item6;
        }
        public override bool? UseItem(Player player)
        {
            if (!SubworldSystem.AnyActive<Macrocosm>())
            {
                SubworldSystem.Enter<Moon>();
            }
            else
            {
                SubworldSystem.Exit();
            }
            return true;
        }
    }
}
