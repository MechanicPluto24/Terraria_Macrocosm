using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using SubworldLibrary;
using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using System.IO;
using Macrocosm.Content.Players;
using Terraria.ID;
using Macrocosm.Common.Base;
using Macrocosm.Content.Rocket;

namespace Macrocosm
{
	public enum MessageType : byte
	{
		SyncNPCTargeting,
		SyncDashDirection,
		EmbarkPlayerInRocket,
		LaunchRocket,
		SyncPlayerRocketStatus
	}

	public class Macrocosm : Mod
	{
		public static Mod Instance => ModContent.GetInstance<Macrocosm>();

		public const string EffectAssetPath = "Macrocosm/Content/Effects/";

		public override void Load()
		{
			ModCalls();
			LoadEffects();
		}

		private static void LoadEffects()
		{
			Filters.Scene["Macrocosm:RadiationNoiseEffect"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>(EffectAssetPath + "RadiationNoiseEffect", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "RadiationNoiseEffect"));
			Filters.Scene["Macrocosm:RadiationNoiseEffect"].Load();
		}

		private void ModCalls()
		{
			#region Ryan's mods calls

			if (ModLoader.TryGetMod("TerrariaAmbience", out Mod ta))
				ta.Call("AddTilesToList", null, "Stone", Array.Empty<string>(), new int[]
				{
					ModContent.TileType<Regolith>(),
					ModContent.TileType<Protolith>()
				});

			if (ModLoader.TryGetMod("TerrariaAmbienceAPI", out Mod taAPI))
				taAPI.Call("Ambience", this, "MoonAmbience", "Assets/Sounds/Ambient/Moon", 1f, 0.0075f, new Func<bool>(SubworldSystem.IsActive<Moon>));

			#endregion
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType messageType = (MessageType)reader.ReadByte();

			switch (messageType)
			{
				case MessageType.SyncDashDirection:
					{ 
						int dashPlayerID = reader.ReadByte();
						DashPlayer dashPlayer= Main.player[dashPlayerID].GetModPlayer<DashPlayer>();

						int newDir = reader.ReadByte();
						dashPlayer.DashDirection = (DashPlayer.DashDir)newDir;

						if (Main.netMode == NetmodeID.Server)
							dashPlayer.SyncPlayer(-1, whoAmI, false);

						break;
					}

				case MessageType.EmbarkPlayerInRocket:
					{
						int playerId = reader.ReadByte();
						bool asCommander = reader.ReadBoolean();
						int rocketId = reader.ReadByte();

						if (Main.netMode == NetmodeID.Server)
							(Main.npc[rocketId].ModNPC as RocketNPC).ReceiveEmbarkedPlayer(playerId, asCommander, rocketId);

						break;
					}

				case MessageType.LaunchRocket:
					{
						int rocketId = reader.ReadByte();
						(Main.npc[rocketId].ModNPC as RocketNPC).Launch();

						break;
					}

				case MessageType.SyncPlayerRocketStatus:
					{
						int rocketPlayerID = reader.ReadByte();
						RocketPlayer rocketPlayer = Main.player[rocketPlayerID].GetModPlayer<RocketPlayer>();
						BitsByte bb = reader.ReadByte();
						rocketPlayer.InRocket = bb[0];
						rocketPlayer.AsCommander = bb[1];
						rocketPlayer.RocketID = reader.ReadByte();
						rocketPlayer.TargetSubworldID = reader.ReadString();

						if (Main.netMode == NetmodeID.Server)
							rocketPlayer.SyncPlayer(-1, whoAmI, false);

						break;
					}

				default:
					Logger.WarnFormat("Macrocosm: Unknown Message type: {messageType}", messageType);
					break;
			}
		}
	}
}