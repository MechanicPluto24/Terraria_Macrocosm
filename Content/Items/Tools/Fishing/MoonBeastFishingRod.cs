using Macrocosm.Content.Projectiles.Friendly.Tools;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Fishing;

public class MoonBeastFishingRod : ModItem
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

        Item.fishingPole = 75; //% fishing power 
        Item.shootSpeed = 22f; // Wooden Fishing Pole: 9f, Golden Fishing Rod: 17f.
        Item.shoot = ModContent.ProjectileType<MoonBeastBobber>();
    }

    // Fire multiple bobbers.
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int bobberAmount = 3;
        float spreadAmount = 100f; // how much the different bobbers are spread out.
        for (int index = 0; index < bobberAmount; ++index)
        {
            Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);
            Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
        }
        return false;
    }

    public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
    {
        lineOriginOffset = new Vector2(60, -36);
        if (bobber.type == Item.shoot)
            lineColor = new Color(208, 60, 86);
        else if (bobber.type == ProjectileID.FishingBobberGlowingRainbow)
            lineColor = Main.DiscoColor;
    }

    public override void AddRecipes()
    {
    }
}