using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Tombstones;

public class MoonGoldTombstone : TombstoneProjectile
{
    public override int TileType => ModContent.TileType<Tiles.Tombstones.MoonGoldTombstone>();

    public override int TargetRockTileType => ModContent.TileType<Regolith>();

    public override int ImpactDustType => ModContent.DustType<RegolithDust>();

    public override int StyleCount => 3;
}
