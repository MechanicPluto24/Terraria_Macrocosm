using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Players
{
    public class LeechPlayer : ModPlayer
    {
        public int flatLeechLife;
        public float percentLeechLife;
        public float incLeechLife;

        public int flatLeechMana;
        public float percentLeechMana;
        public float incLeechMana;

        public bool manaPotionDisabler;
        public override void ResetEffects()
        {
            flatLeechLife = 0;
            percentLeechLife = 0f;
            incLeechLife = 1f;
            flatLeechMana= 0;
            percentLeechMana = 0f;
            incLeechMana = 1f;
            manaPotionDisabler = false;
        }
    }

    public class LeechItem : GlobalItem
    {
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && target.type != NPCID.TargetDummy)
            {
                var p = player.GetModPlayer<LeechPlayer>();
                player.statLife += (int)((p.flatLeechLife + damageDone * p.percentLeechLife) * p.incLeechLife);
                player.statMana += (int)((p.flatLeechMana + damageDone * p.percentLeechMana) * p.incLeechMana);
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.healMana > 0 && player.GetModPlayer<LeechPlayer>().manaPotionDisabler) return false;
            return true;
        }
    }

    public class LeechProjectile : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && Main.myPlayer == projectile.owner && target.type != NPCID.TargetDummy)
            {
                var p = Main.player[Main.myPlayer];
                var mp = p.GetModPlayer<LeechPlayer>();
                //p.statLife += (int)((mp.flatLeechLife + damageDone * mp.percentLeechLife) * mp.incLeechLife);
                int healLife = (int)((mp.flatLeechLife + damageDone * mp.percentLeechLife) * mp.incLeechLife);
                if (healLife > 0) p.Heal(healLife);
                int healMana = (int)((mp.flatLeechMana + damageDone * mp.percentLeechMana) * mp.incLeechMana);
                if (healMana > 0)
                {
                    p.statMana += healMana;
                    p.ManaEffect(healMana);
                }

            }
        }
    }
}
