using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class CrescentScripture : ModItem
    {
        public override void SetStaticDefaults()
        {
            MoRHelper.AddElementToItem(Type, MoRHelper.Celestial, true);
        }
        public override void SetDefaults()
        {
            Item.damage = 390;
            Item.DamageType = DamageClass.Melee;
            Item.width = 84;
            Item.height = 84;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.shootSpeed = 25f;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarity3>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<CrescentScriptureProjectile>();
            Item.channel = true;

            MoRHelper.SetSlashBonus(Item);
        }

        public override bool AltFunctionUse(Player player) => false;
        public float SwingDirection { get; private set; } = -1;

        public override bool CanUseItem(Player player)
        {
            SwingDirection = -SwingDirection;
            return base.CanUseItem(player);
        }
    }
}