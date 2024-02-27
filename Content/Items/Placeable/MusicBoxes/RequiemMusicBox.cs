using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.MusicBoxes
{
    public class RequiemMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/Requiem"), ModContent.ItemType<RequiemMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.RequiemMusicBox>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<Tiles.MusicBoxes.RequiemMusicBox>(), 0);
        }
    }
}
