using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Keys
{
    public class XaocKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<Procellarum>(), TODO);
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<Ilmenite>(), TODO);
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<Micronova>(), TODO);
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<Totality>(), TODO);
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<ManisolBlades>(), TODO);
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<StarDestroyer>(), TODO);
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<FrigorianGaze>(), TODO);
            ShimmerSystem.RegisterOverride(Type, ModContent.ItemType<GreatstaffOfHorus>(), TODO);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ShadowKey);
            Item.width = 18;
            Item.height = 30;
            Item.value = 200;
            Item.rare = ModContent.RarityType<MoonRarity2>();
            Item.consumable = false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            Color colorFront = new(95, 152, 140, 15);
            Color colorBack = new(158, 255, 157, 20);

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;
            time %= 4f;
            time /= 2f;
            if (time >= 1f)
                time = 2f - time;
            time = time * 0.5f + 0.5f;

            for (float j = 0f; j < 1f; j += 0.25f)
            {
                float radians = (j + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 3f).RotatedBy(radians) * time, frame, colorFront, rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float f = 0f; f < 1f; f += 0.34f)
            {
                float radians = (f + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 1f).RotatedBy(radians) * time, frame, colorBack, rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }


            return true;
        }
    }
}
