using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Sets.Items
{
    public abstract class LiquidContainer : ModItem
    {
        public float Amount { get; set; }
        public abstract float Capacity { get; }

        public bool Full => Amount >= Capacity;
        public bool Empty => Amount <= 0f;

        public float Percent
        {
            get => MathHelper.Clamp(Amount / Capacity, 0, 1);
            set => Amount = MathHelper.Clamp(value * Capacity, 0, Capacity);
        }

        public override void SaveData(TagCompound tag)
        {
            if (Amount != default)
                tag[nameof(Amount)] = Amount;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(Amount)))
                Amount = tag.GetFloat(nameof(Amount));
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Amount);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Amount = reader.ReadSingle();
        }
    }
}
