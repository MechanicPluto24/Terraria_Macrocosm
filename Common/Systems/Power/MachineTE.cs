using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Macrocosm.Common.Systems.Power
{

    public abstract partial class MachineTE : ModTileEntity
    {
        public abstract MachineTile MachineTile { get; }

        public virtual bool PoweredOn => MachineTile.IsPoweredOnFrame(Position.X, Position.Y);
        public bool ManuallyTurnedOff { get; set; }

        public virtual Color DisplayColor { get; } = Color.White;

        /// <summary> Things to happen before the first update tick. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. </summary>
        public virtual void MachineUpdate() { }

        /// <summary> Used for power state checking </summary>
        public virtual void UpdatePowerState() { }

        /// <summary> Used for updating values and things to happen when disconnected from a <see cref="WireCircuit"/> </summary>
        public virtual void OnPowerDisconnected() { }

        public virtual void OnTurnedOn(bool automatic) { }
        public virtual void OnTurnedOff(bool automatic) { }

        public virtual string GetPowerInfo() => "";

        /// <summary> Draw the power info text above the machine </summary>
        /// <param name="basePosition"> The top left in world coordinates </param>
        public virtual void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor) { }

        /// <summary>
        /// Used to toggle this machine.
        /// </summary>
        /// <param name="automatic"> Whether the machine was toggled automatically or manually. </param>
        /// <param name="skipWire"></param>
        public void Toggle(bool automatic, bool skipWire = false)
        {
            if (PoweredOn)
                TurnOff(automatic, skipWire);
            else
                TurnOn(automatic, skipWire);
        }

        /// <summary>
        /// Used to turn on this machine.
        /// </summary>
        /// <param name="automatic"> Whether the machine was turned on automatically or manually. </param>
        /// <param name="skipWire"></param>
        public void TurnOn(bool automatic, bool skipWire = false)
        {
            MachineTile.OnToggleStateFrame(Position.X, Position.Y, skipWire);
            ManuallyTurnedOff = false;
            OnTurnedOn(automatic);
        }

        /// <summary>
        /// Used to turn off this machine.
        /// </summary>
        /// <param name="automatic"> Whether the machine was turned off automatically or manually. </param>
        /// <param name="skipWire"></param>
        public void TurnOff(bool automatic, bool skipWire = false)
        {
            MachineTile.OnToggleStateFrame(Position.X, Position.Y, skipWire);
            ManuallyTurnedOff = !automatic;
            OnTurnedOff(automatic);
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

        /// <summary> Net send data. Always call base when overriding </summary>
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ManuallyTurnedOff);
        }

        /// <summary> Net receive data. Always call base when overriding </summary>
        public override void NetReceive(BinaryReader reader)
        {
            ManuallyTurnedOff = reader.ReadBoolean();
        }

        /// <summary> Save TE data. Always call base when overriding </summary>
        public override void SaveData(TagCompound tag)
        {
            if (ManuallyTurnedOff) tag[nameof(ManuallyTurnedOff)] = true;
        }

        /// <summary> Load TE data. Always call base when overriding </summary>
        public override void LoadData(TagCompound tag)
        {
            ManuallyTurnedOff = tag.ContainsKey(nameof(ManuallyTurnedOff));
        }
    }
}
