using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Walls;
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

            //WorldGen.TryGrowingTreeByType(ModContent.TileType<RubberTree>(), Player.tileTargetX, Player.tileTargetY);

            //var data = TileObjectData.GetTileData(type, 0);

            //WorldData.DemonSun = !WorldData.DemonSun;

            bool message = true;
            foreach (var subworld in OrbitSubworld.GetOrbitSubworlds(Earth.ID))
            {
                bool value = WorldData.GetSubworldData(subworld.ID).Unlocked;
                WorldData.GetSubworldData(subworld.ID).Unlocked = !value;
                if (message)
                {
                    Main.NewText(value ? "Orbit locked" : "Orbit unlocked");
                    message = false;
                }
            }
            foreach (var subworld in OrbitSubworld.GetOrbitSubworlds(ModContent.GetInstance<Moon>().ID))
            {
                bool value = WorldData.GetSubworldData(subworld.ID).Unlocked;
                WorldData.GetSubworldData(subworld.ID).Unlocked = !value;
                if (message)
                {
                    Main.NewText(value ? "Orbit locked" : "Orbit unlocked");
                    message = false;
                }
            }

            return true;
        }
    }

}
