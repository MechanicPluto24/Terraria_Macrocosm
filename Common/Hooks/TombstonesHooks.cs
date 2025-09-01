using Macrocosm.Content.Projectiles.Friendly.Tombstones;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    class TombstonesHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Player.DropTombstone += Player_DropTombstone;
        }


        public void Unload()
        {
            On_Player.DropTombstone -= Player_DropTombstone;
        }

        private void Player_DropTombstone(On_Player.orig_DropTombstone orig, Player player, long coinsOwned, NetworkText deathText, int hitDirection)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    bool golden = coinsOwned > 100000;

                    // TODO: Here could be either:
                    // - a switch statement based on the subworld
                    // - a pair of proj types, members of MacrocosmSubworld.Current
                    int tombstoneType = golden ? ModContent.ProjectileType<MoonGoldTombstone>() : ModContent.ProjectileType<MoonTombstone>();

                    float speed;
                    for (speed = Main.rand.Next(-35, 36) * 0.1f; speed < 2f && speed > -2f; speed += Main.rand.Next(-30, 31) * 0.1f) { }

                    Projectile tombstone = Projectile.NewProjectileDirect(player.GetSource_Misc("PlayerDeath_TombStone"),
                        new Vector2
                        (
                            player.position.X + player.width / 2,
                            player.position.Y + player.height / 2
                        ),
                        new Vector2
                        (
                            Main.rand.Next(10, 30) * 0.1f * (float)hitDirection + speed,
                            Main.rand.Next(-40, -20) * 0.1f
                        ),
                        tombstoneType, 0, 0f, Main.myPlayer);

                    DateTime now = DateTime.Now;
                    string str = now.ToString("D");

                    if (GameCulture.FromCultureName(GameCulture.CultureName.English).IsActive)
                        str = now.ToString("MMMM d, yyy");

                    string miscText = deathText.ToString() + "\n" + str;
                    tombstone.miscText = miscText;
                    tombstone.netUpdate = true;
                }
            }
            else
            {
                orig(player, coinsOwned, deathText, hitDirection);
            }
        }

    }
}
