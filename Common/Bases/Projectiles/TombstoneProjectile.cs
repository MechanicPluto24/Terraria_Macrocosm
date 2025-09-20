using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles;

public abstract class TombstoneProjectile : ModProjectile
{
    /// <summary> The tile to place on successful landing </summary>
    public abstract int TileType { get; }

    /// <summary> The target rock type for the "rubble" (embedded in rock) tile style </summary>
    public virtual int TargetRockTileType => -1;

    /// <summary> Dust created when impacting the <see cref="TargetRockTileType"/> </summary>
    public virtual int ImpactDustType => -1;

    /// <summary> The number of styles this tombstone has. </summary>
    public abstract int StyleCount { get; }

    /// <summary> Width of the projectile, used for framing </summary>
    public virtual int Width => 32;

    /// <summary> Height of the projectile, used for framing </summary>
    public virtual int Height => 34;

    /// <summary>
    /// The style of this tombstone. Used for picking the frame, wrapped by StyleCount, and it's multiplied by 2 to get the placed tile style.
    /// </summary>
    private int Style
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = StyleCount;
    }

    public override void SetDefaults()
    {
        Projectile.knockBack = 12f;
        Projectile.width = Width;
        Projectile.height = Height;
        Projectile.aiStyle = ProjAIStyleID.GraveMarker; // needed for bounce 
        Projectile.penetrate = -1;
        DrawOffsetX = -5;
        DrawOriginOffsetX = 0;
        DrawOriginOffsetY = -5;
    }

    private bool spawned;

    public override bool PreAI()
    {
        if (!spawned)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Style = Main.rand.Next(StyleCount);
                Projectile.netUpdate = true;
            }
            spawned = true;
        }

        Projectile.frame = Style;


        if (Projectile.velocity.Y == 0f)
            Projectile.velocity.X *= 0.98f;

        Projectile.rotation += Projectile.velocity.X * 0.1f;
        Projectile.velocity.Y += 0.2f;

        if (Projectile.owner != Main.myPlayer)
            return false;

        int tileX = (int)((Projectile.position.X + Projectile.width / 2) / 16f);
        int tileY = (int)((Projectile.position.Y + Projectile.height - 4f) / 16f);

        bool placeTile = false;
        bool onTargetRock = TargetRockTileType > 0 && Main.tile[tileX, tileY + 1].TileType == TargetRockTileType && Main.tile[tileX + 1, tileY + 1].TileType == TargetRockTileType;
        int tileStyle = Style * 2 + (onTargetRock ? 1 : 0);

        if (TileObject.CanPlace(tileX, tileY, TileType, tileStyle, Projectile.direction, out TileObject objectData))
            placeTile = TileObject.Place(objectData);

        if (placeTile)
        {
            NetMessage.SendObjectPlacement(-1, tileX, tileY, objectData.type, objectData.style, objectData.alternate, objectData.random, Projectile.direction);
            SoundEngine.PlaySound(SoundID.Dig, new Vector2(tileX * 16, tileY * 16));
            int signId = Sign.ReadSign(tileX, tileY);
            if (signId >= 0)
            {
                Sign.TextSign(signId, Projectile.miscText);
                NetMessage.SendData(MessageID.ReadSign, -1, -1, null, signId, 0f, (byte)new BitsByte(b1: true));
            }

            if (onTargetRock && ImpactDustType > 0)
                ImpactEffect();

            Projectile.Kill();
        }

        return false;
    }

    private void ImpactEffect()
    {
        for (int i = 0; i < Main.rand.Next(30, 45); i++)
        {
            Dust dust = Dust.NewDustDirect(
                new Vector2(Projectile.Center.X, Projectile.Center.Y),
                Projectile.width,
                Projectile.height,
                ImpactDustType,
                Main.rand.NextFloat(-0.8f, 0.8f),
                Main.rand.NextFloat(0f, -4f),
                Scale: Main.rand.NextFloat(1f, 1.3f)
            );

            dust.noGravity = false;
        }
    }
}
