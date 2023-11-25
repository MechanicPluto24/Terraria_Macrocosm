using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Global;
using Macrocosm.Content.Rockets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev
{
    class RocketPlacer : ModItem, IDevItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 38;
            Item.maxStack = 1;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item6;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.AltFunction())
                {
                    RocketManager.DespawnAllRockets();
                }
                else
                {
                    Rocket.Create(Main.MouseWorld - Rocket.Size / 2f);
                }
            }

            return true;
        }
    }
}
