using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public class LaunchPadManager : ModSystem
    {
        private static Dictionary<string, List<LaunchPad>> launchPadStorage;

        public override void Load()
        {
            launchPadStorage = new Dictionary<string, List<LaunchPad>>();
        }

        public override void Unload()
        {
            launchPadStorage.Clear();
            launchPadStorage = null;
        }

        public static void Add(string subworldId, LaunchPad launchPad)
        {
            if (launchPadStorage.ContainsKey(subworldId))
            {
                launchPadStorage[subworldId].Add(launchPad);
            }
            else
            {
                launchPadStorage.Add(subworldId, new() { launchPad });
            }
        }

        public static void Remove(string subworldId, LaunchPad launchPad, bool shouldSync = true)
        {
            if (launchPadStorage.ContainsKey(subworldId))
            {
                var toRemove = GetLaunchPadAtStartTile(subworldId, launchPad.StartTile);

                if (toRemove is not null)
                {
                    toRemove.Active = false;

                    if (shouldSync)
                        toRemove.NetSync(subworldId);
                }
            }
        }

        public static void ClearAllLaunchPads(bool announce = true, bool shouldSync = true)
        {
            if (announce)
                Utility.Chat("Cleared all launch pads!", Color.Green);

            foreach (var lpKvp in launchPadStorage)
            {
                foreach (var lp in lpKvp.Value)
                {
                    lp.Active = false;

                    if (shouldSync)
                        lp.NetSync(lpKvp.Key);
                }
            }
        }

        public static bool Any(string subworldId) => GetLaunchPads(subworldId).Any();
        public static bool None(string subworldId) => !Any(subworldId);


        public static List<LaunchPad> GetLaunchPads(string subworldId)
        {
            if (launchPadStorage.ContainsKey(subworldId))
                return launchPadStorage[subworldId];

            return new List<LaunchPad>();
        }

        public static LaunchPad GetLaunchPadAtTileCoordinates(string subworldId, Point16 tile)
        {
            return GetLaunchPads(subworldId).FirstOrDefault(lp =>
            {
                Rectangle coordinates = new(lp.StartTile.X, lp.StartTile.Y, lp.EndTile.X - lp.StartTile.X + 2, lp.EndTile.Y - lp.StartTile.Y + 2);
                return coordinates.Contains(tile.X, tile.Y);
            });
        }

        public static bool TryGetLaunchPadAtTileCoordinates(string subworldId, Point16 tile, out LaunchPad launchPad)
        {
            launchPad = GetLaunchPadAtTileCoordinates(subworldId, tile);
            return launchPad != null;
        }

        public static LaunchPad GetLaunchPadAtStartTile(string subworldId, Point16 startTile)
            => GetLaunchPads(subworldId).FirstOrDefault(lp => lp.StartTile == startTile);

        public static bool TryGetLaunchPadAtStartTile(string subworldId, Point16 startTile, out LaunchPad launchPad)
        {
            launchPad = GetLaunchPadAtStartTile(subworldId, startTile);
            return launchPad != null;
        }

        private int checkTimer;
        public override void PostUpdateNPCs()
        {
            UpdateLaunchPads();
        }

        private void UpdateLaunchPads()
        {
            checkTimer++;

            if (checkTimer >= 10)
            {
                checkTimer = 0;

                if (launchPadStorage.ContainsKey(MacrocosmSubworld.CurrentID))
                {
                    for (int i = 0; i < launchPadStorage[MacrocosmSubworld.CurrentID].Count; i++)
                    {
                        var launchPad = launchPadStorage[MacrocosmSubworld.CurrentID][i];

                        if (!launchPad.Active)
                        {
                            launchPadStorage[MacrocosmSubworld.CurrentID].RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            launchPad.Update();
                        }
                    }
                }
            }
        }

        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            if (launchPadStorage.ContainsKey(MacrocosmSubworld.CurrentID))
                foreach (LaunchPad launchPad in launchPadStorage[MacrocosmSubworld.CurrentID])
                    launchPad.Draw(Main.spriteBatch, Main.screenPosition);

            Main.spriteBatch.End();
        }

        public override void ClearWorld()
        {
            launchPadStorage.Clear();
        }

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                if (launchPadStorage.ContainsKey(MacrocosmSubworld.CurrentID))
                {
                    foreach (var launchPad in launchPadStorage[MacrocosmSubworld.CurrentID])
                    {
                        launchPad.NetSync(MacrocosmSubworld.CurrentID, toClient: remoteClient);
                    }
                }
            }

            return false;
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);

        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void SaveData(TagCompound tag)
        {
            TagCompound launchPads = new();

            foreach (var kvp in launchPadStorage)
                launchPads[kvp.Key] = kvp.Value;

            tag[nameof(launchPadStorage)] = launchPads;
        }

        public static void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(launchPadStorage)))
            {
                TagCompound launchPads = tag.GetCompound(nameof(launchPadStorage));

                foreach (var kvp in launchPads)
                    launchPadStorage[kvp.Key] = (List<LaunchPad>)launchPads.GetList<LaunchPad>(kvp.Key);
            }
        }
    }
}
