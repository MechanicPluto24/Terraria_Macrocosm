using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Crafting;

public class Fabricator : ModTile
{
    private const int AnimationLineCount = 2;
    private const int AnimationLineFrameCount = 30;

    private const int AnimationFrameCount = AnimationLineFrameCount * AnimationLineCount;

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.Width = 3;
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
        TileObjectData.newTile.CoordinatePadding = 2;

        TileObjectData.newTile.StyleHorizontal = false;
        TileObjectData.newTile.StyleWrapLimit = AnimationLineFrameCount * 18 * 3;
        TileObjectData.newTile.StyleLineSkip = AnimationLineCount;

        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;
        DustType = -1;

        AddMapEntry(new Color(128, 128, 128), CreateMapEntryName());
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        int frame = Main.tileFrame[type];
        frameXOffset = 3 * 18 * (frame % AnimationLineFrameCount);
        frameYOffset = 3 * 18 * (frame / AnimationLineFrameCount);
    }

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        int ticksPerFrame = 5;
        int frameCount = AnimationFrameCount;

        // Add 1 second pause at the start of every animation line
        ticksPerFrame = (frame % AnimationLineFrameCount % 30 == 0) ? 60 : ticksPerFrame;

        if (++frameCounter >= ticksPerFrame)
        {
            frameCounter = 0;
            if (++frame >= frameCount)
                frame = 0;
        }
    }
}
