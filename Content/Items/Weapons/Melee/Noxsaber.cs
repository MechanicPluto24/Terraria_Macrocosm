using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class Noxsaber : ModItem
    {
        private static Asset<Texture2D> glow;

        public override void Load()
        {
            glow = ModContent.Request<Texture2D>(Texture + "_Glow");
        }

        public override void SetStaticDefaults()
        {
            ItemSets.UnobtainableItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 777;
            Item.DamageType = DamageClass.Melee;
            Item.width = 100;
            Item.height = 100;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true; // Lets you use the item without clicking the mouse repeatedly (i.e. swinging swords)
            Item.Glowmask().Texture = glow;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            //if (Main.rand.NextBool(2))
            //{
            //	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<CrucibleDust>());
            //}
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.85f * Main.essScale);
        }

        public override void AddRecipes()
        {
        }
    }
}