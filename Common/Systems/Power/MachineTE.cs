using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI.Chat;

namespace Macrocosm.Common.Systems.Power
{
    public enum MachineType
    {
        Consumer,
        Generator,
        Battery,
        Controller
    }

    public abstract class MachineTE : ModTileEntity, IClientUpdateable
    {
        public abstract MachineTile MachineTile { get; }

        public abstract MachineType MachineType { get; }

        public virtual bool Operating => MachineTile.IsOperatingFrame(Position.X, Position.Y);
        public virtual bool PoweredOn => MachineTile.IsPoweredOnFrame(Position.X, Position.Y);

        /// <summary> The actual power, after circuit distribution. </summary>
        public float ActivePower
        {
            get => activePower;
            set => activePower = value;
        }
        private float activePower;


        /// <summary> The normal working power of the machine </summary>
        public float Power
        {
            get => nominalPower;
            set => nominalPower = value;
        }
        private float nominalPower;

        public bool CanAutoPowerOn { get; set; } = true;
        public bool CanAutoPowerOff { get; set; } = true;

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

            Power = 0;
            MachineUpdate();
        }

        public void ClientUpdate()
        {
            Update();
        }

        public override void OnKill()
        {
            if (this is IInventoryOwner inventoryOwner)
            {
                inventoryOwner.Inventory.DropAllItems(inventoryOwner.InventoryItemDropLocation);
            }
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

        public string GetPowerInfo()
            => $"{Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo").Format($"{Power:F2}")}";

        public string GetStatusInfo()
            => $"{Language.GetText($"Mods.Macrocosm.Machines.Common.StatusInfo").Format($"{ActivePower:F2}", $"{Power:F2}")}";

        public string GetFullStatusInfo()
            => $"{Language.GetText($"Mods.Macrocosm.Machines.Common.FullStatusInfo.{MachineType}").Format($"{ActivePower:F2}", $"{Power:F2}")}";

        public string GetMachineNameAndStatusInfo()
            => $"{Lang.GetMapObjectName(MapHelper.TileToLookup(MachineTile.Type, 0))} - {GetFullStatusInfo()}";

        public static void DrawMachinePowerInfo(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.CurrentItem().type != ModContent.ItemType<CircuitProbe>())
                return;

            foreach (var kvp in ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    Color color = machine.MachineType is MachineType.Generator or MachineType.Battery ? Color.LimeGreen : Color.Orange;
                    Vector2 position = machine.Position.ToWorldCoordinates() - new Vector2(8, 16 + 8) - Main.screenPosition;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, machine.GetStatusInfo(), position, color, 0f, Vector2.Zero, Vector2.One);
                }
            }
        }
    }
}
