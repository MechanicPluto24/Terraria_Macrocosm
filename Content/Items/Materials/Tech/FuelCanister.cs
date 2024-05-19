using Macrocosm.Common.Sets.Items;
using Macrocosm.Common.Config;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Bars;
using Macrocosm.Content.Items.Materials.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Items.Materials.Tech
{
    public class FuelCanister : LiquidContainer
    {
        private Asset<Texture2D> fillTexture;

        private static LocalizedText CapacityTooltip;
        private static LocalizedText AmountTooltip;

        public override float Capacity => 100;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            CapacityTooltip = this.GetLocalization(nameof(CapacityTooltip), () => "Can hold up to {0} {1} of fuel");
            AmountTooltip = this.GetLocalization(nameof(AmountTooltip), () => "{0}/{1} {2}");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 44;
            Item.maxStack = 9999;
            Item.value = 100;
            Item.rare = ItemRarityID.LightRed;
            Item.material = true;
            Amount = 100;
        }

        public override bool CanStack(Item source)
        {
            LiquidContainer sourceContainer = source.ModItem as LiquidContainer;
            return Empty && sourceContainer.Empty || Full && sourceContainer.Full;
        }

        public override void OnStack(Item source, int numToTransfer)
        {
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if(MacrocosmConfig.Instance.UnitSystem == MacrocosmConfig.UnitSystemType.Metric)
            {
                float maxFuel = Capacity;
                float currentFuel = Amount;
                string unit = "liters"; // TODO: localize
                tooltips.Add(new TooltipLine(Mod, nameof(CapacityTooltip), CapacityTooltip.Format(maxFuel, unit)));
                tooltips.Add(new TooltipLine(Mod, nameof(AmountTooltip), AmountTooltip.Format($"{currentFuel:0.##}", maxFuel, unit)));
            }
            else if (MacrocosmConfig.Instance.UnitSystem == MacrocosmConfig.UnitSystemType.Imperial)
            {
                float maxFuel = Capacity / 3.785f;
                float currentFuel = Amount / 3.785f;
                string unit = "liters"; // TODO: localize
                tooltips.Add(new TooltipLine(Mod, nameof(CapacityTooltip), CapacityTooltip.Format(maxFuel, unit)));
                tooltips.Add(new TooltipLine(Mod, nameof(AmountTooltip), AmountTooltip.Format($"{currentFuel:0.##}", maxFuel, unit)));
            }
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

        private Rectangle GetSourceRect()
        {
            int y = Percent switch
            {
                <= 0f => 36,
                > 0f and < 0.2f => 34,
                >= 0.2f and < 0.6f => 30,
                >= 0.6f and < 0.8f => 26,
                >= 0.8f and < 1f => 22,
                _ => 18,
            };
            return new(0, y, fillTexture.Width(), fillTexture.Height());
        }
    }
}