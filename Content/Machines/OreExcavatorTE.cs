using Macrocosm.Common.Bases.Machines;
using Macrocosm.Common.Loot;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Items.Placeable.Blocks;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines
{
    public class OreExcavatorTE : MachineTE, IInventoryOwner
    {
        public override MachineTile MachineTile => ModContent.GetInstance<OreExcavator>();
        public override bool Operating => Main.tile[Position].TileFrameY >= 16 * MachineTile.Height;


        protected SimpleLootTable loot;
        protected int checkTimer;
        protected bool ranFirstUpdate;

        protected virtual int OreGenerationRate => 60;
        protected virtual int InventorySize => 50;

        public Inventory Inventory { get; set; }

        public SimpleLootTable Loot => loot;

        public Vector2 InventoryItemDropLocation => Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, MachineTile.Width, MachineTile.Height);
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            }

            Point16 tileOrigin = new(0, MachineTile.Height - 1);
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);

            return placedEntity;
        }

        public virtual void PopulateItemLoot()
        {
            switch (MacrocosmSubworld.CurrentID)
            {
                case "Macrocosm/Moon":

                    loot.Add(new TECommonDrop(this, ModContent.ItemType<Regolith>(), 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<Protolith>(), 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100));

                    loot.Add(new TECommonDrop(this, ModContent.ItemType<ArtemiteOre>(), 10));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<SeleniteOre>(), 10));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<ChandriumOre>(), 10));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<DianiteOre>(), 10));
                    loot.Add(new TECommonDrop(this, ItemID.LunarOre, 8));

                    break;

                default:

                    loot.Add(new TECommonDrop(this, ItemID.DirtBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100));
                    loot.Add(new TECommonDrop(this, ItemID.StoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100));

                    loot.Add(new TECommonDrop(this, ItemID.CopperOre, 16));
                    loot.Add(new TECommonDrop(this, ItemID.TinOre, 16));
                    loot.Add(new TECommonDrop(this, ItemID.IronOre, 12));
                    loot.Add(new TECommonDrop(this, ItemID.LeadOre, 12));
                    loot.Add(new TECommonDrop(this, ItemID.SilverOre, 10));
                    loot.Add(new TECommonDrop(this, ItemID.TungstenOre, 10));
                    loot.Add(new TECommonDrop(this, ItemID.GoldOre, 8));
                    loot.Add(new TECommonDrop(this, ItemID.PlatinumOre, 8));

                    loot.Add(new TEDropWithConditionRule(this, ItemID.CobaltOre, 8, condition: new Conditions.IsHardmode()));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.PalladiumOre, 8, condition: new Conditions.IsHardmode()));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.MythrilOre, 6, condition: new Conditions.IsHardmode()));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.OrichalcumOre, 6, condition: new Conditions.IsHardmode()));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.AdamantiteOre, 4, condition: new Conditions.IsHardmode()));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.TitaniumOre, 4, condition: new Conditions.IsHardmode()));

                    break;
            }
        }

        public override void Update()
        {
            if (!ranFirstUpdate)
            {
                Inventory ??= new(InventorySize, this);

                if (Inventory.Owner is null)
                    Inventory.Owner = this;

                // Generate loot table based on current subworld
                loot = new();
                PopulateItemLoot();

                ranFirstUpdate = true;
            }

            if (Operating)
                checkTimer++;

            if (checkTimer >= OreGenerationRate)
            {
                checkTimer = 0;
                loot?.Drop(Utility.GetClosestPlayer(Position, MachineTile.Width * 16, MachineTile.Height * 16));
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<OreExcavator>();
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            TagIO.Write(Inventory.SerializeData(), writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            TagCompound tag = TagIO.Read(reader);
            Inventory = Inventory.DeserializeData(tag);
        }

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(Inventory)] = Inventory;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(Inventory)))
                Inventory = tag.Get<Inventory>(nameof(Inventory));
        }
    }
}
