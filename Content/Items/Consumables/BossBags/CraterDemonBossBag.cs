using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Items.Vanity.BossMasks;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.BossBags
{
    public class CraterDemonBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true; // This set is one that every boss bag should have, it, for example, lets our boss bag drop dev armor..
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CraterDemonMask>(), 7));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Moonstone>(), 1, 30, 60));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeliriumPlating>(), 1, 30, 90));

            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenHeroShield>()));

            itemLoot.Add(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<CalcicCane>(),
                ModContent.ItemType<Cruithne>(),
                ModContent.ItemType<ImbriumJewel>()
                /*, ModContent.ItemType<ChampionBlade>() */
                ));
        }

        // Below is code for the visuals
        public override Color? GetAlpha(Color lightColor)
            => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Color colorFront = new(31, 255, 106, 15);
            Color colorBack = new(158, 255, 157, 20);

            Item.DrawBossBagEffect(spriteBatch, colorFront, colorBack, rotation, scale);

            return true;
        }
    }
}
