using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Walls
{
    /// <summary> ModWall that can have more than one variant with respect to housing and enemy spawns. See <see cref="WallSafetyType"/>. </summary>
    public abstract class VariantWall : ModWall
    {
        /// <summary> The full set of variants to generate. First entry will be autoloaded. </summary>
        public virtual WallSafetyType[] SafetyVariants => [WallSafetyType.Safe, WallSafetyType.Natural, WallSafetyType.Unsafe];

        /// <summary> The active variant this instance represents. </summary>
        public WallSafetyType Variant { get; private set; }

        /// <summary> Internal name has a suffix appended if <see cref="Variant"/> != <see cref="WallSafetyType.Safe"/>. </summary>
        public override string Name => base.Name + (Variant != WallSafetyType.Safe ? Variant.ToString() : "");

        /// <summary> All variants share the same texture by default </summary>
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

        // Variant passed for convenience
        public virtual void SetVariantStaticDefaults(WallSafetyType variant)
        {
        }

        public sealed override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = Variant == WallSafetyType.Safe;
            SetVariantStaticDefaults(Variant);
        }

        public static int WallType<T>(WallSafetyType variant = WallSafetyType.Safe) where T : VariantWall
        {
            if (ContentInstance<T>.Instance != null)
                return ContentInstance<T>.Instance.Type;

            return ContentInstance<T>.Instances.FirstOrDefault(i => i.Variant == variant).Type; // Because yeah
        }
        public static int GetWallVariantType<T>(WallSafetyType variant) where T : VariantWall => GetWallVariant(WallType<T>(), variant)?.Type ?? 0;
        public static int GetWallVariantType(int wallType, WallSafetyType variant) => GetWallVariant(wallType, variant)?.Type ?? 0;
        public static VariantWall GetWallVariant(int wallType, WallSafetyType variant)
        {
            if (ModContent.GetModWall(wallType) is not VariantWall current)
                return null;

            string currentFullName = current.FullName;
            string variantSuffix = variant switch
            {
                WallSafetyType.Safe => "",
                WallSafetyType.Natural => "Natural",
                WallSafetyType.Unsafe => "Unsafe",
                _ => ""
            };

            string targetFullName = currentFullName.Replace("Unsafe", "").Replace("Natural", "") + variantSuffix;
            if (ModContent.TryFind(targetFullName, out VariantWall target))
                return target;

            return current;
        }
    }
}