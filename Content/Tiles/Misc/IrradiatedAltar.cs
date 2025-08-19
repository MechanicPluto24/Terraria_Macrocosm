using Macrocosm.Common.Players;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
namespace Macrocosm.Content.Tiles.Misc;


public class IrradiatedAltar : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
        TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
        TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;

        Main.tileNoAttach[Type] = true;
        DustType = ModContent.DustType<IrradiatedDust>();

        MinPick = 9999; // Very hard to break.
        Main.tileLighted[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.addTile(Type);
        Main.tileFrameImportant[Type] = true;

        AddMapEntry(new Color(50, 100, 50), CreateMapEntryName());
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Utility.GetEmmitedLight(i, j, new Color(128, 255, 128), applyPaint: true, out r, out g, out b);
    }

    public override void NumDust(int x, int y, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    public override void NearbyEffects(int i, int j, bool closer)
    {
        if (Main.gamePaused || closer)
            return;

        if (TileObjectData.IsTopLeft(i, j))
        {
            Player player = Main.LocalPlayer;
            float distance = Vector2.DistanceSquared(player.Center / 16, new Vector2(i, j));
            if (distance <= 20 * 20)
                player.GetModPlayer<IrradiationPlayer>().IrradiationLevel += 0.024f * (1f - distance / (20f * 20f));
        }
    }
}
