using Macrocosm.Common.Netcode;
using Macrocosm.Content.Players;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Subworlds
{
    class MacrocosmWorld : ModSystem
    {
        /// <summary> Whether the dusk time boundary happened in this update tick </summary>
        public static bool IsDusk { get; set; } = false;

        /// <summary> Whether the dawn time boundary happened in this update tick </summary>
        public static bool IsDawn { get; set; } = false;

        public static int Seed => Main.ActiveWorldFileData.Seed;
        public static string SeedText => Main.ActiveWorldFileData.SeedText;

        public override void Load()
        {
            On_UIWorldListItem.DrawSelf += DrawWorldListPlanetIcons;
        }

        public override void Unload()
        {
            On_UIWorldListItem.DrawSelf -= DrawWorldListPlanetIcons;
        }

        public override void OnWorldLoad()
        {
            if (!SubworldSystem.AnyActive())
                Earth.WorldSize = WorldSize.Current;
        }

        public override void PostWorldGen()
        {
        }

        public override void PreUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                MacrocosmSubworld activeSubworld = MacrocosmSubworld.Current;

                SubworldSystem.hideUnderworld = true;
                SubworldSystem.noReturn = false;

                UpdateTime(activeSubworld);

                activeSubworld.PreUpdateWorld();

                GameMechanicsUpdates();
                FreezeEnvironment();
            }
        }

        // Updates the time 
        private static void UpdateTime(MacrocosmSubworld subworld)
        {
            double timeRate = subworld.TimeRate;

            // Fast forward 60 times if using sun/moon-dials
            if (Main.IsFastForwardingTime())
            {
                timeRate *= 60.0;
                Main.desiredWorldTilesUpdateRate = timeRate / 60.0;
                Main.desiredWorldEventsUpdateRate = timeRate;
            }

            // Apply current journey power time modifier
            timeRate *= CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;

            // Apply all players sleeping multiplier 
            if (Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0)
                timeRate *= 5;

            // Don't pass time if disabled from the journey powers 
            if (CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled)
                timeRate = 0;

            Main.time += timeRate;
            Main.desiredWorldTilesUpdateRate = timeRate / 60.0;
            Main.desiredWorldEventsUpdateRate = timeRate;

            IsDusk = Main.dayTime && Main.time >= subworld.DayLenght;
            IsDawn = !Main.dayTime && Main.time >= subworld.NightLenght;

            if (IsDusk)
            {
                Main.time = 0;
                Main.dayTime = false;

                if (Main.fastForwardTimeToDusk)
                    Main.fastForwardTimeToDusk = false;
            }

            if (IsDawn)
            {
                Main.time = 0;
                Main.dayTime = true;

                if (Main.fastForwardTimeToDawn)
                    Main.fastForwardTimeToDawn = false;
            }
        }

        // Updates wiring, TEs and liquids 
        private static void GameMechanicsUpdates()
        {
            if (MacrocosmSubworld.Current.ShouldUpdateWiring)
                Wiring.UpdateMech();

            TileEntity.UpdateStart();
            foreach (TileEntity te in TileEntity.ByID.Values)
            {
                te.Update();
            }
            TileEntity.UpdateEnd();

            if (++Liquid.skipCount > 1)
            {
                Liquid.UpdateLiquid();
                Liquid.skipCount = 0;
            }
        }

        // Freezes environment factors like rain or clouds. 
        // Required when NormalUpdates are turned on (if we ever want that), and as failsafe if something is still updated with them turned off.
        private static void FreezeEnvironment()
        {
            //if (Main.gameMenu)
            //	return;

            Main.numClouds = 0;
            Main.windSpeedCurrent = 0;
            Main.weatherCounter = 0;

            // Tricky way to stop vanilla fallen stars for spawning when NormalUpdates are turned on 
            Star.starfallBoost = 0;

            Main.slimeRain = false;
            Main.slimeRainTime = 0;

            Main.StopSlimeRain(false);

            LanternNight.WorldClear();
            Main.StopRain(); // Rain, rain, go away, come again another day
        }

        private void DrawWorldListPlanetIcons(On_UIWorldListItem.orig_DrawSelf orig, UIWorldListItem uIItem, SpriteBatch spriteBatch)
        {
            orig(uIItem, spriteBatch);

            var player = Main.LocalPlayer.GetModPlayer<MacrocosmPlayer>();
            string subworldId = "Earth";
            Texture2D texture = Macrocosm.EmptyTex;

            if (player.TryGetReturnSubworld(uIItem.Data.UniqueId, out string id))
                subworldId = id;
 
            if (ModContent.RequestIfExists<Texture2D>(Macrocosm.TextureAssetsPath + "Icons/" + subworldId, out var asset))
                texture = asset.Value;

            var dims = uIItem.GetOuterDimensions();
            var pos = new Vector2(dims.X + texture.Width + 102, dims.Y + dims.Height - texture.Height + 1);

            Rectangle bounds = new((int)pos.X, (int)pos.Y, texture.Width, texture.Height);
            spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

            if (bounds.Contains(Main.mouseX, Main.mouseY))
                Main.instance.MouseText(Language.GetTextValue("Mods.Macrocosm.Subworlds." + subworldId + ".DisplayName"));
        }

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.LastSubworldCheck);
                Guid guid = SubworldSystem.AnyActive<Macrocosm>() ? MacrocosmSubworld.Current.MainWorldUniqueId : Main.ActiveWorldFileData.UniqueId;
                packet.Write(guid.ToString());
                packet.Send(remoteClient);
            }

            return false;
        }

        public override void PreUpdateEntities()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateEntities();
        }

        public override void PreUpdatePlayers()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdatePlayers();
        }

        public override void PostUpdatePlayers()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdatePlayers();
        }

        public override void PreUpdateNPCs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateNPCs();
        }

        public override void PostUpdateNPCs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateNPCs();
        }

        public override void PreUpdateGores()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateGores();
        }

        public override void PostUpdateGores()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateGores();
        }

        public override void PreUpdateProjectiles()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateProjectiles();
        }

        public override void PostUpdateProjectiles()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateProjectiles();
        }

        public override void PreUpdateItems()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateItems();
        }

        public override void PostUpdateItems()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateItems();
        }

        public override void PreUpdateDusts()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateDusts();
        }

        public override void PostUpdateDusts()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateDusts();
        }

        public override void PreUpdateTime()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateTime();
        }

        public override void PostUpdateTime()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateTime();
        }

        public override void PostUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateWorld();
        }

        public override void PreUpdateInvasions()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateInvasions();
        }

        public override void PostUpdateInvasions()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateInvasions();
        }

        public override void PostUpdateEverything()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateEverything();
        }
    }
}
