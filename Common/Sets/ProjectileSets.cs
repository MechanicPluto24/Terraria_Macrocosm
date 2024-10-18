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
            ProjectileID.StardustDragon1, 0.65f,   
            ProjectileID.StardustDragon2, 0.65f,   
            ProjectileID.StardustDragon3, 0.65f,   
            ProjectileID.StardustDragon4, 0.65f,
            ProjectileID.LastPrismLaser, 0.65f
        );
    }
}
