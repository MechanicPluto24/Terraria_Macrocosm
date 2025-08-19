using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines.Generators.Solar;

public class SolarPanelLarge : MachineTile
{
    public override short Width => 6;
    public override short Height => 4;
    public override MachineTE MachineTE => ModContent.GetInstance<SolarPanelLargeTE>();

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;

        TileObjectData.newTile.DefaultToMachine(this);
        TileObjectData.newTile.Origin = new Point16(3, 2);
        TileObjectData.newTile.StyleHorizontal = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, 2, 2);
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;
        DustType = -1;

        AddMapEntry(new Color(0, 52, 154), CreateMapEntryName());

        RegisterItemDrop(ModContent.ItemType<Items.Machines.Generators.Solar.SolarPanelLarge>(), 0, 1);
    }
}
