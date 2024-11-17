using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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

        public bool CanAutoPowerOn { get; set; } = true;
        public bool CanAutoPowerOff { get; set; } = true;

        public virtual Color DisplayColor { get; } = Color.White;

        /// <summary> Things to happen before the first update tick. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. </summary>
        public virtual void MachineUpdate() { }

        /// <summary> Used for power state checking </summary>
        public virtual void UpdatePowerState() { }

        public virtual void OnPowerOn() { }
        public virtual void OnPowerOff() { }

        public virtual string GetPowerInfo() => "";

        public virtual void DrawMachinePowerInfo(SpriteBatch spriteBatch) { }

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

            MachineUpdate();
            UpdatePowerState();
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

            if (ByID.TryGetValue(placedEntity, out TileEntity tileEntity) && tileEntity is MachineTE machine)
            {
                CircuitSystem.HandleMachinePlacement(machine);
            }

            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
                CircuitSystem.HandleMachineRemoval(this);
            }
        }

        public virtual IEnumerable<Point16> GetWirePositions()
        {
            int startX = Position.X;
            int startY = Position.Y;
            int width = MachineTile.Width;
            int height = MachineTile.Height;

            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    yield return new Point16(x, y);
                }
            }
        }
    }
}
