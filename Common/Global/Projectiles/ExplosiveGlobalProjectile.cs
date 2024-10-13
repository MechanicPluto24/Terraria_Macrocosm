using Macrocosm.Common.Sets;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles
{
    public class ExplosiveGlobalProjectile : GlobalProjectile
    {
        public override void Load()
        {
            On_Projectile.BombsHurtPlayers += On_Projectile_BombsHurtPlayers;
        }

        public override void Unload()
        {
            On_Projectile.BombsHurtPlayers -= On_Projectile_BombsHurtPlayers;
        }

        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => (entity.aiStyle == ProjAIStyleID.Explosive || ProjectileID.Sets.Explosive[entity.type]);

        // this gets reset?????
        private int sourceType = -1;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo sourceItem)
                sourceType = sourceItem.Item.type;
        }

        private void On_Projectile_BombsHurtPlayers(On_Projectile.orig_BombsHurtPlayers orig, Projectile self, Microsoft.Xna.Framework.Rectangle projRectangle, int j)
        {
            if (sourceType == -1 || ItemSets.ExplosivesShotDealDamageToOwner[sourceType])
                orig(self, projRectangle, j);
        }
    }
}
