using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Items
{
    public class EnemyBannerItem : ModItem
    {
        private readonly string texture;
        private readonly string name;
        private readonly int createTile;
        private readonly int placeStyle;

        public override string Name => name;
        public override string Texture => texture;
        protected override bool CloneNewInstances => true;

        public EnemyBannerItem(string texture, string name, int createTile, int placeStyle = 0)
        {
            this.texture = texture;
            this.name = name;

            this.createTile = createTile;
            this.placeStyle = placeStyle;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(createTile, placeStyle);
            Item.width = 10;
            Item.height = 24;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
        }
    }
}
