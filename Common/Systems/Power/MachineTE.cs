using Macrocosm.Common.Bases.Tiles;
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
    public abstract class MachineTE : ModTileEntity, IClientUpdateable
    {
        public abstract MachineTile MachineTile { get; }

        public virtual bool Operating => MachineTile.IsOperatingFrame(Position.X, Position.Y);
        public virtual bool PoweredOn => MachineTile.IsPoweredOnFrame(Position.X, Position.Y);

        private float activePower;
        public float ActivePower
        {
            get => activePower;
            set
            {
                activePower = MathHelper.Lerp(activePower, value, 0.25f);
            }
        }

        public bool IsConsumer => consumedPower != null;
        private float? consumedPower;

        public float ConsumedPower 
        { 
            get => consumedPower ?? 0f;
            set
            {
                consumedPower = value;
                generatedPower = null;
            }
        }

        public bool IsGenerator => generatedPower != null;
        private float? generatedPower;
        public float GeneratedPower
        {
            get => generatedPower ?? 0f;
            set
            {
                generatedPower = value;
                consumedPower = null;
            }
        }

        /// <summary> Things to happen before the first update tick. Only runs on SP and Server. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. Only runs on SP and Server </summary>
        public virtual void MachineUpdate() { }

        public virtual void OnPowerOn() { }
        public virtual void OnPowerOff() { }

        public void PowerOn()
        {
            MachineTile.TogglePowerStateFrame(Position.X, Position.Y);
            OnPowerOn();
        }

        public void PowerOff()
        {
            MachineTile.TogglePowerStateFrame(Position.X, Position.Y);
            OnPowerOff();
        }

        private bool ranFirstUpdate = false;
        public sealed override void Update()
        {
            if (!ranFirstUpdate)
            {
                OnFirstUpdate();

                if(Main.netMode != NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);

                ranFirstUpdate = true;
            }

            GeneratedPower = 0;
            ConsumedPower = 0;
            MachineUpdate();
        }

        public void ClientUpdate()
        {
            Update();
        }

        public bool IsConnected(MachineTE other)
        {
            for (int i = Position.X; i < Position.X + MachineTile.Width; i++)
            {
                for (int j = Position.Y; j < Position.Y + MachineTile.Height; j++)
                {
                    if (WorldGen.InWorld(i, j))
                    {
                        var visited = new HashSet<Point16>();
                        if (CheckForConnection(i, j, other, visited))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool CheckForConnection(int x, int y, MachineTE other, HashSet<Point16> visited)
        {
            if (!WorldGen.InWorld(x, y) || visited.Contains(new Point16(x, y)))
                return false;

            visited.Add(new Point16(x, y));

            if (Utility.TryGetTileEntityAs(x, y, out MachineTE found) && found.ID == other.ID)
                return true;

            Point[] directions = { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };

            foreach (var dir in directions)
            {
                int checkX = x + dir.X;
                int checkY = y + dir.Y;
                if (PowerWiring.Map[new Point16(checkX, checkY)].AnyWire)
                {
                    if (CheckForConnection(checkX, checkY, other, visited))
                        return true;
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

            CircuitSystem.SearchCircuits();

            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
                CircuitSystem.SearchCircuits();
            }
        }
    }
}
