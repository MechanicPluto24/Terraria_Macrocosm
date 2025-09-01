using Macrocosm.Content.Projectiles.Friendly.Thrown;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Throwable
{
    public class LunarCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToThrownWeapon(ModContent.ProjectileType<LunarCrystalProjectile>(), 20, 8f);
            Item.UseSound = SoundID.Item1;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ModContent.RarityType<MoonRarity1>(); ;
        }
    }
}
