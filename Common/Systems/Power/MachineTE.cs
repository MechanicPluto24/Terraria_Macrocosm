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

    public abstract partial class MachineTE : ModTileEntity
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

        /// <summary> Draw the power info text above the machine </summary>
        /// <param name="basePosition"> The top left in world coordinates </param>
        public virtual void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor) { }

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

            BuildCircuit();
            MachineUpdate();
            UpdatePowerState();
        }

        public override void PreGlobalUpdate()
        {
        }

        public override void PostGlobalUpdate()
        {
            SolveAllCircuits();
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
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }
    }
}
