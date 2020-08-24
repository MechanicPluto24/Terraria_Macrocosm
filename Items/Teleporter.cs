using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Macrocosm.Subworlds;
using Macrocosm;
using Terraria.ID;

namespace Macrocosm.Items
{
    class Teleporter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Teleports the user to and from the Moon");
        }
        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 36;
            item.rare = ItemRarityID.Purple;
            item.value = 100000;
            item.maxStack = 1;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item6;
        }
        public override bool UseItem(Player player)
        {
            if (!Subworld.AnyActive<Macrocosm>())
            {
                Subworld.Enter<Moon>();
            }
            else
            {
                Subworld.Exit();
            }
            return true;
        }
    }
}
