using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
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

	internal class LaunchPadMarker : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;

            DustType = -1;
            HitSound = SoundID.Mech;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<LaunchPadMarkerTE>().Hook_AfterPlacement, -1, 0, false);

			TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(200, 200, 200), name);
        }

        public override bool CanPlace(int i, int j) => true; //TODO: not allow it between valid markers

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			ModContent.GetInstance<LaunchPadMarkerTE>().Kill(i, j);
		}
	}

    internal class LaunchPadMarkerTE : ModTileEntity
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
                Tile tile = Main.tile[x, y];

				if (CheckAdjacentMarkers(x, y, out LaunchPadMarkerTE pair))
				{
					Pair = pair;
					Pair.IsPair = true;

					MarkerState = MarkerState.Vacant;

					LaunchPad = LaunchPadManager.GetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentWorld, new(x, y));
					if (LaunchPad is null)
 						LaunchPad = LaunchPad.Create(MacrocosmSubworld.CurrentWorld, x, y);
 
					Pair.LaunchPad = LaunchPad;
					LaunchPad.EndTile = Pair.Position;
					
					if (LaunchPad.HasRocket)
						MarkerState = MarkerState.Occupied;
				}
				else
				{
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

			// Start checking to the left
			/*
			while (x >= originalX - CheckDistance)
			{
				x--;

				Tile tile = Main.tile[x, y];
				if (tile.HasTile && tile.TileType == ModContent.TileType<LaunchPadMarker>())
				{
					bool result = Utility.TryGetTileEntityAs(x, y, out LaunchPadMarkerTE foundPair);
					pair = foundPair;
 					return result;
 				}
				else
				{
					MarkerState = MarkerState.Invalid;
					break;
				}
			}
			*/

			// Start checking to the right
			x = originalX; 
			while (x < originalX + CheckDistance)
			{
				x++;

				Tile tile = Main.tile[x, y];
				if (tile.HasTile)
				{
					if (tile.TileType == ModContent.TileType<LaunchPadMarker>())
					{
						bool result = ByPosition.TryGetValue(new(x, y), out TileEntity foundPair);
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
					//TODO: NetSync ?
				}
			}

			Main.tile[Position.ToPoint()].TileFrameX = GetFrame();

			if (HasPair)
				Main.tile[Pair.Position.ToPoint()].TileFrameX = GetFrame();
		}

		private short GetFrame()
		{
			short frameNumber = MarkerState switch
			{
				MarkerState.Invalid => 0,
				MarkerState.Occupied => 1,
				MarkerState.Vacant => 2,
				MarkerState.Inactive => 3,
				_ => 3,
			};

			return (short)(frameNumber * 18);
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
