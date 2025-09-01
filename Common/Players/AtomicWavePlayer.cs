using Macrocosm.Content.Projectiles.Friendly.Buff;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class AtomicWavePlayer : ModPlayer
    {
        public bool AtomicWave { get; set; }

        private int cooldown;

        public override void ResetEffects()
        {
            AtomicWave = false;
        }

        public override void PostUpdate()
        {
            if (AtomicWave)
            {
                if (cooldown >= 1)
                    cooldown--;

                bool close = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && Vector2.Distance(Player.Center, npc.Center) < 200f)
                        close = true;
                }

                if (close && cooldown < 1)
                {
                    Projectile.NewProjectile(new EntitySource_Misc("AtomicWave"), Player.Center, Vector2.Zero, ModContent.ProjectileType<AtomicWaveProjectile>(), 700, 10f, Main.myPlayer);
                    cooldown = 300;
                }
            }
        }
    }
}
