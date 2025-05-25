using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Projectiles.Friendly.Misc;
using Terraria.DataStructures;

namespace Macrocosm.Common.Players
{
    public class AtomicWavePotionPlayer : ModPlayer
    {
        public bool AtomicWave { get; set; }

        private int Cooldown;

        public override void ResetEffects()
        {
            AtomicWave = false;
        }

        public override void PostUpdate()
        {
            if (AtomicWave)
            {
                if (Cooldown >= 1)
                {
                    Cooldown--;
                }
                bool close =false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && Vector2.Distance(Player.Center,npc.Center) < 200f)
                        close=true;
                }
                if(close&&Cooldown<1)
                {
                    Projectile.NewProjectile(new EntitySource_Misc("AtomicWave"), Player.Center, Vector2.Zero, ModContent.ProjectileType<AtomicWaveProjectile>(), 700, 10f, Main.myPlayer);
                    Cooldown=300;
                }
            }
        }
    }
}
