using Macrocosm.Content.Dusts;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Artemite
{
    public class ArtemiteHammer : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 7;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.hammer = 115;
            Item.tileBoost = 5;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            #region Variables
            float lightMultiplier = 0.25f;
            #endregion

            #region Dust
            if (Main.rand.NextBool(4))
            {
                int swingDust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<ArtemiteDust>(), -35 * player.direction, default, default, default, Main.rand.NextFloat(1.25f, 1.35f));
                Main.dust[swingDust].velocity *= 0.05f;
            }
            #endregion

            #region Lighting
            Lighting.AddLight(player.position, 1 * lightMultiplier, 1 * lightMultiplier, 1 * lightMultiplier);
            #endregion
        }
    }
}