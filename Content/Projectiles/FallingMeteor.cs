using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Items.Materials;
using System;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Miscellaneous;

namespace Macrocosm.Content.Projectiles {
    public class FallingMeteor : ModProjectile {

        public override void SetDefaults() {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 500;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
        }

        public override void Kill(int timeLeft) {

            #region Spawn Items

            if(Main.netMode != NetmodeID.MultiplayerClient) {
                if (Main.rand.NextBool(3)) {
                    //int type = Utils.SelectRandom<int>(Main.rand, SomeGeode, SomeOtherGeode); -- maybe WeigthedRandom?
                    int type = ModContent.ItemType<MoonGeode>();
                    Vector2 position = new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height);
                    int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
                }
            }
                      
            #endregion

            #region Gores

            #endregion

            if (Main.netMode != NetmodeID.Server) {

                #region Dusts

                for(int i = 0; i < Main.rand.Next(30,80); i++) {
                    Dust dust = Dust.NewDustDirect(
                        new Vector2(Projectile.position.X, Projectile.position.Y + 1.5f * Projectile.height), 
                        Projectile.width, 
                        Projectile.height, 
                        ModContent.DustType<RegolithDust>(), 
                        Main.rand.NextFloat(-1f, 1f), 
                        Main.rand.NextFloat(0f, -5f), 
                        Scale: Main.rand.NextFloat(1.5f, 2f)
                    );

                    dust.noGravity = false;

                }

                #endregion

                #region Sounds

                #endregion
            }

            #region Screenshake effect

            // let the server do it for every player
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float maxDist = 110f * 16f; // 110 tiles max distance (least screenshake) 
            float maxScreenshake = 40f; // max screenshake (up to 100) for distance = 0

            for(int i = 0; i < 255; i++) {

                Player player = Main.player[i];

                if (player.active) {
                    float distance = Vector2.Distance(player.Center, Projectile.Center);

                    if(distance < maxDist)
                        player.GetModPlayer<MacrocosmPlayer>().ScreenShakeIntensity = maxScreenshake - distance/maxDist * maxScreenshake;
                }
            }

            #endregion
        }

        override public void AI() {

            if (Projectile.ai[1] == 0f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
                Projectile.ai[1] = 1f;
                Projectile.netUpdate = true;
            }
            
            if (Projectile.ai[1] != 0f)
                Projectile.tileCollide = true;

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
        }



    }
}
