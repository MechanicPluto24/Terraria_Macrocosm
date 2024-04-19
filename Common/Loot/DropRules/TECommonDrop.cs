using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace Macrocosm.Common.Loot.DropRules
{
    public class TECommonDrop : CommonDrop, IBlacklistable
    {
        public TileEntity TileEntity;

        public TECommonDrop(TileEntity tileEntity, int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
            : base(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
            TileEntity = tileEntity;
        }

        public bool Blacklisted { get; set; }
        public int ItemID => itemId;

        public override bool CanDrop(DropAttemptInfo info) => !Blacklisted;

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(chanceDenominator) < chanceNumerator)
            {
                int stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);
                Vector2 position = TileEntity.Position.ToWorldCoordinates();
                TileObjectData data = TileObjectData.GetTileData(Main.tile[TileEntity.Position]);
                if (data is not null)
                    position = new(position.X + data.Width * 16 / 2, position.Y + (data.Height + 3) * 16);

                if (TileEntity is IInventoryOwner inventoryOwner && inventoryOwner.Inventory is not null)
                {
                    Item item = new(itemId, stack);
                    inventoryOwner.Inventory.TryPlacingItem(item, sound: false);

                    Particle.CreateParticle<ItemTransferParticle>((p) =>
                    {
                        p.StartPosition = position + Main.rand.NextVector2Circular(32, 16);
                        p.EndPosition = position + new Vector2(0,-96) + Main.rand.NextVector2Circular(16, 16);
                        p.ItemType = itemId;
                    });
                }
                else
                {
                    CommonCode.DropItem(position, new EntitySource_TileEntity(TileEntity), itemId, stack);
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
