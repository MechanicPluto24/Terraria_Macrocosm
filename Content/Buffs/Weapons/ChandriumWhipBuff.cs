using Macrocosm.Content.Players;
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
                Vector2 vector = player.HandPosition.Value - player.velocity;
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(player.Center, 0, 0, DustID.PinkTorch, player.direction * 2, 0f, 150, default, 1.3f)];
                    dust.position = vector;
                    dust.velocity *= 0f;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.velocity += player.velocity;
                    if (Main.rand.NextBool(2))
                    {
                        dust.position += Utils.RandomVector2(Main.rand, -4f, 4f);
                        dust.scale += Main.rand.NextFloat();
                        if (Main.rand.NextBool(2))
                            dust.customData = this;
                    }
                }
            }
        }
    }
}
