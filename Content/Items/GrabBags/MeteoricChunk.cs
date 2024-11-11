﻿
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.GrabBags
{
    public class MeteoricChunk : ModItem
    {
        private static Asset<Texture2D> sheet;

        private int frameY = 0;
        private bool flip = false;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
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

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {

            for (int i = 0; i < Main.rand.Next(1, 5); i++)
            {
                int itemType = Utils.SelectRandom(Main.rand,
                    ModContent.ItemType<SeleniteOre>(),
                    ModContent.ItemType<ChandriumOre>(),
                    ModContent.ItemType<ArtemiteOre>(),
                    ModContent.ItemType<DianiteOre>(),
                    ModContent.ItemType<NickelOre>()
                );
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), itemType, Main.rand.Next(25, 45));
            }
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
            Draw(spriteBatch, frameY, flip, Item.Center - Main.screenPosition, lightColor, rotation, scale);
            return false;
        }

        /// <summary> Draw with the default appearance in the inventory </summary>
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Draw(spriteBatch, 0, false, position, drawColor, 0f, scale);
            return false;
        }

        private void Draw(SpriteBatch spriteBatch, int frameY, bool flip, Vector2 position, Color color, float rotation, float scale)
        {
            sheet ??= ModContent.Request<Texture2D>(Texture + "_Sheet");
            SpriteEffects effects = flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle sourceRect = sheet.Frame(1, 4, frameY: frameY);
            spriteBatch.Draw(sheet.Value, position, sourceRect, color, rotation, sourceRect.Size() / 2f, scale, effects, 0f);
        }
    }
}
