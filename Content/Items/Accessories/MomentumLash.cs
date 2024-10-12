using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class MomentumLash : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.ApprenticeScarf}";

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MomentumLashPlayer>().MomentumLash = true;
        }
    }

    public class MomentumLashPlayer : ModPlayer
    {
        public bool MomentumLash;
        public float momentumBonus;
        public int decreaseTick;
        public override void ResetEffects()
        {
            MomentumLash = false;
        }

        public override void PostUpdateEquips()
        {
            if(MomentumLash)
            {
                if(decreaseTick < 60 && momentumBonus > 0)
                {
                    decreaseTick++;
                }
                if(decreaseTick >= 60)
                {
                    momentumBonus -= 0.01f;
                    decreaseTick = 45;
                }
                Player.GetDamage<SummonDamageClass>() += momentumBonus;
                Player.GetAttackSpeed<SummonDamageClass>() += momentumBonus;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (MomentumLash && ProjectileID.Sets.IsAWhip[proj.type] == true && !target.friendly && target.type != NPCID.TargetDummy)
            {
                if (momentumBonus < 0.25f) momentumBonus += 0.01f;
                decreaseTick = 0;
            }
        }
    }
}
