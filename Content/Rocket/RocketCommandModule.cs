using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Buffs.GoodBuffs.MountBuffs;
using Macrocosm.Content.Rocket.UI;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Rocket
{
	public class RocketCommandModuleTile : ModTile
	{
		const int width = 6;
		const int height = 6;

		public override void SetStaticDefaults()
		{	
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			MinPick = 1000;

			DustType = -1;
			HitSound = SoundID.MenuClose; // huh

			int[] heights = new int[6];
			Array.Fill<int>(heights, 16);

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.Width = width;
			TileObjectData.newTile.Height = height;
			//TileObjectData.newTile.AnchorValidTiles = { ModContent.TileType<RocketServiceModule> };
			TileObjectData.newTile.CoordinateHeights = heights;
			TileObjectData.newTile.Origin = new Point16(0, height - 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, width, 0);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<RocketCommandModuleTileEntity>().Hook_AfterPlacement, -1, 0, false);

			TileObjectData.addTile(Type);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Rocket");

			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override bool CanPlace(int i, int j)
			=> true;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Point16 origin = TileUtils.GetTileOrigin(i, j);
			ModContent.GetInstance<RocketCommandModuleTileEntity>().Kill(origin.X, origin.Y);
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;

			//Should your tile entity bring up a UI, this line is useful to prevent item slots from misbehaving
			Main.mouseRightRelease = false;

			//The following four (4) if-blocks are recommended to be used if your multitile opens a UI when right clicked:
			if (player.sign > -1)
			{
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = string.Empty;
			}
			if (Main.editChest)
			{
				Main.editChest = false;
				Main.npcChatText = string.Empty;
			}
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}
			if (player.talkNPC > -1)
			{
				player.SetTalkNPC(-1);
				Main.npcChatCornerItem = 0;
				Main.npcChatText = string.Empty;
			}

			if (TileUtils.TryGetTileEntityAs(i, j, out RocketCommandModuleTileEntity entity) && player.TryGetModPlayer(out RocketPlayer rocketPlayer))
			{
				RocketUI.Show(new Point16(i, j));
				return true;
			}

			return false;
		}

		public static void Launch(string subworldName, Point16 tePos)
		{
			if (TileUtils.TryGetTileEntityAs(tePos.X, tePos.Y, out RocketCommandModuleTileEntity entity) && Main.LocalPlayer.TryGetModPlayer(out RocketPlayer rocketPlayer))
			{
				rocketPlayer.TargetSubworldID = subworldName;
				rocketPlayer.InRocket = true;
				int rocketId = NPC.NewNPC(null, (entity.Position.X + width / 2) * 16, (entity.Position.Y + height) * 16, ModContent.NPCType<Rocket>(), ai0: rocketPlayer.Player.whoAmI);
				Main.npc[rocketId].position.Y -= 3.5f;
				Main.npc[rocketId].position.X += 2f;
				entity.Kill(entity.Position.X, entity.Position.Y);
				WorldGen.KillTile(entity.Position.X, entity.Position.Y);
			}
		}
	}

	public class RocketCommandModuleTileEntity : ModTileEntity
	{


		public override void PreGlobalUpdate()
		{
			bool found = false;
			
			foreach (ModTileEntity entity in manager.EnumerateEntities().Values.OfType<ModTileEntity>())
			{
				if (entity is RocketCommandModuleTileEntity)
				{
					if (found)
					{
						entity.Kill(entity.Position.X, entity.Position.Y);
						WorldGen.KillTile(entity.Position.X, entity.Position.Y);
					}
					else
					{
						found = true;
					}
				}
			}
		}

		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];

			return tile.HasTile && tile.TileType == ModContent.TileType<RocketCommandModuleTile>();
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				//Sync the entire multitile's area. 
				int width = 6;
				int height = 6;
				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

				//Sync the placement of the tile entity with other clients
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
			}

			//ModTileEntity.Place() handles checking if the entity can be placed, then places it
			Point16 tileOrigin = new Point16(0, 5);
			int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
			return placedEntity;
		}

		public override void OnNetPlace()
		{
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
		}

		public override void OnKill()
		{
		}
	}
}
