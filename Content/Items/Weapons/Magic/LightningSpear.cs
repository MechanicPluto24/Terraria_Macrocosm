using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic;

public class LightningSpear : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        ItemID.Sets.Spears[Type] = true;

        Redemption.AddElement(Item, Redemption.ElementID.Thunder, true);
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 10;
        Item.rare = ModContent.RarityType<MoonRarity3>();
        Item.value = Item.sellPrice(gold: 10);
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 13;
        Item.useTime = 13;
        Item.UseSound = SoundID.Item71;
        Item.autoReuse = true;
        Item.damage = 400;
        Item.knockBack = 6.5f;
        Item.noUseGraphic = true;
        Item.DamageType = DamageClass.Magic;
        Item.noMelee = true;
        Item.shootSpeed = 1f;
        Item.shoot = ModContent.ProjectileType<LightningSpearProjectile>();
        Item.mana =15;
    }


    public override bool CanUseItem(Player player)
    {
        
        Item.useTime = 25;
        Item.useAnimation = 17;
        Item.useStyle = ItemUseStyleID.Swing;
        player.SetItemAltUseCooldown(Type, 25);
        return true;
    
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    { 
        Projectile.NewProjectile(source, position, velocity * 25, ModContent.ProjectileType<LightningSpearProjectile>(), damage, knockback, Main.myPlayer, ai0: 0f); 
        return false;
    }

    public override bool? UseItem(Player player)
    {
        if (!Main.dedServ && Item.UseSound.HasValue)
        {
            SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
        }
        return null;
    }

    public override void AddRecipes()
    {

    }
}
