using Macrocosm.Content.Dusts;
using Macrocosm.Content.Tiles.Blocks;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Tombstones
{
    public class MoonTombstone : MacrocosmTombstone
    {
        public override int TileType => ModContent.TileType<Tiles.Tombstones.MoonTombstone>();

        public override int TargetRockTileType => ModContent.TileType<Regolith>();

        public override int ImpactDustType => ModContent.DustType<RegolithDust>();

        public override int StyleCount => 2;

    }
}
