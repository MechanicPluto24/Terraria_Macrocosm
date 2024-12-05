using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace Macrocosm.Content.Rockets
{
    public enum RocketDrawLayer
    {
        BeforeProjectiles,
        AfterProjectiles,
        BeforeNPCs,
        AfterNPCs
    }

    public class RocketManager : ModSystem
    {
        public static Rocket[] Rockets { get; private set; }

        public const int MaxRockets = 256;

        public static bool DebugModeActive = false;

        public override void Load()
        {
            On_Main.DoDraw_DrawNPCsBehindTiles += On_Main_DoDraw_DrawNPCsBehindTiles;
            On_Main.DrawProjectiles += DrawRockets_Projectiles;
            On_Main.DrawNPCs += DrawRockets_NPCs;
            On_Main.DrawDust += DrawRockets_OverlaysAfterDusts;
        }

        public override void OnModLoad()
        {
            Rockets = new Rocket[MaxRockets];
            for (int i = 0; i < MaxRockets; i++)
                Rockets[i] = new Rocket();
        }

        public override void Unload()
        {
            Rockets = null;

            On_Main.DoDraw_DrawNPCsBehindTiles -= On_Main_DoDraw_DrawNPCsBehindTiles;
            On_Main.DrawProjectiles -= DrawRockets_Projectiles;
            On_Main.DrawNPCs -= DrawRockets_NPCs;
            On_Main.DrawDust -= DrawRockets_OverlaysAfterDusts;
        }

        public static int ActiveRocketCount => Rockets.Count(rocket => rocket.Active);
        public static int InCurrentSubworldRocketCount => Rockets.Count(rocket => rocket.ActiveInCurrentWorld);

        public static void AddRocket(Rocket rocket)
        {
            if (ActiveRocketCount >= MaxRockets)
                return;

            int index = Rockets.Select((rocket, idx) => new { rocket, idx })
                                    .Where(item => !item.rocket.Active)
                                    .Select(item => item.idx)
                                    .FirstOrDefault();

            rocket.WhoAmI = index;
            Rockets[index] = rocket;
        }

        public override void PostUpdateNPCs()
        {
            UpdateRockets();
        }

        private static void UpdateRockets()
        {
            for (int i = 0; i < MaxRockets; i++)
            {
                Rocket rocket = Rockets[i];

                if (!rocket.ActiveInCurrentWorld)
                    continue;

                rocket.Update();
            }
        }

        public static void DespawnAllRockets(bool announce = true)
        {
            if (announce)
                Utility.Chat("Despawned all rockets from this subworld!", Color.Green);

            for (int i = 0; i < MaxRockets; i++)
            {
                Rocket rocket = Rockets[i];
                if (rocket.ActiveInCurrentWorld)
                {
                    rocket.Despawn();
                }
            }
        }


        private static void PreDrawBeforeTiles()
        {
            for (int i = 0; i < MaxRockets; i++)
            {
                Rocket rocket = Rockets[i];

                if (!rocket.ActiveInCurrentWorld)
                    continue;

                rocket.PreDrawBeforeTiles(Main.spriteBatch, rocket.Position - Main.screenPosition, inWorld: true);
            }
        }

        private static void DrawRockets(RocketDrawLayer layer)
        {
            for (int i = 0; i < MaxRockets; i++)
            {
                Rocket rocket = Rockets[i];

                if (!rocket.ActiveInCurrentWorld)
                    continue;

                if (rocket.DrawLayer != layer)
                    continue;

                rocket.Draw(Rocket.DrawMode.World, Main.spriteBatch, rocket.Position - Main.screenPosition, useRenderTarget: true);

                if (DebugModeActive)
                {
                    rocket.DisplayWhoAmI();
                    rocket.DrawDebugModuleHitbox();
                }
            }

            if (DebugModeActive)
            {
                string text = $"Rockets active: {Rockets.Where(r => r.Active).Count()}\nHere: {Rockets.Where(r => r.ActiveInCurrentWorld).Count()}";
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, text, new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.14f), Color.White, 0f, Vector2.Zero, Vector2.One * 0.5f);
            }
        }

        private static void PostDrawRockets(RocketDrawLayer layer)
        {
            for (int i = 0; i < MaxRockets; i++)
            {
                Rocket rocket = Rockets[i];

                if (!rocket.ActiveInCurrentWorld)
                    continue;

                if (rocket.DrawLayer != layer)
                    continue;

                rocket.PostDraw(Main.spriteBatch, rocket.Position - Main.screenPosition, inWorld: true);
            }
        }

        private static void DrawOverlays()
        {
            for (int i = 0; i < MaxRockets; i++)
            {
                Rocket rocket = Rockets[i];

                if (!rocket.ActiveInCurrentWorld)
                    continue;

                rocket.DrawOverlay(Main.spriteBatch, rocket.Position - Main.screenPosition);
            }
        }

        public override void ClearWorld()
        {
            for (int i = 0; i < MaxRockets; i++)
                Rockets[i] = new Rocket();
        }

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                for (int i = 0; i < MaxRockets; i++)
                {
                    var rocket = Rockets[i];
                    rocket.NetSync(toClient: remoteClient);
                    rocket.SendCustomizationData(toClient: remoteClient);
                    rocket.Inventory.SyncEverything(toClient: remoteClient);
                }
            }

            return false;
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);
        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void SaveData(TagCompound dataCopyTag)
        {
            dataCopyTag[nameof(Rockets)] = Rockets;
        }

        public static void LoadData(TagCompound dataCopyTag)
        {
            if (dataCopyTag.ContainsKey(nameof(Rockets)))
            {
                // This is for future-proofing regarding array size
                var rockets = dataCopyTag.Get<Rocket[]>(nameof(Rockets));
                Array.Copy(rockets, Rockets, rockets.Length);
            }
        }

        private static SpriteBatchState state1, state2;

        private void On_Main_DoDraw_DrawNPCsBehindTiles(On_Main.orig_DoDraw_DrawNPCsBehindTiles orig, Main self)
        {
            orig(self);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);
            PreDrawBeforeTiles();
            Main.spriteBatch.End();
        }

        private void DrawRockets_NPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            state1.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            DrawRockets(RocketDrawLayer.BeforeNPCs);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            PostDrawRockets(RocketDrawLayer.BeforeNPCs);

            spriteBatch.End();
            spriteBatch.Begin(state1);

            orig(self, behindTiles);

            state2.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            DrawRockets(RocketDrawLayer.AfterNPCs);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            PostDrawRockets(RocketDrawLayer.AfterNPCs);

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        private void DrawRockets_Projectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            DrawRockets(RocketDrawLayer.BeforeProjectiles);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            PostDrawRockets(RocketDrawLayer.BeforeProjectiles);

            spriteBatch.End();

            orig(self);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            DrawRockets(RocketDrawLayer.AfterProjectiles);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, default, Main.Transform);

            PostDrawRockets(RocketDrawLayer.AfterProjectiles);

            spriteBatch.End();
        }

        private void DrawRockets_OverlaysAfterDusts(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, default, RasterizerState.CullNone, default, Main.Transform);
            DrawOverlays();
            Main.spriteBatch.End();
        }
    }
}
