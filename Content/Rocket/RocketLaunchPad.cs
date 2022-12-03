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
	public class RocketLaunchPad : ModTile
	{
		const int width = 6;
		const int height = 1;

		public override void SetStaticDefaults()
		{	
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileNoAttach[Type] = true;
			MinPick = 1000;

			DustType = -1;
			HitSound = SoundID.MenuClose; // huh

			//int[] heights = new int[6];
			//Array.Fill(heights, 16);

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.Width = width;
			TileObjectData.newTile.Height = height;
			//TileObjectData.newTile.AnchorValidTiles = { ModContent.TileType<RocketServiceModule> };
			//TileObjectData.newTile.CoordinateHeights = heights;
			TileObjectData.newTile.Origin = new Point16(0, height - 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, width, 0);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<RocketLaunchPadTE>().Hook_AfterPlacement, -1, 0, false);

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
			ModContent.GetInstance<RocketLaunchPadTE>().Kill(origin.X, origin.Y);
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

			if (TileUtils.TryGetTileEntityAs(i, j, out RocketLaunchPadTE entity) && player.TryGetModPlayer(out RocketPlayer rocketPlayer))
			{
				//RocketSystem.Instance.ShowBuildingUI(new Point16(i, j));
				//return true;
			}

			return false;
		}
	}

	public class RocketLaunchPadTE : ModTileEntity
	{
		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.HasTile && tile.TileType == ModContent.TileType<RocketLaunchPad>();
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style, alternate);
			int topLeftX = i - tileData.Origin.X;
			int topLeftY = j - tileData.Origin.Y;

			int rocketNpcWidth = 139;
			int rocketNpcHeight = 470;
			int rocketID = NPC.NewNPC(new EntitySource_TileEntity(this), (int)(topLeftX * 16f), (int)(topLeftY * 16f - rocketNpcHeight), ModContent.NPCType<Rocket>());

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				//Sync the entire multitile's area. 
				NetMessage.SendTileSquare(Main.myPlayer, topLeftX, topLeftY, tileData.Width, tileData.Height);

				//Sync the placement of the tile entity with other clients
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, topLeftX, topLeftY, Type);

				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, rocketID);

				return -1;
			}

			//ModTileEntity.Place() handles checking if the entity can be placed, then places it
			int placedEntity = Place(topLeftX, topLeftY);
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
