using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Players
{
    public class CritMultiPlayer : ModPlayer
    {
        public float AddCritMulti = 0f;
        public float NonCritMulti = 1f;
        public bool tempestBand;

        public override void ResetEffects()
        {
            AddCritMulti = 0f;
            NonCritMulti = 1f;
            tempestBand = false;
        }
    }

    public class CritMultiItem : GlobalItem
    {
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += player.GetModPlayer<CritMultiPlayer>().AddCritMulti;
            modifiers.NonCritDamage *= player.GetModPlayer<CritMultiPlayer>().NonCritMulti;
        }
    }

    public class CritMultiProjectile : GlobalProjectile
    {
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.myPlayer == projectile.owner)
            {
                modifiers.CritDamage += Main.player[Main.myPlayer].GetModPlayer<CritMultiPlayer>().AddCritMulti;
                modifiers.NonCritDamage *= Main.player[Main.myPlayer].GetModPlayer<CritMultiPlayer>().NonCritMulti;
            }
        }
    }
}
