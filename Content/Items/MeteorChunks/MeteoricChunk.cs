
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.MeteorChunks
{
    public class MeteoricChunk : ModItem
    {
        private int frameY = 0;
        private bool flip = false;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        override public void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 1);
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
                int itemType = Utils.SelectRandom(Main.rand, ModContent.ItemType<ArtemiteOre>(), ModContent.ItemType<ChandriumOre>(), ModContent.ItemType<SeleniteOre>(), ModContent.ItemType<DianiteOre>());
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), itemType, Main.rand.Next(1, 4));
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
            Draw(spriteBatch, frameY, flip, Item.Center - Main.screenPosition, lightColor, rotation, Item.Size / 2, scale);
            return false;
        }

        /// <summary> Draw with the default appearance in the inventory </summary>
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Draw(spriteBatch, 0, false, position + new Vector2(0, TextureAssets.Item[Type].Height() / 3f), drawColor, 0f, origin, scale * 4);
            return false;
        }

        private void Draw(SpriteBatch spriteBatch, int frameY, bool flip, Vector2 position, Color color, float rotation, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            SpriteEffects effects = flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle sourceRect = texture.Frame(1, 4, frameY: frameY);
            spriteBatch.Draw(texture, position, sourceRect, color, rotation, origin, scale, effects, 0f);
        }
    }
}
