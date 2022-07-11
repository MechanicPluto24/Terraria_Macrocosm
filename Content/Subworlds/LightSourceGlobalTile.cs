using Terraria;
using SubworldLibrary;
using Terraria.ModLoader;
using Terraria.ID;

namespace Macrocosm.Content.Subworlds
{
    public class LightSourceGlobalTile : GlobalTile
    {

        /// <summary>
        /// Disables placed torches, campfires and other blocks with fire when placed on subworlds with atmospheres without oxygen 
        /// </summary>
        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                if (Main.tileWaterDeath[type] || type == TileID.Torches || type == TileID.Campfire) // TODO: make a list of tiles and tileframes(?) that appear to use fire 
                {
                    WorldGen.TryToggleLight(i, j, false, skipWires: false);
                }
            }

            return true;
        }


        public override void RightClick(int i, int j, int type)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                if (type == TileID.Campfire)
                {
                    WorldGen.TryToggleLight(i, j, false, skipWires: false);
                }
            }
        }

        public override void HitWire(int i, int j, int type)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                if (Main.tileWaterDeath[type] || type == TileID.Torches || type == TileID.Campfire)
                {
                    WorldGen.TryToggleLight(i, j, false, skipWires: false); // does not work with campfires 
                }
            }
        }

    }
}