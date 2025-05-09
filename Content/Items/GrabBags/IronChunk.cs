
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.GrabBags
{
    public class IronChunk : ModItem
    {
        private static Asset<Texture2D> sheet;

        private int frameY = 0;
        private bool flip = false;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.OpenableBag[Type] = true;
        }

        override public void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Purple;
        }

        public override void OnSpawn(IEntitySource source)
        {
            frameY = Main.rand.Next(3);
            flip = Main.rand.NextBool();
        }

        public override void RightClick(Player player)
        {
            (int itemType, int minAmount, int maxAmount)[] ores =
            {
                (ItemID.CopperOre, 40, 70),
                (ItemID.TinOre, 40, 70),
                (ModContent.ItemType<AluminumOre>(), 35, 65),
                (ItemID.IronOre, 30, 60),
                (ItemID.LeadOre, 30, 60),
                (ModContent.ItemType<LithiumOre>(), 25, 50),
                (ItemID.SilverOre, 20, 45),
                (ItemID.TungstenOre, 20, 45),
                (ItemID.GoldOre, 10, 30),
                (ItemID.PlatinumOre, 10, 30)
            };

            var (itemType, minAmount, maxAmount) = Utils.SelectRandom(Main.rand, ores);

            player.QuickSpawnItem(
                player.GetSource_OpenItem(Type),
                itemType,
                Main.rand.Next(minAmount, maxAmount)
            );
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(Item.position, Item.width, Item.height / 2, ModContent.DustType<SmokeDust>(), newColor: new Color(160, 160, 160, 180));
                dust.velocity.X = Main.rand.NextFloat(-0.2f, 0.2f);
                dust.velocity.Y = -0.4f;
                dust.noGravity = true;
            }
        }

        /// <summary> Draw with a random frame and flipping in the world </summary>
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            sheet ??= ModContent.Request<Texture2D>(Texture + "_Sheet");
            SpriteEffects effects = flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle sourceRect = sheet.Frame(1, 4, frameY: frameY);
            spriteBatch.Draw(sheet.Value, Item.Center - Main.screenPosition, sourceRect, Utility.Colorize(alphaColor, lightColor), rotation, sourceRect.Size() / 2f, scale, effects, 0f);
            return false;
        }

        /// <summary> Draw with the default appearance in the inventory </summary>
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => true;
    }
}
