using Macrocosm.Common.Netcode;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.CameraModifiers;
using Macrocosm.Content.Debuffs.Environment;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class RocketPlayer : ModPlayer
    {
        public bool InRocket { get; set; } = false;
        public bool IsCommander { get; set; } = false;
        public int RocketID { get; set; } = -1;
        public string TargetWorld { get; set; } = "";

        private PanCameraModifier cameraModifier;

        public override void Load()
        {
            On_Main.RefreshPlayerDrawOrder += On_Main_RefreshPlayerDrawOrder;
        }

        public override void Unload()
        {
            On_Main.RefreshPlayerDrawOrder -= On_Main_RefreshPlayerDrawOrder;
        }


        public override void ResetEffects()
        {
            if (RocketID < 0 || RocketID >= RocketManager.ActiveRocketCount)
                InRocket = false;
            else if (!RocketManager.Rockets[RocketID].Active)
                InRocket = false;

            if (!InRocket)
            {
                IsCommander = false;
                RocketID = -1;
                TargetWorld = "";
            }
        }

        public void EmbarkPlayerInRocket(int rocketId, bool asCommander = false)
        {
            RocketID = rocketId;
            IsCommander = asCommander;

            if (Player.whoAmI == Main.myPlayer)
            {
                cameraModifier = new(RocketManager.Rockets[RocketID].Center - new Vector2(Main.screenWidth, Main.screenHeight) / 2f, Main.screenPosition, 0.015f, "PlayerInRocket", Utility.QuadraticEaseOut);
                Main.instance.CameraModifiers.Add(cameraModifier);
            }

            InRocket = true;

            Player.StopVanityActions();
            Player.RemoveAllGrapplingHooks();

            if (Player.mount.Active)
                Player.mount.Dismount(Player);
        }

        public void DisembarkFromRocket()
        {
            InRocket = false;
            IsCommander = false;

            if (Player.whoAmI == Main.myPlayer)
            {
                if (cameraModifier is not null && !cameraModifier.Finished)
                    cameraModifier.ReturnToNormalPosition = true;
            }
        }

        public override void PreUpdateMovement()
        {
            if (InRocket)
            {
                Rocket rocket = RocketManager.Rockets[RocketID];

                if (!rocket.Active)
                {
                    DisembarkFromRocket();
                    return;
                }

                if (rocket.State is Rocket.ActionState.Flight or Rocket.ActionState.Landing)
                    Player.velocity = rocket.Velocity;
                else
                    Player.velocity = Vector2.Zero;

                Player.direction = 1;
                Player.Center = new Vector2(rocket.Center.X + Player.direction * 5, rocket.Position.Y + 110) - (IsCommander ? new Vector2(0, 50) : Vector2.Zero);
                Player.noFallDmg = true;
                Player.noItems = true;

                if (Player.whoAmI == Main.myPlayer)
                {
                    cameraModifier.TargetPosition = RocketManager.Rockets[RocketID].Center - new Vector2(Main.screenWidth, Main.screenHeight) / 2f;

                    if (!rocket.Bounds.Contains(Player.Center.ToPoint()) || !rocket.ActiveInCurrentWorld)
                        DisembarkFromRocket();

                    bool escapePressed = Main.keyState.KeyPressed(Keys.Escape) && !Main.oldKeyState.KeyPressed(Keys.Escape);
                    bool dismountPressed = Player.controlMount;

                    if (rocket.State != Rocket.ActionState.Idle)
                    {
                        UISystem.Hide();
                    }
                    else if (escapePressed)
                    {
                        if (UISystem.Active)
                            UISystem.Hide();
                        else
                            DisembarkFromRocket();
                    }
                    else if (dismountPressed)
                    {
                        UISystem.Hide();
                        DisembarkFromRocket();
                    }
                }
            }
            else if (Player.whoAmI == Main.myPlayer)
            {
                if (UISystem.RocketUIActive)
                    UISystem.Hide();

                if (cameraModifier is not null)
                    cameraModifier.ReturnToNormalPosition = true;
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (InRocket && RocketManager.Rockets[RocketID].State != Rocket.ActionState.Idle)
                return false;

            return true;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (InRocket && RocketManager.Rockets[RocketID].State != Rocket.ActionState.Idle)
                return false;

            return true;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.whoAmI == Main.myPlayer && InRocket && RocketManager.Rockets[RocketID].State == Rocket.ActionState.Idle)
            {
                UISystem.Hide();
                DisembarkFromRocket();
            }
        }

        public override void PreUpdateBuffs()
        {
            if (InRocket)
            {
                Player.noItems = true;
                Player.releaseMount = true;
            }
        }

        public override void PostUpdateBuffs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && InRocket)
            {
                Player.buffImmune[ModContent.BuffType<Depressurized>()] = true;
            }
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            RocketPlayer cloneRocketPlayer = targetCopy as RocketPlayer;

            cloneRocketPlayer.InRocket = InRocket;
            cloneRocketPlayer.IsCommander = IsCommander;
            cloneRocketPlayer.RocketID = RocketID;
            cloneRocketPlayer.TargetWorld = TargetWorld;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            RocketPlayer clientRocketPlayer = clientPlayer as RocketPlayer;

            if (clientRocketPlayer.InRocket != InRocket ||
                clientRocketPlayer.IsCommander != IsCommander ||
                clientRocketPlayer.RocketID != RocketID ||
                clientRocketPlayer.TargetWorld != TargetWorld)
            {
                SyncPlayer(-1, -1, false);
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncRocketPlayer);
            packet.Write((byte)Player.whoAmI);
            packet.Write(new BitsByte(InRocket, IsCommander));
            packet.Write((byte)RocketID);
            packet.Write(TargetWorld);
            packet.Send(toWho, fromWho);
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
        {
            int playerWhoAmI = reader.ReadByte();
            RocketPlayer rocketPlayer = Main.player[playerWhoAmI].GetModPlayer<RocketPlayer>();
            BitsByte bb = reader.ReadByte();
            rocketPlayer.InRocket = bb[0];
            rocketPlayer.IsCommander = bb[1];
            rocketPlayer.RocketID = reader.ReadByte();
            rocketPlayer.TargetWorld = reader.ReadString();

            if (Main.netMode == NetmodeID.Server)
                rocketPlayer.SyncPlayer(-1, whoAmI, false);
        }

        private void On_Main_RefreshPlayerDrawOrder(On_Main.orig_RefreshPlayerDrawOrder orig, Main self)
        {
            orig(self);

            List<Player> playersThatDrawBehindNPCs = typeof(Main).GetFieldValue<List<Player>>("_playersThatDrawBehindNPCs", self, BindingFlags.NonPublic | BindingFlags.Instance);
            List<Player> playersThatDrawAfterProjectiles = typeof(Main).GetFieldValue<List<Player>>("_playersThatDrawAfterProjectiles", self, BindingFlags.NonPublic | BindingFlags.Instance);

            Player player;
            RocketPlayer rocketPlayer;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                player = Main.player[i];
                if (i != Main.myPlayer &&
                    player.active &&
                    !player.outOfRange &&
                    playersThatDrawAfterProjectiles.Contains(player) &&
                    player.TryGetModPlayer(out rocketPlayer) &&
                    rocketPlayer.InRocket)
                {
                    playersThatDrawAfterProjectiles.Remove(player);
                    playersThatDrawBehindNPCs.Add(player);
                }
            }

            player = Main.LocalPlayer;
            if (playersThatDrawAfterProjectiles.Contains(player) &&
                player.TryGetModPlayer(out rocketPlayer) &&
                rocketPlayer.InRocket)
            {
                playersThatDrawAfterProjectiles.Remove(player);
                playersThatDrawBehindNPCs.Add(player);
            }
        }
    }
}

