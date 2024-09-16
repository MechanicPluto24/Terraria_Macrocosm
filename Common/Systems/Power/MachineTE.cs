using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
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

        /// <summary> Things to happen before the first update tick. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. </summary>
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
                ranFirstUpdate = true;

                //if(Main.netMode != NetmodeID.MultiplayerClient)
                //    NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }

            GeneratedPower = 0;
            ConsumedPower = 0;
            MachineUpdate();
        }

        public void ClientUpdate()
        {
            Update();
        }

        public string GetPowerInfo()
        {
            // TODO: localize these 4 words
            string name = Lang.GetMapObjectName(MapHelper.TileToLookup(MachineTile.Type, 0));
            if (IsGenerator)
                return $"{name} - Available: {ActivePower:F2}W / Generated: {GeneratedPower:F2} W";
            else
                return $"{name} - Consumed: {ActivePower:F2}W / Required: {ConsumedPower:F2} W";
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

                        var tile = Main.tile[i, j];
                        if (tile.RedWire && CheckForConnection(i, j, other, visited, VanillaWireType.Red))
                            return true;

                        if (tile.BlueWire && CheckForConnection(i, j, other, visited, VanillaWireType.Blue))
                            return true;

                        if (tile.GreenWire && CheckForConnection(i, j, other, visited, VanillaWireType.Green))
                            return true;

                        if (tile.YellowWire && CheckForConnection(i, j, other, visited, VanillaWireType.Yellow))
                            return true;
                    }
                }
            }

            return false;
        }


        private bool CheckForConnection(int x, int y, MachineTE other, HashSet<Point16> visited, VanillaWireType wireType)
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

                if (!WorldGen.InWorld(checkX, checkY))
                    continue;

                var currentTile = Main.tile[x, y];
                var checkTile = Main.tile[checkX, checkY];

                switch (wireType)
                {
                    case VanillaWireType.Red: 
                        if (currentTile.RedWire && checkTile.RedWire)
                            if (CheckForConnection(checkX, checkY, other, visited, wireType))
                                return true;
                        break;

                    case VanillaWireType.Blue:  
                        if (currentTile.BlueWire && checkTile.BlueWire)
                            if (CheckForConnection(checkX, checkY, other, visited, wireType))
                                return true;
                        break;

                    case VanillaWireType.Green:
                        if (currentTile.GreenWire && checkTile.GreenWire)
                            if (CheckForConnection(checkX, checkY, other, visited, wireType))
                                return true;
                        break;

                    case VanillaWireType.Yellow:
                        if (currentTile.YellowWire && checkTile.YellowWire)
                            if (CheckForConnection(checkX, checkY, other, visited, wireType))
                                return true;
                        break;
                }
            }

            return false;
        }


        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == MachineTile.Type;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            Point16 tileOrigin = TileObjectData.GetTileData(type, style).Origin;
            int x = i - tileOrigin.X;
            int y = j - tileOrigin.Y;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, x, y, MachineTile.Width, MachineTile.Height);
                NetMessage.SendData(MessageID.TileEntityPlacement, number: x, number2: y, number3: Type);
                return -1;
            }

            int placedEntity = Place(x, y);
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
