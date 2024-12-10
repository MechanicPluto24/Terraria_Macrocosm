using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace Macrocosm.Common.Loot.DropRules
{
    public class TECommonDrop : CommonDrop, IBlacklistable
    {
        public MachineTE MachineTE;
        private readonly float multipleEntityFactor = 1f;
        private readonly int? multipleEntityMaxDistance = null;

        /// <summary>
        /// <c>multipleEntityFactor</c> represents how chanceDenominator increases (returns per unit decrease) for multiple machines of this type. 
        /// <br/> 0.0f => chanceDenominator is invariable to the number of machines; total returns increase untouched.
        /// <br/> 1.0f => chanceDenominator increases directly with the number of machines; returns per unit decrease; total returns are unchanged regardless of number of machines.
        /// <br/> 0.Xf => chanceDenominator increases by specified factor; returns per unit decrease by number of machines, depending on factor; total returns increase amount depends on factor (closer to 0 => high, closer to 1 => low).   
        /// </summary>
        public TECommonDrop(MachineTE machine, int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1, float multipleEntityFactor = 0.5f, int? multipleEntityMaxDistance = null)
            : base(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
            MachineTE = machine;
            this.multipleEntityFactor = multipleEntityFactor;
            this.multipleEntityMaxDistance = multipleEntityMaxDistance;
        }

        public bool Blacklisted { get; set; }
        public int ItemID => itemId;

        public override bool CanDrop(DropAttemptInfo info) => !Blacklisted;

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;

            int activeMachines = 0;
            foreach (var machine in TileEntity.ByID.Values.OfType<MachineTE>().Where((te) => te.type == MachineTE.type))
                if (machine.PoweredOn)
                    activeMachines++;

            int adjustedChanceDenominator = (int)(chanceDenominator * (1f + multipleEntityFactor * (activeMachines - 1)));

            if (info.player.RollLuck(adjustedChanceDenominator) < chanceNumerator)
            {
                int stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);
                Vector2 position = MachineTE.Position.ToWorldCoordinates();
                TileObjectData data = TileObjectData.GetTileData(Main.tile[MachineTE.Position]);
                if (data is not null)
                    position = new(position.X + data.Width * 16 / 2, position.Y + (data.Height + 3) * 16);

                if (MachineTE is IInventoryOwner inventoryOwner && inventoryOwner.Inventory is not null)
                {
                    Item item = new(itemId, stack);
                    inventoryOwner.Inventory.TryPlacingItem(item, sound: false);

                    Particle.Create<ItemTransferParticle>((p) =>
                    {
                        p.StartPosition = position + Main.rand.NextVector2Circular(32, 16);
                        p.EndPosition = position + new Vector2(0, -96) + Main.rand.NextVector2Circular(16, 16);
                        p.ItemType = itemId;
                        p.TimeToLive = Main.rand.Next(60, 80);
                    });
                }
                else
                {
                    CommonCode.DropItem(position, new EntitySource_TileEntity(MachineTE), itemId, stack);
                }

                result = default;
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            result = default;
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
        }
    }
}
