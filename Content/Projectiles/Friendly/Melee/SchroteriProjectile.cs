using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Common.Utils;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
	// Shortsword projectiles are handled in a special way with how they draw and damage things
	// The "hitbox" itself is closer to the player, the sprite is centered on it
	// However the interactions with the world will occur offset from this hitbox, closer to the sword's tip (CutTiles, Colliding)
	// Values chosen mostly correspond to Iron Shortsword
	public class SchroteriProjectile : ModProjectile
	{
        public const int FadeInDuration = 7;
		public const int FadeOutDuration = 4;

		public const int TotalDuration = 30;
        private Schroteri blade;

        private int hitStacks;

		// The "width" of the blade
		public float CollisionWidth => 10f * Projectile.scale;

		public int Timer {
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void OnSpawn(IEntitySource source)
        {
            blade = (source as EntitySource_ItemUse_WithAmmo).Item.ModItem as Schroteri;
            hitStacks = blade.HitStacks;
			if(hitStacks>Schroteri.MaxStacks)
				hitStacks=Schroteri.MaxStacks;
            Projectile.netUpdate = true;
        }

		public override void SetDefaults() {
			Projectile.Size = new Vector2(50); // This sets width and height to the same value (important when projectiles can rotate)
			Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.scale = 1f;
			Projectile.DamageType = DamageClass.MeleeNoSpeed;
			Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
			Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
			Projectile.timeLeft = 30; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
			Projectile.hide = true; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			Timer += 1;
			
			if (Timer >= TotalDuration) {
				// Kill the projectile if it reaches it's intended lifetime
				Projectile.Kill();
				return;
			}
			else {
				// Important so that the sprite draws "in" the player's hand and not fully in front or behind the player
				player.heldProj = Projectile.whoAmI;
			}

			// Fade in and out
			// GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
			// The first call handles the fade in, the second one the fade out.
			// Notice the second call's parameters are swapped, this means the result will be reverted
			Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

			// Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
			Projectile.Center = playerCenter + Projectile.velocity * (Timer - 1f);

			// Set spriteDirection based on moving left or right. Left -1, right 1
			

			// Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

			// The code in this method is important to align the sprite with the hitbox how we want it to
			SetVisualOffsets();
		}

		private void SetVisualOffsets() {
			// 32 is the sprite size (here both width and height equal)
			const int HalfSpriteWidth = 32 / 2;
			const int HalfSpriteHeight = 32 / 2;

			int HalfProjWidth = Projectile.width / 2;
			int HalfProjHeight = Projectile.height / 2;

			// Vanilla configuration for "hitbox in middle of sprite"
			DrawOriginOffsetX = 0;
			DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
			DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);

			// Vanilla configuration for "hitbox towards the end"
			//if (Projectile.spriteDirection == 1) {
			//	DrawOriginOffsetX = -(HalfProjWidth - HalfSpriteWidth);
			//	DrawOffsetX = (int)-DrawOriginOffsetX * 2;
			//	DrawOriginOffsetY = 0;
			//}
			//else {
			//	DrawOriginOffsetX = (HalfProjWidth - HalfSpriteWidth);
			//	DrawOffsetX = 0;
			//	DrawOriginOffsetY = 0;
			//}
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone){
			hitStacks += 1;
			
			blade.HitStacks+=1;
            blade.ResetTimer = 0;
            Projectile.netUpdate = true;
	    }

		public override bool ShouldUpdatePosition() {
			// Update Projectile.Center manually
			return false;
		}

		public override void CutTiles() {
			// "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
			Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
            Player player = Main.player[Projectile.owner];
           
            hitbox = Utils.CenteredRectangle(hitbox.Center.ToVector2() +(Utility.PolarVector(10 * hitStacks, Projectile.velocity.ToRotation()))*(float)((float)Timer / (float)TotalDuration), new Vector2(hitbox.Width, hitbox.Height));
        }
		public override bool PreDraw(ref Color lightColor){
			Player player = Main.player[Projectile.owner];
			
			Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
			Color effectColor = new Color(130, 220, 199, 0)*(float)((float)hitStacks/(float)Schroteri.MaxStacks);
            float rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            SpriteEffects spriteEffect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			for (float progress = 0.4f; progress <= 1f; progress += 0.1f)
            	Main.EntitySpriteDraw(texture,Vector2.Lerp(Projectile.Center,Projectile.Center+ (Utility.PolarVector(10 * hitStacks, Projectile.velocity.ToRotation())*(float)((float)Timer / (float)TotalDuration)), progress + 0.2f)+ new Vector2(0f, Projectile.gfxOffY)- Main.screenPosition, null, effectColor, rotation, texture.Size() / 2f, 1f*progress, spriteEffect);
				return true;
		}
	}
}