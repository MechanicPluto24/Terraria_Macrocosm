using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Environment.Sands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Sands;

public class SilicaSand : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSand[Type] = true;

        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        // Merge with most blocks
        Main.tileBrick[Type] = true;

        // Merge with dirt without blend frames
        Main.tileMergeDirt[Type] = false;
        Main.tileMerge[TileID.Dirt][Type] = true;
        Main.tileMerge[Type][TileID.Dirt] = true;

        TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
        TileID.Sets.CanBeDugByShovel[Type] = true;
        TileID.Sets.Falling[Type] = true;
        TileID.Sets.Suffocate[Type] = true;
        TileID.Sets.FallingBlockProjectile[Type] = new(ModContent.ProjectileType<SilicaSandFalling>(), FallingProjectileDamage: 20);

        TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Sand"]);

        MineResist = 0.5f;
        DustType = ModContent.DustType<SilicaSandDust>();

        AddMapEntry(new Color(199, 197, 171));
    }

    public override void Convert(int i, int j, int conversionType)
    {
        switch (conversionType)
        {
            case BiomeConversionID.Corruption:
                WorldGen.ConvertTile(i, j, ModContent.TileType<SilicaEbonsand>());
                return;

            case BiomeConversionID.Crimson:
                WorldGen.ConvertTile(i, j, ModContent.TileType<SilicaCrimsand>());
                return;

            case BiomeConversionID.Hallow:
                WorldGen.ConvertTile(i, j, ModContent.TileType<SilicaPearlsand>());
                return;
        }
    }

    public override bool HasWalkDust() => true;

    public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
    {
        dustType = ModContent.DustType<SilicaSandDust>();
    }
}