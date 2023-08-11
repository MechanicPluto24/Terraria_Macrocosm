using System;
using System.Linq;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    public class RocketManager : ModSystem
    {
        public static Rocket[] Rockets { get; private set; }

		public const int MaxRockets = byte.MaxValue;

		public override void Load()
        {
            Rockets = new Rocket[MaxRockets];

            On_Main.DrawProjectiles += DrawRocket_Projectiles;
            On_Main.DrawNPCs += DrawRocket_NPCs;
        }

        public override void Unload()
        {
			Rockets = null;

            On_Main.DrawProjectiles -= DrawRocket_Projectiles;
            On_Main.DrawNPCs -= DrawRocket_NPCs;
        }

		public static int ActiveRocketCount => Rockets.Count(rocket => rocket.Active);
		public static int RocketsInCurrentSubworld => Rockets.Count(rocket => rocket.ActiveInCurrentSubworld);

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
            Utility.Chat(ActiveRocketCount + ", " + RocketsInCurrentSubworld);
            UpdateRockets();
		}

        private static void UpdateRockets()
        {
            for (int i = 0; i < MaxRockets; i++)
            {
                Rocket rocket = Rockets[i];

                if (!rocket.ActiveInCurrentSubworld)
                    continue;

				rocket.Update();
            }
        }

        public static void DespawnAllRockets()
        {
			for (int i = 0; i < MaxRockets; i++)
                Rockets[i].Despawn();
        }

        private static void DrawRockets(RocketDrawLayer layer)
        {
			for (int i = 0; i < MaxRockets; i++)
			{
                Rocket rocket = Rockets[i];

                if (!rocket.ActiveInCurrentSubworld)
                    continue;

				if (rocket.DrawLayer == layer)
                {
                    Color lightColor = Lighting.GetColor((int)(rocket.Center.X / 16), (int)(rocket.Center.Y / 16));
                    rocket.Draw(Main.spriteBatch, Main.screenPosition, lightColor);
                }
            }
        }

		public override void ClearWorld()
		{
            Array.Fill(Rockets, new Rocket());
		}

		public override void SaveWorldData(TagCompound tag) => SaveRocketData(tag); 
        public override void LoadWorldData(TagCompound tag) => ReadSavedRocketData(tag);

        public static void SaveRocketData(TagCompound dataCopyTag) 
        {
			dataCopyTag[nameof(Rockets)] = Rockets;
		}

        public static void ReadSavedRocketData(TagCompound dataCopyTag) 
        {
			if (dataCopyTag.ContainsKey(nameof(Rockets)))
				Rockets = dataCopyTag.Get<Rocket[]>(nameof(Rockets));

            OnWorldSpawn();
		}

		private static void OnWorldSpawn()
		{
			for (int i = 0; i < MaxRockets; i++)
			{
				Rocket rocket = Rockets[i];

				if (!rocket.ActiveInCurrentSubworld)
					continue;

				rocket.OnWorldSpawn();
			}
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
