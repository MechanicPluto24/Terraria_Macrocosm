using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Shield)]
	public class CelestialBulwark : ModItem
	{
		public override void SetStaticDefaults()
		{

		}
		public override void SetDefaults()
		{
			Item.damage = 120;
			Item.DamageType = DamageClass.Melee;
			Item.crit = 0;
			Item.knockBack = 9f;
			Item.width = 34;
			Item.height = 40;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
			Item.accessory = true;

			Item.defense = 14;
		}

		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
			=> !((equippedItem.type == Type && incomingItem.type == ItemID.EoCShield) || (incomingItem.type == Type && equippedItem.type == ItemID.EoCShield));

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();

			dashPlayer.AccDashHorizontal = true;
			dashPlayer.AccDashVertical = true;

			dashPlayer.AccDashSpeedX = 14f;
			dashPlayer.AccDashSpeedY = 8f;

			dashPlayer.AccDashDamage = Item.damage;
			dashPlayer.AccDashKnockback = Item.knockBack;
			dashPlayer.AccDashImmuneTime = 6;

			dashPlayer.AccDashHitboxIncrease = 16;

			dashPlayer.AccDashAfterImage = true;
			dashPlayer.AccDashStartVisuals = StartDashVisuals;
			dashPlayer.AccDashVisuals = DashVisuals;

			dashPlayer.OnCollisionWithNPC = OnNPCCollide;
		}

        public override void UpdateVanity(Player player)
        {
        }

        private void StartDashVisuals(Player player)
        {
            Particle.CreateParticle<CelestialBulwarkDashParticle>(p =>
            {
                p.Scale = 0.35f;
                p.Color = CelestialDisco.CelestialColor;
                p.Position = player.Center;
                p.PlayerID = player.whoAmI;
                p.Rotation = player.velocity.ToRotation() - MathHelper.PiOver2;
            });

			/*
            for (int k = 0; k < 50; k++)
            {
                int dustType = ModContent.DustType<CelestialDust>();
                Dust dust = Dust.NewDustDirect(new Vector2(player.position.X, player.position.Y), player.width, player.height, dustType, 0f, 0f, 100, default, 2f);
				dust.velocity = Main.rand.NextVector2Unit().RotatedBy(player.velocity.ToRotation());
            }
			*/
        }

        private void DashVisuals(Player player)
        {
			DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            float progress = (float)dashPlayer.DashTimer / dashPlayer.AccDashDuration;
            Lighting.AddLight(player.Center, CelestialDisco.CelestialColor.ToVector3() * 5f * Utility.QuadraticEaseIn(progress));
			int count = (int)MathF.Floor(5 * progress);

            for (int i = 0; i < count; i++)
            {
                Dust.NewDustDirect(player.Center + new Vector2(35, 0).RotatedBy(player.velocity.ToRotation()) + Main.rand.NextVector2Circular(50, 50) * progress, 1, 1, ModContent.DustType<CelestialDust>(), 0f, 0f, 100, default, 1.1f);
            }
        }

        private void OnNPCCollide(Player player, NPC npc)
		{
            for (int i = 0; i < 10; i++)
                Particle.CreateParticle<CelestialStar>(npc.Center + Main.rand.NextVector2Circular(npc.width/2, npc.height/2), npc.velocity + Main.rand.NextVector2Circular(2,2), scale: 1.2f);

            for (int i = 0; i < 25; i++)
            {
				Vector2 dustVelocity = Main.rand.NextVector2Circular(2, 2);
                Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<CelestialDust>(), dustVelocity.X, dustVelocity.Y, 100, default, 1.5f);
            }
        }

        public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<BrokenHeroShield>());
			recipe.AddIngredient(ItemID.EoCShield);
			recipe.AddIngredient(ItemID.FragmentNebula, 15);
			recipe.AddIngredient(ItemID.FragmentStardust, 15);
			recipe.AddIngredient(ItemID.FragmentVortex, 15);
			recipe.AddIngredient(ItemID.FragmentSolar, 15);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			DrawMask(spriteBatch, position, origin, scale);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			DrawMask(spriteBatch, Item.Center - Main.screenPosition, Item.Size / 2f, scale, rotation);
			Lighting.AddLight(Item.Center, CelestialDisco.CelestialColor.ToVector3());
		}

		private static Texture2D[] celestialTextures =
			{
				ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Nebula").Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Stardust").Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Vortex").Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Solar").Value
			};


		private SpriteBatchState state;
		private void DrawMask(SpriteBatch spriteBatch, Vector2 position, Vector2 origin, float scale = 1f, float rotation = 0f)
		{
			Texture2D currentTex = celestialTextures[(int)CelestialDisco.CelestialStyle];
			Texture2D nextTex = celestialTextures[(int)CelestialDisco.NextCelestialStyle];
			Color currentColor = Color.White.WithOpacity(CelestialDisco.CelestialStyleProgress);
			Color nextColor = Color.White.WithOpacity(1f - CelestialDisco.CelestialStyleProgress);

			state.SaveState(spriteBatch);
			spriteBatch.EndIfBeginCalled();

			spriteBatch.Begin(BlendState.NonPremultiplied, state);

			spriteBatch.Draw(currentTex, position, null, nextColor, rotation, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(nextTex, position, null, currentColor, rotation, origin, scale, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}
}