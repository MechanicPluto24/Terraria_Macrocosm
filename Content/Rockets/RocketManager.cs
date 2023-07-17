using System.Collections.Generic;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets
{
    public enum RocketDrawLayer
    {
        BeforeProjectiles,
        AfterProjectiles,
        BeforeNPCs,
        AfterNPCs
    }

    public class RocketManager : ModSystem, ICopyWorldData
    {
        public static List<Rocket> Rockets { get; private set; }

		public const int MaxRockets = byte.MaxValue;

		public override void Load()
        {
			Rockets = new List<Rocket>();

            On_Main.DrawProjectiles += DrawRocket_Projectiles;
            On_Main.DrawNPCs += DrawRocket_NPCs;
        }

        public override void Unload()
        {
			Rockets = null;

            On_Main.DrawProjectiles -= DrawRocket_Projectiles;
            On_Main.DrawNPCs -= DrawRocket_NPCs;
        }

		public override void PostUpdateNPCs()
		{
            UpdateRockets();
		}

        private static void UpdateRockets()
        {
            for (int i = 0; i < Rockets.Count; i++)
            {
                Rocket rocket = Rockets[i];

				if (!rocket.Active)
				{
					Rockets.RemoveAt(i);
					i--;
				}

                if (rocket.CurrentWorld != MacrocosmSubworld.CurrentSubworld)
                    continue;

				rocket.Update();
            }
        }

        public static void DespawnAllRockets()
        {
			foreach (Rocket rocket in Rockets)
				rocket.Despawn();
        }

        private static void DrawRockets(RocketDrawLayer layer)
        {
            foreach (Rocket rocket in Rockets)
            {
				if (rocket.CurrentWorld != MacrocosmSubworld.CurrentSubworld)
					continue;

				if (rocket.DrawLayer == layer)
                {
                    Color lightColor = Lighting.GetColor((int)(rocket.Center.X / 16), (int)(rocket.Center.Y / 16));
                    rocket.Draw(Main.spriteBatch, Main.screenPosition, lightColor);
                }
            }
        }

        public override void OnWorldLoad() => Rockets.Clear();

		public override void OnWorldUnload() => Rockets.Clear();

		public override void SaveWorldData(TagCompound tag)
		{
            for(int i = 0; i < MaxRockets; i++)
            {
                if (Rockets[i].Active)
                {

                }
            }
		}

		public override void LoadWorldData(TagCompound tag)
		{
			base.LoadWorldData(tag);
		}

		public void CopyRocketData()
        {
        
        }

		public void ReadCopiedRocketData()
		{

		}

		private void DrawRocket_NPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            var state1 = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            
            DrawRockets(RocketDrawLayer.BeforeNPCs);

            spriteBatch.End();
            spriteBatch.Begin(state1);

            orig(self, behindTiles);

			var state2 = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            
            DrawRockets(RocketDrawLayer.AfterNPCs);

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        private void DrawRocket_Projectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            
            DrawRockets(RocketDrawLayer.BeforeProjectiles);
            
            spriteBatch.End();

            orig(self);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.ZoomMatrix);
           
            DrawRockets(RocketDrawLayer.AfterProjectiles);
            
            spriteBatch.End();
        }

    }
}
