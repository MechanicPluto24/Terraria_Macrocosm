using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Pets;

public class CraterDemonPet : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
        Main.projPet[Projectile.type] = true;

        // This code is needed to customize the vanity pet display in the player select screen. Quick explanation:
        // * It uses fluent API syntax, just like Recipe
        // * You start with ProjectileID.Sets.SimpleLoop, specifying the start and end frames as well as the speed, and optionally if it should animate from the end after reaching the end, effectively "bouncing"
        // * To stop the animation if the player is not highlighted/is standing, as done by most grounded pets, add a .WhenNotSelected(0, 0) (you can customize it just like SimpleLoop)
        // * To set offset and direction, use .WithOffset(x, y) and .WithSpriteDirection(-1)
        // * To further customize the behavior and animation of the pet (as its AI does not run), you have access to a few vanilla presets in DelegateMethods.CharacterPreview to use via .WithCode(). You can also make your own, showcased in MinionBossPetProjectile
        ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 6)
            .WithOffset(-10, -20f)
            .WithSpriteDirection(-1)
            .WithCode(DelegateMethods.CharacterPreview.Float);
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.ZephyrFish);
        AIType = ProjectileID.ZephyrFish;
    }

    public override bool PreAI()
    {
        Player player = Main.player[Projectile.owner];
        player.zephyrfish = false; // Relic from AIType
        return true;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        Projectile.frame = MathF.Sign(Projectile.velocity.X) > 0 ? 1 : 0;

        if (!player.dead && player.HasBuff(ModContent.BuffType<Content.Buffs.Pets.CraterDemonPet>()))
            Projectile.timeLeft = 2;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle frame = texture.Frame(horizontalFrames: 2, frameX: Projectile.frame);
        Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        return false;

    }
}
