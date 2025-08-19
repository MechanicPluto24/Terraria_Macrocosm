using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee;

public class Crucible : ModItem
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
        Item.damage = 666;
        Item.DamageType = DamageClass.Melee;
        Item.width = 76;
        Item.height = 76;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing; // 1 = sword
        Item.knockBack = 6f;
        Item.value = 10000;
        Item.rare = ModContent.RarityType<MoonRarity3>();
        Item.UseSound = SoundID.Item20;
        Item.autoReuse = true; // Lets you use the item without clicking the mouse repeatedly (i.e. swinging swords)
        Item.CustomDrawData().Glowmask = glow;
    }
    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        if (Main.rand.NextBool(2))
        {
            int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<CrucibleDust>());
        }
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.85f * Main.essScale);
    }

    public override void AddRecipes()
    {
    }
}