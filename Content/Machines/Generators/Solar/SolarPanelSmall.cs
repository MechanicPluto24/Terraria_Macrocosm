using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines.Generators.Solar
{
    public class SolarPanelSmall : MachineTile
    {
        public override short Width => 3;
        public override short Height => 2;
        public override MachineTE MachineTE => ModContent.GetInstance<SolarPanelSmallTE>();

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = true;

            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, 1, 1);

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = -1;

            AddMapEntry(new Color(0, 52, 154), CreateMapEntryName());

            RegisterItemDrop(ModContent.ItemType<Items.Machines.Generators.Solar.SolarPanelSmall>(), 0, 1);
        }
    }
}
