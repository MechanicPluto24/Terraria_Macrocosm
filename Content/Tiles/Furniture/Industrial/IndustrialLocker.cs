﻿using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    [LegacyName("MoonBaseLocker")]
    public class IndustrialLocker : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());
        }
    }
}
