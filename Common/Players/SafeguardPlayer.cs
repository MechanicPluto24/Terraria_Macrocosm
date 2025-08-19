using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players;

public class SafeguardPlayer : ModPlayer
{
    public bool Safeguard { get; set; }

    public override void ResetEffects()
    {
        Safeguard = false;
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Safeguard)
        {
            modifiers.Cancel();
            Player.ClearBuff(ModContent.BuffType<Content.Buffs.Potions.SafeguardBuff>());
            Player.immuneTime = 60;
        }
    }
}
