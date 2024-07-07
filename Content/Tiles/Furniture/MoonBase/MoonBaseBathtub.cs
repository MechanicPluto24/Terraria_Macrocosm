using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.MoonBase
{
    public class MoonBaseBathtub : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.Bathtub"));

            DustType = ModContent.DustType<MoonBasePlatingDust>();
            AdjTiles = [TileID.Bathtubs];
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}