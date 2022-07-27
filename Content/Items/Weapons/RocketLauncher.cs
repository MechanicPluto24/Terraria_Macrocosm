using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons {
    public class RocketLauncher : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Dovah's Rocket Launcher");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.damage = 150;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 26;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = 10000;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item11;
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {

            type = ProjectileID.RocketII; // placeholder 
        }
    }
}
