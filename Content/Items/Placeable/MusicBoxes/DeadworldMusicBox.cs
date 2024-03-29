using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.MusicBoxes
{
    public class DeadworldMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld"), ModContent.ItemType<DeadworldMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.DeadworldMusicBox>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<Tiles.MusicBoxes.DeadworldMusicBox>(), 0);
        }
    }
}
