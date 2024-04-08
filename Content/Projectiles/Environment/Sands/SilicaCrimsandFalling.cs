using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Sands
{
    public class SilicaCrimsandFalling : ModProjectile
    {
        public override string Texture => base.Texture.Replace("Falling", "");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.ForcePlateDetection[Type] = true;
            ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
            ProjectileID.Sets.FallingBlockTileItem[Type] = new(ModContent.TileType<Tiles.Blocks.SilicaCrimsand>(), ModContent.ItemType<Items.Placeable.Blocks.SilicaCrimsand>());
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.CrimsandBallFalling);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SilicaCrimsandDust>());
                Main.dust[dust].velocity.X *= 0.4f;
            }
        }
    }
}