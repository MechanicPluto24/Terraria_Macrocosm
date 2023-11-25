using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.CelestialBulwark
{
    [AutoloadEquip(EquipType.Shield)]
    public class CelestialBulwark : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 0;
            Item.knockBack = 9f;
            Item.width = 34;
            Item.height = 40;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;

            Item.defense = 14;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => !((equippedItem.type == Type && incomingItem.type == ItemID.EoCShield) || (incomingItem.type == Type && equippedItem.type == ItemID.EoCShield));

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.AccDashHorizontal = true;
            dashPlayer.AccDashVelocity = 14f;
            dashPlayer.AccDashDamage = Item.damage;
            dashPlayer.AccDashKnockback = Item.knockBack;
            dashPlayer.AccDashImmuneTime = 6;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<BrokenHeroShield>());
            recipe.AddIngredient(ItemID.EoCShield);
            recipe.AddIngredient(ItemID.FragmentNebula, 15);
            recipe.AddIngredient(ItemID.FragmentStardust, 15);
            recipe.AddIngredient(ItemID.FragmentVortex, 15);
            recipe.AddIngredient(ItemID.FragmentSolar, 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawMask(spriteBatch, position, origin, scale);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            DrawMask(spriteBatch, Item.Center - Main.screenPosition, Item.Size / 2f, scale, rotation);
            Lighting.AddLight(Item.Center, CelestialDisco.CelestialColor.ToVector3());
        }

        private static Texture2D[] celestialTextures =
            {
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark/CelestialBulwark_Mask_Nebula").Value,
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark/CelestialBulwark_Mask_Stardust").Value,
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark/CelestialBulwark_Mask_Vortex").Value,
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark/CelestialBulwark_Mask_Solar").Value
            };


        private SpriteBatchState state;
        private void DrawMask(SpriteBatch spriteBatch, Vector2 position, Vector2 origin, float scale = 1f, float rotation = 0f)
        {
            Texture2D currentTex = celestialTextures[(int)CelestialDisco.CelestialStyle];
            Texture2D nextTex = celestialTextures[(int)CelestialDisco.NextCelestialStyle];
            Color currentColor = Color.White.WithOpacity(CelestialDisco.CelestialStyleProgress);
            Color nextColor = Color.White.WithOpacity(1f - CelestialDisco.CelestialStyleProgress);

            state.SaveState(spriteBatch);
            spriteBatch.EndIfBeginCalled();

            spriteBatch.Begin(BlendState.NonPremultiplied, state);

            spriteBatch.Draw(currentTex, position, null, nextColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(nextTex, position, null, currentColor, rotation, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}