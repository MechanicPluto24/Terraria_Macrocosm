using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace Macrocosm.Common.Loot
{
    public class TECommonDrop : CommonDrop
    {
        public TileEntity TileEntity;

        public TECommonDrop(TileEntity tileEntity, int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
            : base(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
            TileEntity = tileEntity;
        }

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(chanceDenominator) < chanceNumerator)
            {
                Vector2 position = TileEntity.Position.ToWorldCoordinates();
                Vector2 itemTransferVelocity = -Vector2.UnitY * 70f;

                TileObjectData data = TileObjectData.GetTileData(Main.tile[TileEntity.Position]);
                if (data is not null)
                {
                    position = new(position.X + data.Width * 16 / 2, position.Y + data.Height * 16);
                }

                int stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);

                if (TileEntity is IInventoryOwner inventoryOwner)
                {
                    Item item = new(itemId, stack);
                    inventoryOwner.Inventory.TryPlacingItem(item);
                    Particle.CreateParticle(ParticleOrchestraType.ItemTransfer, position, itemTransferVelocity, uniqueInfoPiece: itemId);
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
