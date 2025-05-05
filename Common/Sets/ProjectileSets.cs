using Macrocosm.Common.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Sets
{
    /// <summary> Projectile Sets for special behavior of some Projectiles, useful for crossmod. </summary>
    [ReinitializeDuringResizeArrays]
    public class ProjectileSets
    {
        /// <summary> Projectiles types that spawn tile dust on collision. </summary>
        public static bool[] HitsTiles { get; } = ProjectileID.Sets.Factory.CreateNamedSet(nameof(HitsTiles)).Description("Projectiles types that spawn tile dust on collision.").RegisterBoolSet();

        /// <summary> Damage adjustment for projectiles while in a Macrocosm subworld. </summary>
        public static float[] DamageAdjustment { get; } = ProjectileID.Sets.Factory.CreateNamedSet(nameof(DamageAdjustment)).Description("Damage adjustment for projectiles while in a Macrocosm subworld.").RegisterFloatSet(defaultState: 1f,
            ProjectileID.StardustDragon1, 0.6f,
            ProjectileID.StardustDragon2, 0.6f,
            ProjectileID.StardustDragon3, 0.6f,
            ProjectileID.StardustDragon4, 0.6f,
            ProjectileID.LastPrismLaser, 0.6f,
            ProjectileID.MiniNukeMineI, 0.4f,
            ProjectileID.MiniNukeMineII, 0.4f,
            ProjectileID.MiniNukeRocketI, 0.4f,
            ProjectileID.MiniNukeRocketII, 0.4f,
            ProjectileID.MiniNukeGrenadeI, 0.4f,
            ProjectileID.MiniNukeGrenadeII, 0.4f,
            ProjectileID.MiniNukeSnowmanRocketI, 0.4f,
            ProjectileID.MiniNukeSnowmanRocketII, 0.4f
        );

        public static TrashData[] TrashData { get; } = ProjectileID.Sets.Factory.CreateCustomSet(defaultState: new TrashData(),
            ProjectileID.WoodenArrowFriendly, new TrashData(ProjectileID.WoodenArrowFriendly, 7),
            ProjectileID.SandBallGun, new TrashData(ProjectileID.SandBallGun, DustID.Sand),
            ProjectileID.SkeletonBone, new TrashData(ProjectileID.SkeletonBone, DustID.Bone)
        );
    }
}
