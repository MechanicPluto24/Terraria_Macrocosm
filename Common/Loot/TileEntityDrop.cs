using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace Macrocosm.Common.Loot
{
    public class TileEntityDrop : IItemDropRule
    {
        public TileEntity TileEntity;
        public int ItemId;
        public int ChanceDenominator;
        public int AmountDroppedMinimum;
        public int AmountDroppedMaximum;
        public int ChanceNumerator;

        public TileEntityDrop(TileEntity tileEntity, int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
        {
            if (amountDroppedMinimum > amountDroppedMaximum)
            {
                throw new ArgumentOutOfRangeException(nameof(amountDroppedMinimum), $"{nameof(amountDroppedMinimum)} must be lesser or equal to {nameof(amountDroppedMaximum)}.");
            }

            TileEntity = tileEntity;
            ItemId = itemId;
            ChanceDenominator = chanceDenominator;
            AmountDroppedMinimum = amountDroppedMinimum;
            AmountDroppedMaximum = amountDroppedMaximum;
            ChanceNumerator = chanceNumerator;
            ChainedRules = [];
        }

        public List<IItemDropRuleChainAttempt> ChainedRules { get; }

        public virtual bool CanDrop(DropAttemptInfo info) => true;

        public virtual ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(ChanceDenominator) < ChanceNumerator)
            {
                Vector2 position = TileEntity.Position.ToWorldCoordinates();
                TileObjectData data = TileObjectData.GetTileData(Main.tile[TileEntity.Position]);
                if (data is not null)
                    position = Main.rand.NextVector2FromRectangle(new((int)position.X, (int)position.Y, data.Width * 16, data.Height * 16));

                CommonCode.DropItem(position, new EntitySource_TileEntity(TileEntity), ItemId, info.rng.Next(AmountDroppedMinimum, AmountDroppedMaximum + 1));
                result = default;
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            result = default;
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
        }

        public virtual void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            float chance = ChanceNumerator / ChanceDenominator;
            float dropRate = chance * ratesInfo.parentDroprateChance;
            drops.Add(new DropRateInfo(ItemId, AmountDroppedMinimum, AmountDroppedMaximum, dropRate, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, chance, drops, ratesInfo);
        }

    }
}
