using Macrocosm.Common.Players;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Weapons
{
    public class ChandriumWhipBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] <= 1)
            {
                player.GetModPlayer<MacrocosmPlayer>().ChandriumWhipStacks = 0;
            }

            if (player.HandPosition.HasValue)
            {
                Vector2 handPosition = player.HandPosition.Value - player.velocity;
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustDirect(player.Center, 0, 0, ModContent.DustType<ChandriumBrightDust>(), player.direction * 2, 0f, 150, default, 0.5f);
                    dust.position = handPosition;
                    dust.velocity *= 0f;
                    dust.noGravity = true;
                    dust.alpha = 0;
                    dust.fadeIn = 1f;
                    dust.velocity += player.velocity;
                    if (Main.rand.NextBool(2))
                    {
                        dust.position += Main.rand.NextVector2Circular(-6, 6);
                        dust.scale += Main.rand.NextFloat();
                        if (Main.rand.NextBool(2))
                            dust.customData = this;
                    }
                }
            }
        }
    }
}
