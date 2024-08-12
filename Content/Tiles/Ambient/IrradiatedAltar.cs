using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Ambient
{
    
    public class IrradiatedAltar : ModTile
    {
        public override void SetStaticDefaults()
        {
        
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
              
            Main.tileNoAttach[Type] = true;
            DustType = ModContent.DustType<XaocGreenDust>();
            
			MinPick = 999999999;//Very hard to break.
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.addTile(Type);
            Main.tileFrameImportant[Type] = true;
            

            AddMapEntry(new Color(50, 100, 50), Language.GetText("Nuclear Altar"));//TODO localize this.
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 1f;
            b = 0.5f;
        }
        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
		public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            int distance = (int)Vector2.Distance(player.Center/16, new Vector2(i, j));//Not MoR's player distance code whatsoever. :D
            if (distance<=20)
                player.GetModPlayer<IrradiationPlayer>().IrradiationLevels+=0.0006f;
        }
    }
}
