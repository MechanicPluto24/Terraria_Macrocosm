using Macrocosm.Common.Drawing;
using Macrocosm.Common.Netcode;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Players
{
    public class DashPlayer : ModPlayer
    {

        public bool AccDashHorizontal = false;
        public bool AccDashVertical = false;

        public float AccDashDamage = 0f;    // Set this to > 0 in ModItem.UpdateAccessory for damaging collision 

        // EoC defaults, overridable in ModItem.UpdateAccessory
        public float AccDashVelocity = 10f; // EoC default velocity 
        public float AccDashKnockback = 9f; // EoC default knockback (if damage > 0)
        public int AccDashImmuneTime = 4;   // EoC default immune time  (if damage > 0)

        public int AccDashCooldown = 50;
        public int AccDashDuration = 35;

        public enum DashDir { Down, Up, Right, Left, None = -1 }

        public DashDir DashDirection = DashDir.None;
        private int dashDelay = 0;
        private int dashTimer = 0;


        private bool celestialBulwarkVisible = false;

        public override void PostUpdateBuffs()
        {
            celestialBulwarkVisible = (Player.shield == EquipLoader.GetEquipSlot(Macrocosm.Instance, "CelestialBulwark", EquipType.Shield));

            if (celestialBulwarkVisible)
                Lighting.AddLight(Player.Center, CelestialDisco.CelestialColor.ToVector3() * 0.4f);
        }

        public override void CopyClientState(ModPlayer clientClone)/* tModPorter Suggestion: Replace Item.Clone usages with Item.CopyNetStateTo */
        {
            (clientClone as DashPlayer).DashDirection = DashDirection;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            if ((clientPlayer as DashPlayer).DashDirection != DashDirection)
            {
                SyncPlayer(-1, Main.myPlayer, false);
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncPlayerDashDirection);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)DashDirection);
            packet.Send(toWho, fromWho);
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
        {
            int dashPlayerID = reader.ReadByte();
            DashPlayer dashPlayer = Main.player[dashPlayerID].GetModPlayer<DashPlayer>();

            int newDir = reader.ReadByte();
            dashPlayer.DashDirection = (DashPlayer.DashDir)newDir;

            if (Main.netMode == NetmodeID.Server)
                dashPlayer.SyncPlayer(-1, whoAmI, false);
        }

        public override void ResetEffects()
        {
            AccDashHorizontal = false;
            AccDashVertical = false;

            AccDashDamage = 0f;
            AccDashKnockback = 9f;

            AccDashImmuneTime = 4;
            AccDashVelocity = 10f;

            AccDashCooldown = 50;
            AccDashDuration = 35;

            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap

            if (Player.whoAmI == Main.myPlayer)
            {
                if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[(int)DashDir.Down] < 15)
                    DashDirection = DashDir.Down;
                else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[(int)DashDir.Up] < 15)
                    DashDirection = DashDir.Up;
                else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[(int)DashDir.Right] < 15)
                    DashDirection = DashDir.Right;
                else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[(int)DashDir.Left] < 15)
                    DashDirection = DashDir.Left;
                else
                    DashDirection = DashDir.None;
            }
            //else
            //{
            //	// FIXME: this gets reset again here right after syncing with modpackets
            //	DashDirection = DashDir.None;
            //}
        }

        public override void PreUpdateMovement()
        {
            bool canDash = Player.dashType == 0 && // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
                          !Player.setSolar && // player isn't wearing solar armor
                          !Player.mount.Active;    // player isn't mounted, since dashes on a mount look weird

            // INFO: since other clients have DashDir.None for this player because of the reset, this entire code does not run
            // since general movement in synced automatically, and dash collision is synced below,
            // the only inconsistency remaining is the lack of dust effects
            if (canDash && DashDirection != DashDir.None && dashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDirection)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    case DashDir.Up when Player.velocity.Y > -AccDashVelocity && AccDashVertical:
                    case DashDir.Down when Player.velocity.Y < AccDashVelocity && AccDashVertical:
                        {
                            // Y-velocity is set here
                            // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
                            // This adjustment is roughly 1.3x the intended dash velocity
                            float dashDirection = DashDirection == DashDir.Down ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * AccDashVelocity;
                            break;
                        }
                    case DashDir.Left when Player.velocity.X > -AccDashVelocity && AccDashHorizontal:
                    case DashDir.Right when Player.velocity.X < AccDashVelocity && AccDashHorizontal:
                        {
                            // X-velocity is set here
                            float dashDirection = DashDirection == DashDir.Right ? 1 : -1;
                            newVelocity.X = dashDirection * AccDashVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start the dash
                }

                // start the dash
                dashDelay = AccDashCooldown;
                dashTimer = AccDashDuration;
                Player.velocity = newVelocity;

                // TODO: sync this
                StartDashVisuals();
            }

            if (dashDelay > 0)
                dashDelay--;

            if (dashDelay == 0)
                Player.eocHit = -1;

            if (dashTimer > 0)
            {
                Player.eocDash = dashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
                dashTimer--;

                // TODO: sync this
                DashVisuals();

                #region Dash damage

                // collision with NPCs and the knockback that comes afterwards have to be synced 
                if (Player.whoAmI == Main.myPlayer)
                {
                    if (Player.eocHit < 0)
                    {
                        if (AccDashDamage > 0f)
                        {
                            Rectangle rectangle = new((int)(Player.position.X + Player.velocity.X * 0.5f - 4.0f), (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4.0f), Player.width + 8, Player.height + 8);

                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];

                                if (!npc.active || npc.dontTakeDamage || npc.friendly || (npc.aiStyle == Terraria.ID.NPCAIStyleID.Fairy && !(npc.ai[2] <= 1f)) || !Player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
                                    continue;

                                Rectangle rect = npc.getRect();
                                if (rectangle.Intersects(rect) && (npc.noTileCollide || Player.CanHit(npc)))
                                {
                                    float damage = AccDashDamage * Player.GetDamage(DamageClass.Melee).Multiplicative;
                                    float knockback = AccDashKnockback;
                                    int direction = Player.direction;
                                    bool crit = false;

                                    if (Player.kbGlove)
                                        knockback *= 2f;

                                    if (Player.kbBuff)
                                        knockback *= 1.5f;

                                    if (Main.rand.Next(100) < Player.GetTotalCritChance(DamageClass.Melee))
                                        crit = true;

                                    if (Player.velocity.X < 0f)
                                        direction = -1;

                                    if (Player.velocity.X > 0f)
                                        direction = 1;

                                    if (Player.whoAmI == Main.myPlayer)
                                        Player.ApplyDamageToNPC(npc, (int)damage, knockback, direction, crit);

                                    Player.eocDash = 10;
                                    Player.dashDelay = AccDashCooldown;
                                    Player.velocity.X = -direction * AccDashVelocity * 0.75f;
                                    Player.velocity.Y = -1f * AccDashVelocity * 0.25f;
                                    Player.GiveImmuneTimeForCollisionAttack(AccDashImmuneTime);
                                    Player.eocHit = i;

                                }
                            }
                        }
                    }
                    else if ((!Player.controlLeft || !(Player.velocity.X < 0f)) && (!Player.controlRight || !(Player.velocity.X > 0f)))
                    {
                        Player.velocity.X *= 0.95f;
                    }

                    NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                }


                #endregion
            }
        }

        public void StartDashVisuals()
        {
            if (celestialBulwarkVisible)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustIdx = Dust.NewDust(new Vector2(Player.position.X - 20, Player.position.Y - 10), Player.width + 20, Player.height + 10, ModContent.DustType<CelestialDust>(), Player.direction * -1f, 0f, 0, default, 2f);
                    Main.dust[dustIdx].position.X += Main.rand.Next(-5, 6);
                    Main.dust[dustIdx].position.Y += Main.rand.Next(-5, 6);
                    Main.dust[dustIdx].velocity.X *= 0.6f;
                    Main.dust[dustIdx].scale *= 1.4f + (float)Main.rand.Next(20) * 0.01f;
                }
            }
        }

        public void DashVisuals()
        {
            if (celestialBulwarkVisible)
            {
                for (int k = 0; k < 3; k++)
                {
                    int dustType = ModContent.DustType<CelestialDust>();
                    //int dustIdx = ((Player.velocity.Y != 0f) ? 
                    //	Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + Player.height / 2 - 8f), Player.width, 16, dustType, 0f, 0f, 100, default, 1.4f) : 
                    //	Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + Player.height - 4f), Player.width, 8, dustType, 0f, 0f, 100, default, 1.4f));

                    int dustIdx = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y), Player.width, Player.height, dustType, 0f, 0f, 100, default, 1.4f);
                    Main.dust[dustIdx].velocity *= 0.1f;
                    Main.dust[dustIdx].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                }
            }
        }
    }
}
