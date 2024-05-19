using Macrocosm.Common.Drawing.Trails;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles
{
    public class TrailGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public VertexTrail Trail { get; set; }
    }
}