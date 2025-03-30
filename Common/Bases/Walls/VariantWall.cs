using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Walls
{
    public abstract class VariantWall : ModWall
    {
        /// <summary> The full set of variants to generate. First entry will be autoloaded. </summary>
        public virtual WallSafetyType[] SafetyVariants => [WallSafetyType.Normal, WallSafetyType.Natural, WallSafetyType.Unsafe];

        /// <summary> The active variant this instance represents. </summary>
        public WallSafetyType Variant { get; set; } = WallSafetyType.Normal;

        public override string Name => base.Name + (Variant != WallSafetyType.Normal ? Variant.ToString() : "");
        public override string Texture => this.GetNamespacePath();

        public VariantWall()
        {
            Variant = SafetyVariants.FirstOrDefault();
        }

        public VariantWall CreateVariant(WallSafetyType variant)
        {
            VariantWall instance = (VariantWall)Activator.CreateInstance(GetType());
            instance.Variant = variant;
            return instance;
        }

        public virtual void SetVariantStaticDefaults(WallSafetyType variant)
        {
        }

        public sealed override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = Variant == WallSafetyType.Normal;
            SetVariantStaticDefaults(Variant);
        }
    }
}