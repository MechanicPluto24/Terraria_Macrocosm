using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.MusicBoxes
{
    public class SpaceInvaderMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/SpaceInvader"), ModContent.ItemType<SpaceInvaderMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.SpaceInvaderMusicBox>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<Tiles.MusicBoxes.SpaceInvaderMusicBox>(), 0);
        }
    }
}
