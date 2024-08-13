using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class WaveGunSingle : ModItem
    {
        public override void SetStaticDefaults()
        {

 
        }
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 54;
            Item.height = 36;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WaveGunLaser>();
            Item.shootSpeed = 28f;
        }
    }
    public class WaveGunBlue : WaveGunSingle{
    public override string Texture => "Macrocosm/Content/Items/Weapons/Magic/WaveGunBlue";    
    }
    public class WaveGunRed : WaveGunSingle{
    public override string Texture => "Macrocosm/Content/Items/Weapons/Magic/WaveGunRed";    
    }
}
