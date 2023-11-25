using Macrocosm.Content.Dusts;
using Macrocosm.Content.Tiles.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Tombstones
{
    public class MoonTombstone : ModProjectile
    {
        public const int normalCount = 1;
        public const int goldenCount = 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = normalCount + goldenCount;
        }

        public override void SetDefaults()
        {
            Projectile.knockBack = 12f;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.GraveMarker; // needed for bounce 
            Projectile.penetrate = -1;
            DrawOffsetX = -5;
            DrawOriginOffsetX = 0;
            DrawOriginOffsetY = -5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 0f)
                Projectile.frame = Main.rand.Next(normalCount);
            else
                Projectile.frame = normalCount + Main.rand.Next(goldenCount);
        }

        public override bool PreAI()
        {
            if (Projectile.velocity.Y == 0f)
                Projectile.velocity.X *= 0.98f;

            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Projectile.velocity.Y += 0.2f;

            if (Projectile.owner != Main.myPlayer)
                return false;


            int tileX = (int)((Projectile.position.X + (float)(Projectile.width / 2)) / 16f);
            int tileY = (int)((Projectile.position.Y + (float)Projectile.height - 4f) / 16f);

            bool placeTile = false;

            bool onRegolith = Main.tile[tileX, tileY + 1].TileType == ModContent.TileType<Regolith>() && Main.tile[tileX + 1, tileY + 1].TileType == ModContent.TileType<Regolith>();
            int style = GetTombstoneStyle(onRegolith);
            if (TileObject.CanPlace(tileX, tileY, ModContent.TileType<Tiles.Tombstones.MoonTombstone>(), style, Projectile.direction, out TileObject objectData))
                placeTile = TileObject.Place(objectData);

            if (placeTile)
            {
                NetMessage.SendObjectPlacement(-1, tileX, tileY, objectData.type, objectData.style, objectData.alternate, objectData.random, Projectile.direction);
                SoundEngine.PlaySound(SoundID.Dig, new Vector2(tileX * 16, tileY * 16));
                int signId = Sign.ReadSign(tileX, tileY);
                if (signId >= 0)
                {
                    Sign.TextSign(signId, Projectile.miscText);
                    NetMessage.SendData(MessageID.ReadSign, -1, -1, null, signId, 0f, (int)(byte)new BitsByte(b1: true));
                }

                if (onRegolith)
                    ImpactEffect();

                Projectile.Kill();
            }

            return false;
        }

        private int GetTombstoneStyle(bool onRegolith) => Projectile.frame * 2 + (onRegolith ? 0 : 1);

        private void ImpactEffect()
        {
            for (int i = 0; i < Main.rand.Next(30, 45); i++)
            {
                Dust dust = Dust.NewDustDirect(
                    new Vector2(Projectile.Center.X, Projectile.Center.Y),
                    Projectile.width,
                    Projectile.height,
                    ModContent.DustType<RegolithDust>(),
                    Main.rand.NextFloat(-0.8f, 0.8f),
                    Main.rand.NextFloat(0f, -4f),
                    Scale: Main.rand.NextFloat(1f, 1.3f)
                );

                dust.noGravity = false;
            }
        }
    }
}
