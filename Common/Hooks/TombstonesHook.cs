using Macrocosm.Content.Projectiles.Friendly.Tombstones;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    class TombstonesHook : ILoadable
    {
        public void Load(Mod mod)
        {
            Terraria.On_Player.DropTombstone += Player_DropTombstone;
        }


        public void Unload()
        {
            Terraria.On_Player.DropTombstone -= Player_DropTombstone;
        }

        private void Player_DropTombstone(Terraria.On_Player.orig_DropTombstone orig, Player self, long coinsOwned, NetworkText deathText, int hitDirection)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // here could be a switch statement based on the subworld
                    int tombstoneType = ModContent.ProjectileType<MoonTombstone>();

                    // whether to drop a normal or golden tombstone 
                    float golden = 0f;
                    if (coinsOwned > 100000)
                        golden = 1f;

                    float speed;
                    for (speed = (float)Main.rand.Next(-35, 36) * 0.1f; speed < 2f && speed > -2f; speed += (float)Main.rand.Next(-30, 31) * 0.1f) { }

                    int proj = Projectile.NewProjectile(self.GetSource_Misc("PlayerDeath_TombStone"),
                        self.position.X + (float)(self.width / 2),
                        self.position.Y + (float)(self.height / 2),
                        (float)Main.rand.Next(10, 30) * 0.1f * (float)hitDirection + speed,
                        (float)Main.rand.Next(-40, -20) * 0.1f,
                        tombstoneType, 0, 0f, Main.myPlayer, ai0: golden);

                    DateTime now = DateTime.Now;
                    string str = now.ToString("D");

                    if (GameCulture.FromCultureName(GameCulture.CultureName.English).IsActive)
                        str = now.ToString("MMMM d, yyy");

                    string miscText = deathText.ToString() + "\n" + str;
                    Main.projectile[proj].miscText = miscText;
                }
            }
            else
            {
                orig(self, coinsOwned, deathText, hitDirection);
            }
        }

    }
}
