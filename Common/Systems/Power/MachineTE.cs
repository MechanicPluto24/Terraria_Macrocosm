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

        public virtual Color DisplayColor { get; } = Color.White;

        /// <summary> Things to happen before the first update tick. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. </summary>
        public virtual void MachineUpdate() { }

        /// <summary> Used for power state checking </summary>
        public virtual void UpdatePowerState() { }

        /// <summary> Used for updating values and things to happen when disconnected from a <see cref="WireCircuit"/> </summary>
        public virtual void OnPowerDisconnected() { }

        public virtual void OnTurnedOn() { }
        public virtual void OnTurnedOff() { }

        public virtual string GetPowerInfo() => "";

        /// <summary> Draw the power info text above the machine </summary>
        /// <param name="basePosition"> The top left in world coordinates </param>
        public virtual void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor) { }

        public void TurnOn()
        {
            MachineTile.ToggleStateFrame(Position.X, Position.Y);
            OnTurnedOn();
        }

        public void TurnOff()
        {
            MachineTile.ToggleStateFrame(Position.X, Position.Y);
            OnTurnedOff();
        }

        public override void PreGlobalUpdate()
        {
            BuildCircuits();
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

        public override void PostGlobalUpdate()
        {
            SolveCircuits();
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
