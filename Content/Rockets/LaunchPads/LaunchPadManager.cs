using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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

		// TODO: add optional netsync
		public static void Add(string subworldId, LaunchPad launchPad)
		{
			if (launchPadStorage.ContainsKey(subworldId))
			{
				launchPadStorage[subworldId].Add(launchPad);
			}
			else
			{
				List<LaunchPad> launchPadsList = new() { launchPad };
				launchPadStorage.Add(subworldId, launchPadsList);
			}
		}

		// TODO: add optional netsync
		public static void Remove(string subworldId, LaunchPad launchPad)
		{
			if (launchPadStorage.ContainsKey(subworldId))
 				launchPadStorage[subworldId].Remove(launchPad);
 		}

		public static bool Any(string subworldId) => GetLaunchPads(subworldId).Any();
		public static bool None(string subworldId) => !Any(subworldId);


		public static List<LaunchPad> GetLaunchPads(string subworldId)
		{
			if (launchPadStorage.ContainsKey(subworldId))
				return launchPadStorage[subworldId];

			return new List<LaunchPad>();
		}


		public static LaunchPad GetLaunchPadAtTileCoordinates(string subworldId, int startTileX, int startTileY)
			=> GetLaunchPadAtTileCoordinates(subworldId,new(startTileX, startTileY));
		public static LaunchPad GetLaunchPadAtTileCoordinates(string subworldId, Point16 startTile)
			=> GetLaunchPads(subworldId).FirstOrDefault(lp => lp.StartTile == startTile);

		private int checkTimer;

		public override void PostUpdateNPCs()
		{
			checkTimer++;

			if (checkTimer >= 10)
			{
				checkTimer = 0;

				if (launchPadStorage.ContainsKey(MacrocosmSubworld.CurrentWorld))
					foreach (LaunchPad launchPad in launchPadStorage[MacrocosmSubworld.CurrentWorld])
						launchPad.Update();
			}
		}

		public override void PostDrawTiles()
		{
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.ZoomMatrix);

			if (launchPadStorage.ContainsKey(MacrocosmSubworld.CurrentWorld))
 				foreach (LaunchPad launchPad in launchPadStorage[MacrocosmSubworld.CurrentWorld])
 					launchPad.Draw(Main.spriteBatch, Main.screenPosition);
 
			Main.spriteBatch.End();
		}

		public override void ClearWorld()
		{
			launchPadStorage.Clear();
		}

		public override void SaveWorldData(TagCompound tag) => SaveLaunchPads(tag);

		public override void LoadWorldData(TagCompound tag) => LoadLaunchPads(tag);
			
		public static void SaveLaunchPads(TagCompound tag)
		{
			foreach (var lpKvp in launchPadStorage)
				tag[lpKvp.Key] = lpKvp.Value;
		}

		public static void LoadLaunchPads(TagCompound tag)
		{
			foreach (var lpKvp in launchPadStorage)
				if (tag.ContainsKey(lpKvp.Key))
					launchPadStorage[lpKvp.Key] = (List<LaunchPad>)tag.GetList<LaunchPad>(lpKvp.Key);
 		}
	}
}
