using Terraria.ID;

namespace Macrocosm.Common.Sets
{
    /// <summary>
    /// Projectile Sets for special behavior of some Projectiles, useful for crossmod.
    /// Note: Only initalize sets with vanilla content here, add modded content to sets in SetStaticDefaults.
    /// </summary>
    public class ProjectileSets
    {
        /// <summary> Projectiles that spawn dusts on tile collision </summary>
        public static bool[] HitsTiles { get; } = ProjectileID.Sets.Factory.CreateBoolSet();

        /// <summary>  </summary>
        public static float[] DamageAdjustment { get; } = ProjectileID.Sets.Factory.CreateFloatSet(defaultState: 1f, 
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
    }
}
