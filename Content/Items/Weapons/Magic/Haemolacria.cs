
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic;

public class Haemolacria : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 300;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 20;
        Item.width = 28;
        Item.height = 26;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 8;
        Item.value = 50000;
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.UseSound = SoundID.Item20;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<HaemoBall>();
        Item.shootSpeed = 11f;
        Item.tileBoost = 50;
    }


    public override void AddRecipes()
    {
    }

    /*
    public override bool ModifyItemDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, ref DrawData? coloredDrawData, ref DrawData? glowMaskDrawData)
    {
        drawData.texture = ModContent.Request<Texture2D>(Texture + "_Held").Value;
        return true;
    }
    */

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Item").Value, position - new Vector2(2, 0), null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
        return false;
    }
}
