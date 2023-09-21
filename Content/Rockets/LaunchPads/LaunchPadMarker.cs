using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Rockets.LaunchPads
{
	public enum MarkerState
	{
		Invalid,
		Occupied,
		Vacant,
		Inactive
	};

	public class LaunchPadMarker : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
			Main.tileLighted[Type] = true;

            DustType = -1;
            HitSound = SoundID.Mech;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			//TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<LaunchPadMarkerTE>().Hook_AfterPlacement, -1, 0, false);

			TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(200, 200, 200), name);
        }

		public override bool CanPlace(int i, int j) 
		{
			Main.NewText("Called");
			return LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentWorld, new(i, j), out _);
		} 

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			ModContent.GetInstance<LaunchPadMarkerTE>().Kill(i, j);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Utility.DrawTileGlowmask(i, j, spriteBatch, ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/LaunchPads/LaunchPadMarker_Glow").Value, Color.White);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX <= (int)MarkerState.Inactive * 18)
			{
				switch (tile.TileFrameX / 18)
				{
					case ((int)MarkerState.Invalid):
						r = 255f / 255f;
						g = 25f / 255f;
						b = 25f / 255f;
						break;

					case ((int)MarkerState.Occupied):
						r = 249f / 255f;
						g = 181f / 255f;
						b = 19f / 255f;
						break;

					case ((int)MarkerState.Vacant):
						r = 124f / 255f;
						g = 249f / 255f;
						b = 10f / 255f;
						break;
				}
			}

			//float mult = 0.8f;
			//r *= mult;
			//g *= mult;
			//b *= mult;
		}
	}

    public class LaunchPadMarkerTE : ModTileEntity
    {
		public int CheckInterval { get; set; } = 30; 
		public int CheckDistance { get; set; } = 20;

		public LaunchPad LaunchPad { get; set; }
		public bool HasLaunchPad => LaunchPad != null;

		public LaunchPadMarkerTE Pair { get; set; }
		public bool HasPair => Pair != null;
		public bool IsPair { get; set; }

        public MarkerState MarkerState { get; set; } = MarkerState.Inactive;
		private MarkerState lastMarkerState = MarkerState.Inactive;

		private int checkTimer;

		public override void Update()
		{
			if (IsPair)
				return;

			checkTimer++;

			if (checkTimer >= CheckInterval)
			{
				checkTimer = 0;

				MarkerState = MarkerState.Inactive;

				int x = Position.X;
				int y = Position.Y;

				if (CheckAdjacentMarkers(x, y, out LaunchPadMarkerTE pair))
				{
					Pair = pair;
					Pair.IsPair = true;

					MarkerState = MarkerState.Vacant;

					LaunchPad = LaunchPadManager.GetLaunchPadAtStartTile(MacrocosmSubworld.CurrentWorld, new(x, y));
					LaunchPad ??= LaunchPad.Create(MacrocosmSubworld.CurrentWorld, x, y);
 
					Pair.LaunchPad = LaunchPad;
					LaunchPad.EndTile = Pair.Position;
					
					if (LaunchPad.HasRocket)
						MarkerState = MarkerState.Occupied;
				}
				else
				{
					if(HasPair && !TileEntity.ByPosition.TryGetValue(Pair.Position, out _))
					{
						MarkerState = MarkerState.Inactive;
						Pair.MarkerState = MarkerState.Inactive;
					}
				}

				if (HasPair)
					Pair.MarkerState = MarkerState;

				if(MarkerState != lastMarkerState)
					OnStateChanged();

				lastMarkerState = MarkerState;
			}

			Main.tile[Position.ToPoint()].TileFrameX = GetFrame();

			if (HasPair)
				Main.tile[Pair.Position.ToPoint()].TileFrameX = GetFrame();
		}

		private bool CheckAdjacentMarkers(int x, int y, out LaunchPadMarkerTE pair)
		{
			int originalX = x;

			x = originalX; 
			while (x < originalX + CheckDistance)
			{
				x++;

				Tile tile = Main.tile[x, y];
				if (tile.HasTile)
				{
					if (tile.TileType == ModContent.TileType<LaunchPadMarker>())
					{
						bool result = TileEntity.ByPosition.TryGetValue(new(x, y), out TileEntity foundPair);
						pair = foundPair as LaunchPadMarkerTE;
						return result;
					}
					else if(HasPair && WorldGen.SolidOrSlopedTile(tile))
					{
						MarkerState = MarkerState.Invalid;
						pair = Pair;
						return false;
					}
				}
			}

			// No valid adjacent markers found
			pair = null;
			return false;
		}

		private void OnStateChanged()
		{
			if (MarkerState is MarkerState.Invalid or MarkerState.Inactive)
			{
				if (HasLaunchPad)
				{
					LaunchPadManager.Remove(MacrocosmSubworld.CurrentWorld, LaunchPad);
					LaunchPad = null;
				}
			}

			Main.tile[Position.ToPoint()].TileFrameX = GetFrame();

			if (HasPair)
				Main.tile[Pair.Position.ToPoint()].TileFrameX = GetFrame();
		}

		private short GetFrame() => (short)((int)MarkerState * 18);

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write((byte)MarkerState);
		}

		public override void NetReceive(BinaryReader reader)
		{
			MarkerState = (MarkerState)reader.ReadByte();
		}

		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.HasTile && tile.TileType == ModContent.TileType<LaunchPadMarker>();
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style, alternate);

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				//Sync the entire multitile's area. 
				NetMessage.SendTileSquare(Main.myPlayer, i, j, tileData.Width, tileData.Height);

				//Sync the placement of the tile entity with other clients
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);

				return -1;
			}

			int placedEntity = Place(i, j);

			//if (Main.netMode == NetmodeID.SinglePlayer)
			//	Rocket.Create(new Vector2(i + 9, j + 16) * 16f);

			return placedEntity;
		}

		public override void OnNetPlace()
		{
			//Rocket rocket = Rocket.Create((Position + new Point16(9, 16)).ToVector2() * 16f);
			//rocket.NetSync();
			NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
		}

        public override void OnKill()
        {
			if (HasPair)
				Pair.IsPair = false;

			if (HasLaunchPad)
				LaunchPadManager.Remove(MacrocosmSubworld.CurrentWorld, LaunchPad);
        }
    }
}
