using Macrocosm.Content.Projectiles.Friendly.Tools;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Fishing;

public class IndustrialFishingRod : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanFishInLava[Item.type] = true; // Allows the pole to fish in lava
    }

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 28;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 8;
        Item.useTime = 8;
        Item.UseSound = SoundID.Item1;

        Item.fishingPole = 55; //% fishing power 
        Item.shootSpeed = 18f; // Wooden Fishing Pole: 9f, Golden Fishing Rod: 17f.
        Item.shoot = ModContent.ProjectileType<IndustrialBobber>();
    }

    public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
    {
        lineOriginOffset = new Vector2(66, -36);

        if (bobber.type == Item.shoot)
            lineColor = new Color(206, 206, 206);
        else if (bobber.type == ProjectileID.FishingBobberGlowingRainbow)
            lineColor = Main.DiscoColor;
    }

    public override void AddRecipes()
    {
    }
}