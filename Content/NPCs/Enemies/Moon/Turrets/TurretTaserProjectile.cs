using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Buffs.Radiation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.Turrets;

public class TurretTaserProjectile : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;

        Redemption.AddElementToProjectile(Type, Redemption.ElementID.Thunder);
    }

    private static Asset<Texture2D> chainTexture;
    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.hide = true;
    }

    public int AI_Timer
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;    
    }

    public int Owner
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCs.Add(index);
    }

    public override void AI()
    {
        NPC owner = Main.npc[Owner];
        if (!owner.active || owner.type != ModContent.NPCType<TaserTurret>())
            Projectile.Kill();

        TaserTurret turrent = owner.ModNPC as TaserTurret;
        if (AI_Timer < 60)
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        else
        {
            Projectile.velocity = (owner.Center + turrent.TurretHeight - Projectile.Center).SafeNormalize(Vector2.UnitX) * 10f;
            if (Vector2.Distance(Projectile.Center, owner.Center + turrent.TurretHeight) < 12f)
                Projectile.Kill();
        }

        AI_Timer++;
    }

    public override void OnKill(int timeLeft)
    {
        NPC owner = Main.npc[Owner];
        if (owner.active && owner.type == ModContent.NPCType<TaserTurret>())
            owner.ai[0] = 0f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(144, 300);
        target.AddBuff(ModContent.BuffType<Paralysis>(), 60);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        NPC owner = Main.npc[Owner];
        TaserTurret turret = owner.ModNPC as TaserTurret;

        chainTexture ??= ModContent.Request<Texture2D>(Texture + "_Chain");
        int chainlength = (int)(Vector2.Distance(Projectile.Center, owner.Center + turret.TurretHeight) / (chainTexture.Value).Width);
        for (int i = 0; i <= chainlength; i++)
        {
            Main.EntitySpriteDraw(chainTexture.Value, Projectile.Center + ((new Vector2(1, 0)).RotatedBy((owner.Center + turret.TurretHeight - Projectile.Center).ToRotation()) * (i * (chainTexture.Value).Width)) - Main.screenPosition, null, lightColor, (owner.Center + turret.TurretHeight - Projectile.Center).ToRotation(), chainTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        }
        return true;
    }
}
