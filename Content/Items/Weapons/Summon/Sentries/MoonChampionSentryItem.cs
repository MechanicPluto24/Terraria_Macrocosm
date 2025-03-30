using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Projectiles.Friendly.Summon.Sentries;

namespace Macrocosm.Content.Items.Weapons.Summon.Sentries
{
    public class MoonChampionSentryItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
           
            Item.DamageType = DamageClass.Summon;
            Item.damage = 300;
            Item.width = 96;
            Item.height = 96;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ModContent.RarityType<MoonRarity2>();
			Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.mana = 10;
			Item.UseSound = SoundID.Item1;

			Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<MoonChampionSentry>();
            
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {	
			position = Main.MouseWorld;
            if(!Collision.SolidTiles(position, 42, 46)){
                Projectile.NewProjectile(Item.GetSource_FromAI(), position,velocity,type,damage, 0f);
                player.UpdateMaxTurrets();
            }
			return false;
		}
        
    
    
    }
}