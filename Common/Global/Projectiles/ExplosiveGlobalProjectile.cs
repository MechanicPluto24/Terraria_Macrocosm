using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
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

        private int sourceItemType = -1;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // Save the item that spawned this
            if (source is EntitySource_ItemUse_WithAmmo source_ItemUse_WithAmmo)
            {
                sourceItemType = source_ItemUse_WithAmmo.Item.type;
            }

            // Inherit item source from the projectile that spawned this (for example, Cluster Rockets)
            if (source is EntitySource_Parent source_Parent && source_Parent.Entity is Projectile proj && proj.TryGetGlobalProjectile(out ExplosiveGlobalProjectile parent))
            {
                sourceItemType = parent.sourceItemType;
            }
        }

        private void On_Projectile_BombsHurtPlayers(On_Projectile.orig_BombsHurtPlayers orig, Projectile self, Rectangle projRectangle, int j)
        {
            if (self.TryGetGlobalProjectile(out ExplosiveGlobalProjectile global))
            {
                int type = global.sourceItemType;
                if (type > 0)
                {
                    bool dealDamage;
                    if (Main.getGoodWorld)
                        dealDamage = ItemSets.ExplosivesShotDealDamageToOwner_GetGoodWorld[type];
                    else
                        dealDamage = ItemSets.ExplosivesShotDealDamageToOwner[type];

                    if (!dealDamage)
                        return;
                }
            }

            orig(self, projRectangle, j);
        }
    }
}
