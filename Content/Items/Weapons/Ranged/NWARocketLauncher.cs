using Macrocosm.Common.Bases;
using Macrocosm.Common.Utils;
using Macrocosm.Content.NPCs.Global;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class NWARocketLauncher : GunHeldProjectileItem
    {
        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(40, 13),
            CenterYOffset = 8,
            MuzzleOffset = 33,
            Recoil = (14, 0.1f),
            RecoilDiminish = 0.985f
        };

        public int HeldProjectileType => throw new System.NotImplementedException();

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;

        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 26;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item11;
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
            Item.autoReuse = true;
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, -8);

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {

        }

        public override void ModifyItemScale(Player player, ref float scale)
        {
            scale = player.AltFunction() ? 1f : 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.AltFunction())
            {
                int id = -1;
                bool found = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.TryGetGlobalNPC(out MacrocosmNPC macNpc))
                        macNpc.TargetedByHomingProjectile = false;

                    if (!found && npc.CanBeChasedBy() && Main.npc[i].getRect().Intersects(new Rectangle((int)(Main.MouseWorld.X - 10f), (int)(Main.MouseWorld.Y - 10f), 20, 20)))
                    {
                        id = i;
                        found = true;
                    }
                }

                if (id > -1 && id < Main.maxNPCs)
                    Main.npc[id].GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile = true;

                return false;
            }

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -10) + Utility.PolarVector(25f, velocity.ToRotation());
            type = ModContent.ProjectileType<NWAMissile>();
        }
    }
}
