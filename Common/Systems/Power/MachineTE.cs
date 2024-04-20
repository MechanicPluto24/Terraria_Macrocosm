using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
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

        public virtual bool Operating => MachineTile.IsOperatingFrame(Position.X, Position.Y);
        public virtual bool PoweredUp => MachineTile.IsPoweredUpFrame(Position.X, Position.Y);

        public float ActivePower { get; protected set; }

        public float ConsumedPower { get; protected set; }
        public float GeneratedPower { get; protected set; }

        /// <summary> Things to happen before the first update tick. Only runs on SP and Server. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. Only runs on SP and Server </summary>
        public virtual void MachineUpdate() { }

        public virtual void OnPowerUp() { }
        public virtual void OnPowerDown() { }

        public void PowerUp()
        {
            MachineTile.TogglePowerStateFrame(Position.X, Position.Y);
            OnPowerUp();
        }

        public void PowerDown()
        {
            MachineTile.TogglePowerStateFrame(Position.X, Position.Y);
            OnPowerDown();
        }

        private bool ranFirstUpdate = false;
        public sealed override void Update()
        {
            if (!ranFirstUpdate)
            {
                OnFirstUpdate();
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
                ranFirstUpdate = true;
            }

            MachineUpdate();

            if(GeneratedPower > 0)
                ActivePower = GeneratedPower;

            if (ConsumedPower > 0)
                ActivePower = 0;

            UpdatePower();
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
                        var visited = new HashSet<Point16>();
                        if (CheckForPower(i, j, visited))
                        {
                            foundPower = true;
                        }

                        WireEffects(foundPower, visited);
                    }
                }
            }

            if (foundPower && !PoweredUp)
            {
                PowerUp();
            }

            if (!foundPower && PoweredUp)
            {
                PowerDown();
            }
        }

        private bool CheckForPower(int x, int y, HashSet<Point16> visited)
        {
            if (!WorldGen.InWorld(x, y) || visited.Contains(new Point16(x, y)))
                return false;

            visited.Add(new Point16(x, y));

            if (ConsumedPower > 0)
                ActivePower += TryConsumingPowerFromGenerators(x, y);

            if (ConsumedPower > 0 && ActivePower >= ConsumedPower)
                return true;

            Point[] directions = 
            [
                new(0, -1),
                new(0, 1),
                new(-1, 0),
                new(1, 0)
            ];

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

        private float TryConsumingPowerFromGenerators(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            if (TileLoader.GetTile(tile.TileType) is MachineTile)
            {
                if(Utility.TryGetTileEntityAs(x, y, out MachineTE machine) && machine != this && machine.GeneratedPower > 0)
                {
                    float sourcePower = machine.ActivePower;
                    float powerNeeded = ConsumedPower - ActivePower;
                    machine.ActivePower -= powerNeeded;
                    machine.ActivePower = Math.Max(0, machine.ActivePower);
                    return sourcePower - machine.ActivePower;
                }
            }

            return 0f;

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

        private void WireEffects(bool foundPower, HashSet<Point16> visited)
        {
            if (foundPower && PoweredUp && ConsumedPower > 0)
            {
                foreach (var p in visited)
                {
                    if (Main.rand.NextBool(20) && PowerWiring.Map[p].AnyWire)
                    {
                        var d = Dust.NewDustDirect(p.ToWorldCoordinates() + new Vector2(2), 8, 8, DustID.Electric, Scale: 0.2f, SpeedX: 0, SpeedY: 0);
                        d.noGravity = false;
                    }
                }
            }
        }

    }
}
