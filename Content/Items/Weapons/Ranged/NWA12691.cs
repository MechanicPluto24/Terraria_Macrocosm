using Macrocosm.Common.Utility;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.NPCs.GlobalNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
	public class NWA12691 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("NWA-12691");
			Tooltip.SetDefault("Right click to lock onto an enemy");
			ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 70;
			Item.height = 26;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true;
			Item.knockBack = 8f;
			Item.value = 10000;
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item11;
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
			Item.autoReuse = true;
			Item.shootSpeed = 10f;
			Item.useAmmo = AmmoID.Rocket;
		}

		public override Vector2? HoldoutOffset() => new Vector2(0, 0);

		public override bool AltFunctionUse(Player player) => true;

		public override void ModifyItemScale(Player player, ref float scale)
		{
			scale = player.altFunctionUse == 2 ? 0f : 1f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
			{
				int id = -1;
				bool found = false;

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.TryGetGlobalNPC(out MacrocosmNPC macNpc))
						macNpc.TargetedBy[player.whoAmI] = false;

					if (!found && npc.CanBeChasedBy() && Main.npc[i].getRect().Intersects(new Rectangle((int)(Main.MouseWorld.X - 10f), (int)(Main.MouseWorld.Y - 10f), 20, 20)))
					{
						id = i;
						found = true;
					}
				}

				if (id > -1 && id < Main.maxNPCs)
					Main.npc[id].Macrocosm().TargetedBy[player.whoAmI] = true;

				return false;
			}

			return true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position += new Vector2(0, -4); // barrel offset 
			type = ModContent.ProjectileType<Rocket>();
		}
	}
}