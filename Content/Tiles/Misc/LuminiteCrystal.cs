using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Misc;

public abstract class LuminiteCrystal : ModTile
{
    // We want both tiles to use the same texture
    public override string Texture => this.GetNamespacePath().Replace(Name, nameof(LuminiteCrystal));

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileSpelunker[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileShine[Type] = 300;

        // TileObjectData for placement preview with directional anchors; can be removed if it causes issues
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
        TileObjectData.newTile.DrawYOffset = 2;

        // Top anchor
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
        TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
        TileObjectData.newAlternate.DrawYOffset = -2;
        TileObjectData.addAlternate(1);

        // Right wall anchor
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
        TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
        TileObjectData.newAlternate.DrawYOffset = 0;
        TileObjectData.newAlternate.DrawXOffset = 2;
        TileObjectData.addAlternate(2);

        // Left wall anchor
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
        TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
        TileObjectData.newAlternate.DrawYOffset = 0;
        TileObjectData.newAlternate.DrawXOffset = -2;
        TileObjectData.addAlternate(3);

        TileObjectData.addTile(Type);

        AddMapEntry(new Color(57, 242, 150), CreateMapEntryName());

        HitSound = SoundID.Item27;
        DustType = ModContent.DustType<LuminiteBrightDust>();
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 5;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = (float)Math.Abs(Math.Cos((i * j) + (Main.time / 200)) * 0.07f) * 0.3f;
        g = ((float)Math.Abs(Math.Cos(i + (Main.time / 150)) * 0.12f) * 0.5f) + 0.5f;
        b = ((float)Math.Abs(Math.Cos(j + (Main.time / 250)) * 0.03f) * 0.3f) + 0.4f;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        // Directional draw offset based on anchor direction (TileFrameY row)
        offsetY = (tileFrameY / 18) switch
        {
            0 => 2,   // bottom anchor
            1 => -2,  // top anchor
            _ => 0    // left/right wall anchors
        };
    }

    public override bool CanPlace(int i, int j)
    {
        Tile below = Main.tile[i, j + 1];
        Tile above = Main.tile[i, j - 1];
        Tile right = Main.tile[i + 1, j];
        Tile left = Main.tile[i - 1, j];
        return WorldGen.SolidTile(below) || WorldGen.SolidTile(above) || WorldGen.SolidTile(right) || WorldGen.SolidTile(left);
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        if (!CanPlace(i, j))
        {
            WorldGen.KillTile(i, j);
            return false;
        }

        Tile below = Main.tile[i, j + 1];
        Tile above = Main.tile[i, j - 1];
        Tile right = Main.tile[i + 1, j];
        Tile left = Main.tile[i - 1, j];

        if (WorldGen.SolidTile(below))
            Main.tile[i, j].TileFrameY = 0;
        else if (WorldGen.SolidTile(above))
            Main.tile[i, j].TileFrameY = 18;
        else if (WorldGen.SolidTile(right))
            Main.tile[i, j].TileFrameY = 36;
        else if (WorldGen.SolidTile(left))
            Main.tile[i, j].TileFrameY = 54;

        if (resetFrame)
            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(20) * 18);

        return false;
    }
}

// Natural tile placed during world generation, drops LunarCrystal
public class LuminiteCrystalNatural : LuminiteCrystal
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

        TileID.Sets.BreakableWhenPlacing[Type] = true;

        RegisterItemDrop(ModContent.ItemType<Items.Consumables.Throwable.LunarCrystal>());
    }
}

// Fake tile placed by the Rubblemaker, consumes and drops LunarCrystal
public class LuminiteCrystalFake : LuminiteCrystal
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

        FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Items.Consumables.Throwable.LunarCrystal>(), Type, 0..20);

        RegisterItemDrop(ModContent.ItemType<Items.Consumables.Throwable.LunarCrystal>());
    }
}