using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Players
{
    public class LeechPlayer : ModPlayer
    {
        public int FlatLeechLife { get; set; }
        public float PercentLeechLife { get; set; }
        public float IncLeechLife { get; set; }

        public int FlatLeechMana { get; set; }
        public float PercentLeechMana { get; set; }
        public float IncLeechMana { get; set; }

        public bool DisableManaPotions { get; set; }

        public override void ResetEffects()
        {
            FlatLeechLife = 0;
            PercentLeechLife = 0f;
            IncLeechLife = 1f;
            FlatLeechMana = 0;
            PercentLeechMana = 0f;
            IncLeechMana = 1f;
            DisableManaPotions = false;
        }

        public override bool CanUseItem(Item item)
        {
            if (item.healMana > 0 && DisableManaPotions) 
                return false;

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && target.type != NPCID.TargetDummy)
            {
                Player.statLife += (int)((FlatLeechLife + damageDone * PercentLeechLife) * IncLeechLife);
                Player.statMana += (int)((FlatLeechMana + damageDone * PercentLeechMana) * IncLeechMana);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && Main.myPlayer == proj.owner && target.type != NPCID.TargetDummy)
            {
                int healLife = (int)((FlatLeechLife + damageDone * PercentLeechLife) * IncLeechLife);

                if (healLife > 0)
                    Player.Heal(healLife);

                int healMana = (int)((FlatLeechMana + damageDone * PercentLeechMana) * IncLeechMana);

                if (healMana > 0)
                {
                    Player.statMana += healMana;
                    Player.ManaEffect(healMana);
                }
            }
        }
    }
}
