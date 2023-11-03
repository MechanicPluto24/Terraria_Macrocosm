using Macrocosm.Common.Bases;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class Procellarum : ModItem
    {
        //public override Vector2 SpriteHandlePosition => new(68, 108);
        //public override float HalberdWidth => 14.1f;
//        public override void SetStaticDefaults()
//        {
//            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
//        }
        public override void SetDefaults()
        {
            Item.damage = 450;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.useTime = 60;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Procellarum_HalberdProjectile>();
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 60;
            Item.shootSpeed = 1f;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[ModContent.ProjectileType<Procellarum_HalberdProjectile>()] < 1;
        }
    }
}
