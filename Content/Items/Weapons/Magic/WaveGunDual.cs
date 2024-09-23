using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class WaveGunDual : ModItem
    {
        private static Asset<Texture2D> rifleTexture;

        private static LocalizedText displayNameDual;
        private static LocalizedText displayNameRifle;
        public override LocalizedText DisplayName => displayNameDual;

        public override void Load()
        {
            rifleTexture = ModContent.Request<Texture2D>(Texture + "_Rifle");
            displayNameDual = Language.GetOrRegister(base.DisplayName.Key + "Dual", () => "Wave Gun (Dual)");
            displayNameRifle = Language.GetOrRegister(base.DisplayName.Key + "Rifle", () => "Wave Gun (Rifle)");
        }

        public bool RifleMode { get; private set; }

        public override void SetDefaults()
        {
            Item.damage = 258;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;

            Item.width = 54;
            Item.height = 36;

            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT2>();

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<WaveGunDualHeld>();
            Item.shootSpeed = 28f;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<WaveGunRifleHeld>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<WaveGunDualHeld>()] < 1;

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.AltFunction())
                mult = 0f;
        }

        public override bool? UseItem(Player player)
        {
            if (player.AltFunction())
            {
                Item.channel = false;
                Item.useTime = Item.useAnimation = 40;

                RifleMode = !RifleMode;

                SoundEngine.PlaySound(RifleMode ? SFX.WaveGunJoin : SFX.WaveGunSplit, player.position);
                Item.SetNameOverride(RifleMode ? displayNameRifle.Value : displayNameDual.Value);
            }
            else
            {
                Item.channel = true;

                if (RifleMode)
                {
                    Item.useTime = Item.useAnimation = 40;
                    Item.shootSpeed = 40f;
                    Item.knockBack = 4f;
                }
                else
                {
                    Item.useTime = Item.useAnimation = 14;
                    Item.shootSpeed = 28f;
                    Item.knockBack = 3f;
                }
            }

            return null;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int fireRate = (int)(Item.useTime * player.GetAttackSpeed(DamageClass.Magic));
            int timer = 0;

            if (player.AltFunction())
            {
                fireRate = int.MaxValue;
                timer = int.MaxValue / 2;
            }

            if (RifleMode)
                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<WaveGunRifleHeld>(), damage, knockback, player.whoAmI, ai0: fireRate, ai1: timer);
            else
                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<WaveGunDualHeld>(), damage, knockback, player.whoAmI, ai0: fireRate, ai1: timer, ai2: player.ItemUseCount(Type));

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (RifleMode)
            {
                spriteBatch.Draw(rifleTexture.Value, position - new Vector2(3, 0), null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (RifleMode)
            {
                spriteBatch.Draw(rifleTexture.Value, Item.position - Main.screenPosition, null, lightColor, rotation, rifleTexture.Size() / 2f, scale, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(RifleMode);
        }

        public override void NetReceive(BinaryReader reader)
        {
            RifleMode = reader.ReadBoolean();
        }

        public override void SaveData(TagCompound tag)
        {
            if (RifleMode)
                tag[nameof(RifleMode)] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            RifleMode = tag.ContainsKey(nameof(RifleMode));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<WaveGunBlue>(1)
            .AddIngredient<WaveGunRed>(1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}