using Macrocosm.Content.Dusts;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Artemite
{
    public class ArtemitePickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 6;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.pick = 235;
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