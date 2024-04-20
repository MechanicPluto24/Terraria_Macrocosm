using Macrocosm.Common.Bases.Items;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Bars;
using Macrocosm.Content.Items.Materials.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Tech
{
    public class FuelTank : ModItem, IFillableContainer
    {
        private Asset<Texture2D> fillTexture;

        public bool Full => fill >= 1;
        public bool Empty => fill <= 0;

        private float fill;
        public float Fill
        {
            get => MathHelper.Clamp(fill, 0, 1);
            set => fill = MathHelper.Clamp(value, 0, 1);
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 44;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.LightRed;
            Item.material = true;
            fill = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AluminumBar>(10)
                .AddIngredient<SteelBar>(5)
                .AddIngredient<Silicon>(8)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }

        public override bool CanStack(Item source)
        {
            FuelTank sourceTank = source.ModItem as FuelTank;
            return Empty && sourceTank.Empty || Full && sourceTank.Full;
        }

        public override void OnStack(Item source, int numToTransfer)
        {
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            fillTexture ??= ModContent.Request<Texture2D>(Texture + "_Fill");
            Rectangle sourceRect = GetSourceRect();
            Vector2 drawPosition = position + new Vector2(0f, sourceRect.Y) * scale;
            spriteBatch.Draw(fillTexture.Value, drawPosition, sourceRect, drawColor, 0f, fillTexture.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            fillTexture ??= ModContent.Request<Texture2D>(Texture + "_Fill");
            Rectangle sourceRect = GetSourceRect();
            Vector2 origin = fillTexture.Size() / 2f;
            Vector2 drawPosition = Item.position + origin + (new Vector2(-1f, sourceRect.Y) * scale).RotatedBy(rotation) - Main.screenPosition;
            spriteBatch.Draw(fillTexture.Value, drawPosition, sourceRect, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public Rectangle GetSourceRect()
        {
            int y = fill switch
            {
                <= 0f              => 36,
                >  0f   and < 0.2f => 34,
                >= 0.2f and < 0.6f => 30,
                >= 0.6f and < 0.8f => 26,
                >= 0.8f and < 1f   => 22,
                _                  => 18,
            };
            return new(0, y, fillTexture.Width(), fillTexture.Height());
        }
    }
}