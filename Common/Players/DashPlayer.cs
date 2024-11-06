using Macrocosm.Common.Netcode;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class DashPlayer : ModPlayer
    {
        /// <summary> Whether the player can dash horizontally (left or right) </summary>
        public bool AccDashHorizontal { get; set; }

        /// <summary> Whether the player can dash vertically (up or down) </summary>
        public bool AccDashVertical { get; set; }

        /// <summary> Whether the player can dash downwards (down only) </summary>
        public bool AccDashDownwards { get; set; }

        /// <summary> Cooldown between dashes, defaults to EoC shield value </summary>
        public int AccDashCooldown { get; set; }

        /// <summary> Dash duration, defaults to EoC shield value </summary>
        public int AccDashDuration { get; set; }

        /// <summary> The damage applied on collision with the enemy, value <= 0 for no damage, defaults to 0 </summary>
        public float AccDashDamage { get; set; }

        /// <summary> The horizontal dash speed, defaults to EoC shield value </summary>
        public float AccDashSpeedX { get; set; }

        /// <summary> The vertical dash speed, defaults to half of the EoC shield value </summary>
        public float AccDashSpeedY { get; set; }

        /// <summary> 
		/// Whether to keep the other velocity component when dashing, or reset it
		/// <br>(keep <see cref="Player.velocity.Y"/> when dashing horizontally, and vice-versa).</br>
		/// <br>Defaults to true.</br>
		/// </summary>
        public bool AccDashPreserveVelocity { get; set; }

        /// <summary>
        /// Multiplier to allow player controls during dash. 
		/// <br>1f - can move for the full duration of the dash </br>
		/// <br>0f - can't move at all during the dash </br>
		/// <br>Defaults to 1f.</br>
		/// </summary>
        public float AccDashAllowMovementDuringDashMultiplier { get; set; }

        /// <summary> The knockback applied on collision with the enemy, when <see cref="AccDashDamage"/> > 0 </summary>
        public float AccDashKnockback { get; set; }

        /// <summary> The immunity time applied on collision with the enemy, when <see cref="AccDashDamage"/> > 0 </summary>
        public int AccDashImmuneTime { get; set; }

        /// <summary> The player hitbox increase for larger collision hitbox, using <see cref="Rectangle.Inflate(int, int)"/></summary>
        public int AccDashHitboxIncrease { get; set; }

        /// <summary> Whether the dash makes a player afterimage </summary>
        public bool AccDashAfterImage { get; set; }

        /// <summary> Action called when starting the dash, used mainly for visual and sound effects </summary>
        public Action<Player> AccDashStartVisuals { get; set; }

        /// <summary> Action called for the duration of the dash, used mainly for visual and sound effects </summary>
        public Action<Player> AccDashVisuals { get; set; }

        public enum Direction { Down, Up, Right, Left, None = -1 }

        /// <summary> The dash direction triggered by the player </summary>
        public Direction DashDirection { get; set; } = Direction.None;
        private Direction LastValidDashDirection;
        private int dashDelay = 0;
        private int dashTimer = 0;

        /// <summary> Any dash accessory equipped and active </summary>
        public bool DashAccessoryEquipped => AccDashHorizontal || AccDashVertical || AccDashDownwards;

        /// <summary> Dash timer, from 0 to <see cref="AccDashDuration"/> </summary>
        public int DashTimer => dashTimer;
        public float DashProgress => (float)dashTimer / AccDashDuration;

        /// <summary> Whether the player has collided during the dash with an NPC this frame </summary>
        public bool CollidedWithNPC { get; set; }

        /// <summary> Action called when colliding with an NPC </summary>
        public Action<Player, NPC> OnCollisionWithNPC { get; set; }

        public override void PostUpdateBuffs()
        {
        }

        public override void ResetEffects()
        {
            AccDashHorizontal = false;
            AccDashVertical = false;
            AccDashDownwards = false;

            AccDashCooldown = 50;
            AccDashDuration = 35;

            AccDashSpeedX = 14f;
            AccDashSpeedY = 7f;

            AccDashPreserveVelocity = true;
            AccDashAllowMovementDuringDashMultiplier = 1f;

            AccDashDamage = 0f;
            AccDashKnockback = 9f;
            AccDashImmuneTime = 4;
            AccDashHitboxIncrease = 1;

            AccDashAfterImage = true;
            AccDashStartVisuals = null;
            AccDashVisuals = null;

            CollidedWithNPC = false;
            OnCollisionWithNPC = null;


            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
            if (Player.whoAmI == Main.myPlayer)
            {
                if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[(int)Direction.Down] < 15)
                    DashDirection = Direction.Down;
                else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[(int)Direction.Up] < 15)
                    DashDirection = Direction.Up;
                else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[(int)Direction.Right] < 15)
                    DashDirection = Direction.Right;
                else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[(int)Direction.Left] < 15)
                    DashDirection = Direction.Left;
                else
                    DashDirection = Direction.None;
            }
            //else
            //{
            //	// FIXME: this gets reset again here right after syncing with modpackets
            //	DashDirection = DashDir.None;
            //}

            if (DashDirection is not Direction.None)
                LastValidDashDirection = DashDirection;
        }

        public override void PreUpdateMovement()
        {
            // INFO: since other clients have DashDir.None for this player because of the reset, this entire code does not run
            // since general movement in synced automatically, and dash collision is synced below,
            // the only inconsistency remaining is the lack of dust effects
            if (CanUseDash() && DashDirection != Direction.None && dashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDirection)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    case Direction.Up when Player.velocity.Y > -AccDashSpeedY && AccDashVertical:
                    case Direction.Down when Player.velocity.Y < AccDashSpeedY && (AccDashVertical || AccDashDownwards):
                        {
                            // Y-velocity is set here
                            // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" to compensate gravity being immediately in effect
                            // This adjustment is roughly 1.3x the intended dash velocity
                            float dashDirection = DashDirection == Direction.Down ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * AccDashSpeedY;
                            if (!AccDashPreserveVelocity)
                                newVelocity.X = 0;
                            break;
                        }
                    case Direction.Left when Player.velocity.X > -AccDashSpeedX && AccDashHorizontal:
                    case Direction.Right when Player.velocity.X < AccDashSpeedX && AccDashHorizontal:
                        {
                            // X-velocity is set here
                            float dashDirection = DashDirection == Direction.Right ? 1 : -1;
                            newVelocity.X = dashDirection * AccDashSpeedX;
                            if (!AccDashPreserveVelocity)
                                newVelocity.Y = 0;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start the dash
                }

                // start the dash
                dashDelay = AccDashCooldown;
                dashTimer = AccDashDuration;
                Player.velocity = newVelocity;
                AccDashStartVisuals?.Invoke(Player);
            }

            if (dashDelay > 0)
                dashDelay--;

            if (dashDelay == 0)
                Player.eocHit = -1;

            if (dashTimer > 0)
            {
                dashTimer--;

                AccDashVisuals?.Invoke(Player);

                if (AccDashAfterImage)
                {
                    Player.eocDash = dashTimer;
                    Player.armorEffectDrawShadowEOCShield = true;
                }

                if (AccDashDamage > 0)
                    DashDamage();
            }

            if (dashTimer > (int)(AccDashAllowMovementDuringDashMultiplier * AccDashDuration))
            {
                if (LastValidDashDirection is Direction.Down or Direction.Up)
                {
                    Player.velocity.X = 0f;
                }
            }
        }

        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                //&& Player.dashType == DashID.None // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
                //&& !Player.setSolar // player isn't wearing solar armor
                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }

        private void DashDamage()
        {
            // collision with NPCs and the knockback that comes afterwards have to be synced 
            if (Player.whoAmI == Main.myPlayer)
            {
                if (Player.eocHit < 0)
                {
                    Rectangle playerHitbox = new((int)(Player.position.X + Player.velocity.X * 0.5f - 4.0f), (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4.0f), Player.width + 8, Player.height + 8);
                    playerHitbox.Inflate(AccDashHitboxIncrease, AccDashHitboxIncrease);

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (!npc.active || npc.dontTakeDamage || npc.friendly || npc.aiStyle == NPCAIStyleID.Fairy && !(npc.ai[2] <= 1f) || !Player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
                            continue;

                        Rectangle npcHitbox = npc.getRect();
                        if (playerHitbox.Intersects(npcHitbox) && (npc.noTileCollide || Player.CanHit(npc)))
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

                            if (Math.Abs(Player.velocity.X) > Math.Abs(Player.velocity.Y))
                                Player.velocity.X = -direction * AccDashSpeedX * 0.75f;
                            else
                                Player.velocity.Y = -1f * AccDashSpeedY * 0.5f;

                            Player.GiveImmuneTimeForCollisionAttack(AccDashImmuneTime);
                            Player.eocHit = i;

                            CollidedWithNPC = true;
                            OnCollisionWithNPC?.Invoke(Player, npc);
                        }
                    }
                }
                else if ((!Player.controlLeft || !(Player.velocity.X < 0f)) && (!Player.controlRight || !(Player.velocity.X > 0f)))
                {
                    Player.velocity.X *= 0.95f;
                }

                NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
            }
        }

        public override void CopyClientState(ModPlayer clientClone)
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
            packet.Write((byte)MessageType.SyncDashPlayer);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)DashDirection);
            packet.Send(toWho, fromWho);
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
        {
            int playerWhoAmI = reader.ReadByte();
            DashPlayer dashPlayer = Main.player[playerWhoAmI].GetModPlayer<DashPlayer>();

            int newDir = reader.ReadByte();
            dashPlayer.DashDirection = (Direction)newDir;

            if (Main.netMode == NetmodeID.Server)
                dashPlayer.SyncPlayer(-1, whoAmI, false);
        }
    }
}
