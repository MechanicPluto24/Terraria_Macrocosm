using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineTE : ModTileEntity
    {
        public abstract MachineTile MachineTile { get; }

        public abstract bool Operating { get; }

        public float Power { get; set; }

        /// <summary> Things to happen before the first update tick. Only runs on SP and Server. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. Only runs on SP and Server </summary>
        public virtual void MachineUpdate() { }

        private bool ranFirstUpdate = false;
        public sealed override void Update()
        {
            if (!ranFirstUpdate)
            {
                OnFirstUpdate();
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
                ranFirstUpdate = true;
            }

            UpdatePower();

            MachineUpdate();

            if(!Operating)
                Power = 0;
        }

        private void UpdatePower()
        {
            bool foundPower = false;
            for (int i = Position.X; i < Position.X + MachineTile.Width; i++)
            {
                for (int j = Position.Y; j < Position.Y + MachineTile.Height; j++)
                {
                    if (WorldGen.InWorld(i, j))
                    {
                        Tile tile = Main.tile[i, j];
                        if (IsPowered(i,j))
                        {
                            foundPower = true;
                        }
                    }
                }
            }

            if (foundPower && !MachineTile.GetPowerState(Position.X, Position.Y))
                MachineTile.TogglePowerState(Position.X, Position.Y);

            if (!foundPower && MachineTile.GetPowerState(Position.X, Position.Y))
                MachineTile.TogglePowerState(Position.X, Position.Y);
        }

        public bool IsPowered(int x, int y)
        {
            // To avoid revisiting tiles
            var visited = new HashSet<Point16>();
            return CheckForPower(x, y, visited);
        }

        private bool CheckForPower(int x, int y, HashSet<Point16> visited)
        {
            if (!WorldGen.InWorld(x, y) || visited.Contains(new Point16(x, y)))
                return false;

            visited.Add(new Point16(x, y));

            if (IsPowerSourceActive(x, y))
                return true;

            Point[] directions = 
            [
                new(0, -1),
                new(0, 1),
                new(-1, 0),
                new(1, 0)
            ];

            if (Main.rand.NextBool(20) && PowerWiring.Map[x,y].AnyWire)
            {
                var d = Dust.NewDustPerfect(new Vector2(x * 16 + 8, y * 16 + 8), DustID.Electric, Scale: 0.2f);
                d.noGravity = false;
            }

            foreach (var dir in directions)
            {

                int newX = x + dir.X;
                int newY = y + dir.Y;

                if (PowerWiring.Map[new Point16(newX, newY)].AnyWire)
                {
                    if (CheckForPower(newX, newY, visited))
                        return true;
                }
            }

            return false;
        }

        private bool IsPowerSourceActive(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            if (TileLoader.GetTile(tile.TileType) is MachineTile)
            {
                if(Utility.TryGetTileEntityAs(x, y, out MachineTE machine))
                {
                    return machine.Power > 0;
                }
            }

            return false;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == MachineTile.Type;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, MachineTile.Width, MachineTile.Height);
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            }

            Point16 tileOrigin = TileObjectData.GetTileData(type, style).Origin;
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);

            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
        }
    }
}
