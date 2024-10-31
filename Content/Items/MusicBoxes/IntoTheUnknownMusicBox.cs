using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.MusicBoxes
{
    public class IntoTheUnknownMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/IntoTheUnknown"), ModContent.ItemType<IntoTheUnknownMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.IntoTheUnknownMusicBox>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<Tiles.MusicBoxes.IntoTheUnknownMusicBox>(), 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DeadworldMusicBox>()
                .AddIngredient<RequiemMusicBox>()
                .AddIngredient<StygiaMusicBox>()
                .AddIngredient<SpaceInvaderMusicBox>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
