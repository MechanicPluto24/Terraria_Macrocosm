using Macrocosm.Common.Global.Items;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev
{
    class DevOmnitool : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            ItemSets.DeveloperItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.UseSound = SoundID.Item6;
        }


        public override bool? UseItem(Player player)
        {
            Microsoft.Xna.Framework.Point point = Utility.GetClosestTile(player.Center, ModContent.TileType<Monolith>(), 10000);
            if (point != default)
                player.Teleport(point.ToWorldCoordinates());
            return true;
        }
    }

}
