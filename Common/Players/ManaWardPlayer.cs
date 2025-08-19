using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players;

public class ManaWardPlayer : ModPlayer
{
    public bool ManaWard { get; set; }

    public override void ResetEffects()
    {
        ManaWard = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (ManaWard)
        {
            int manaDamage = 0;
            if ((float)Player.statLife / Player.statLifeMax2 < 0.3f)
            {
                manaDamage = info.Damage;

                if (manaDamage > Player.statMana)
                    manaDamage = Player.statMana;

                Player.statMana -= manaDamage;
                Player.statLife -= info.Damage - manaDamage;
            }
        }
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (ManaWard && (float)Player.statLife / Player.statLifeMax2 < 0.3f) modifiers.FinalDamage *= 0.5f;
    }
}
