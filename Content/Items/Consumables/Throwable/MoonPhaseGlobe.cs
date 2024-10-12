using Macrocosm.Content.Projectiles.Friendly.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Throwable
{
    public class MoonPhaseGlobe : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToThrownWeapon(ModContent.ProjectileType<MoonPhaseGlobeProjectile>(), 20, 8f);
            Item.UseSound = SoundID.Item106;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ItemRarityID.Red;
        }
    }
}
