using Macrocosm.Common.Netcode;
using Macrocosm.Common.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Macrocosm.Common.Systems.Power
{

    public abstract partial class MachineTE : ModTileEntity, IInventoryOwner
    {
        public abstract MachineTile MachineTile { get; }

        public virtual bool PoweredOn => MachineTile.IsPoweredOnFrame(Position.X, Position.Y);
        public bool ManuallyTurnedOff { get; set; }

        public virtual bool CanToggleWithWire => true;

        public virtual Color DisplayColor { get; } = Color.White;

        public bool HasInventory => Inventory is not null && InventorySize > 0;
        public virtual int InventorySize => 0;
        public Inventory Inventory { get; set; }
        public InventoryOwnerType InventoryOwnerType => InventoryOwnerType.TileEntity;
        public Vector2 InventoryPosition => Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;


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

        public void PrintPowerInfo()
        {
            string powerInfo = GetPowerInfo();
            Main.NewText($"{Lang.GetMapObjectName(MapHelper.TileToLookup(MachineTile.Type, 0))} - {powerInfo}", DisplayColor);
        }

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
            BuildClusters();
            BuildCircuits();
        }

        private bool ranFirstUpdate = false;
        public sealed override void Update()
        {
            if (!ranFirstUpdate)
            {
                OnFirstUpdate();
                ranFirstUpdate = true;
            }

            MachineUpdate();
            UpdatePowerState();
        }

        public override void PostGlobalUpdate()
        {
            SolveCircuits();
        }

        public void CreateInventory()
        {
            if(Inventory is null && InventorySize > 0)
            {
                Inventory = new(InventorySize, this);
            }
        }

        public override void OnKill()
        {
            Inventory?.DropAllItems(InventoryPosition);
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
            (ByID[placedEntity] as MachineTE).CreateInventory();
            return placedEntity;
        }

        public void BlockPlacement(int i, int j)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1);
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            }

            Place(i, j);
        }

        public override void OnNetPlace() => Sync();
        public void Sync()
        {
            if(Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetHelper.SyncTEFromClient(ID);
            }
        }

        /// <summary> Net send data. Always call base if overriding </summary>
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ManuallyTurnedOff);
            TagIO.ToStream(Inventory.SerializeData(), writer.BaseStream, compress: true);
        }

        /// <summary> Net receive data. Always call base if overriding </summary>
        public override void NetReceive(BinaryReader reader)
        {
            ManuallyTurnedOff = reader.ReadBoolean();
            Inventory = Inventory.DeserializeData(TagIO.FromStream(reader.BaseStream, compressed: true));
        }

        /// <summary> Save TE data. Always call base if overriding </summary>
        public override void SaveData(TagCompound tag)
        {
            if (ManuallyTurnedOff) tag[nameof(ManuallyTurnedOff)] = true;
            tag[nameof(Inventory)] = Inventory;
        }

        /// <summary> Load TE data. Always call base if overriding </summary>
        public override void LoadData(TagCompound tag)
        {
            ManuallyTurnedOff = tag.ContainsKey(nameof(ManuallyTurnedOff));
            if (tag.ContainsKey(nameof(Inventory)))
            {
                Inventory = tag.Get<Inventory>(nameof(Inventory)) ?? new(InventorySize, this);
                Inventory.Owner = this;
            }
        }
    }
}
