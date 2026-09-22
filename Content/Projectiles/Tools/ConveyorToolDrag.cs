using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Tools;

public class ConveyorToolDrag : ModProjectile
{
    public override string Texture => "Terraria/Images/MagicPixel";
    private Point start;
    private ConveyorToolState state;
    private bool initialized;
    public void Initialize(Point origin, ConveyorToolState settings) { start = origin; state = settings; initialized = true; }
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 2;
        Projectile.netImportant = false;
    }

    public override void AI()
    {
        if (Projectile.owner != Main.myPlayer) return;
        Player player = Main.player[Projectile.owner];
        if (!initialized || player.HeldItem.ModItem is not PipeGrandDesign || ConveyorToolUI.IsBlocked(player) || player.mouseInterface || ConveyorToolUI.CancelDrag)
        { Projectile.Kill(); return; }
        Projectile.timeLeft = 2;
        Projectile.Center = new Vector2(Player.tileTargetX * 16 + 8, Player.tileTargetY * 16 + 8);
        if (!player.channel)
        {
            ConveyorSystem.RequestToolOperation(player, start, Projectile.Center.ToTileCoordinates(), state, player.direction == 1);
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (!initialized || Projectile.owner != Main.myPlayer) return false;
        Color color = state.Cutting ? Color.IndianRed : Color.LightSkyBlue;
        foreach (Point p in ConveyorToolState.Path(start, Projectile.Center.ToTileCoordinates(), Main.LocalPlayer.direction == 1))
        {
            Vector2 screen = p.ToVector2() * 16 - Main.screenPosition;
            if (screen.X < -16 || screen.Y < -16 || screen.X > Main.screenWidth || screen.Y > Main.screenHeight) continue;
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)screen.X + 2, (int)screen.Y + 2, 12, 12), color * 0.5f);
        }
        return false;
    }
}
