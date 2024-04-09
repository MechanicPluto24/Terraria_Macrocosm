using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Sands
{
    public class SilicaEbonsandFalling : ModProjectile
    {
        public override string Texture => base.Texture.Replace("Falling", "");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.ForcePlateDetection[Type] = true;
            ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
            ProjectileID.Sets.FallingBlockTileItem[Type] = new(ModContent.TileType<Tiles.Blocks.Sands.SilicaEbonsand>(), ModContent.ItemType<Items.Blocks.Sands.SilicaEbonsand>());
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EbonsandBallFalling);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SilicaEbonsandDust>());
                Main.dust[dust].velocity.X *= 0.4f;
            }
        }
    }
}