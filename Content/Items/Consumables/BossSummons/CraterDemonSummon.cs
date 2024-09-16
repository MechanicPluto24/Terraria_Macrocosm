using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Items.Drops;
namespace Macrocosm.Content.Items.Consumables.BossSummons
{
    public class CraterDemonSummon : ModItem
    {
        // Different sprite for drawing in world and in inventory, as opposed from the player-held variant
        private static Asset<Texture2D> itemSprite;

        public override void Load()
        {
            itemSprite = ModContent.Request<Texture2D>(Texture + "_Item");
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 18;
            Item.scale = 1f;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
            => player.InModBiome<MoonBiome>() && !NPC.AnyNPCs(ModContent.NPCType<CraterDemon>());

        public override bool? UseItem(Player player)
        {
            if (Utility.SummonBossDirectlyWithMessage(player.Center - new Vector2(0f, 240f), ModContent.NPCType<CraterDemon>()))
                SoundEngine.PlaySound(SoundID.ForceRoar, player.position);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AlienResidue>(7)
                .AddIngredient<SpaceDust>(5)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(ModContent.TileType<IrradiatedAltar>())
                .Register();
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(itemSprite.Value, position, new Rectangle(0, 0, itemSprite.Width(), itemSprite.Height()), drawColor, 0f, itemSprite.Size() / 2f, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(itemSprite.Value, Item.position - Main.screenPosition, null, alphaColor, rotation, itemSprite.Size() / 2f, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
