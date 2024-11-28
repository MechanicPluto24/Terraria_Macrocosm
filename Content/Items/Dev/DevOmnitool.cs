using Macrocosm.Common.Sets;
using Macrocosm.Content.Rockets;
using Terraria;
using Terraria.Graphics.Effects;
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
            /*
            Microsoft.Xna.Framework.Point point = Utility.GetClosestTile(player.Center, ModContent.TileType<KyaniteNest>(), 10000);
            if (point != default)
                player.Teleport(point.ToWorldCoordinates());
            */

            /*
            if (Main.wallHouse[Main.tile[Player.tileTargetX, Player.tileTargetY].WallType])
                Main.NewText("Wall is safe");
            else
                Main.NewText("Wall is unsafe");
            */

            /*
            SkyManager.Instance["Macrocosm:MoonSky"].Deactivate();
            SkyManager.Instance["Macrocosm:MoonSky"].Activate(default);
            */

            /*
            foreach(var rocket in RocketManager.Rockets)
            {
                if (rocket.Bounds.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
                    rocket.Fuel = 0f;
            }
            */

            //Main.drunkWorld = !Main.drunkWorld;

            return true;
        }
    }

}
