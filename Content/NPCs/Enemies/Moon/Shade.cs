using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class Shade : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Attack,
            Flee,
            Enrage
        }

        public ActionState AI_State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public ref float AI_Speed => ref NPC.ai[1];

        public ref float AI_Rage => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.lifeMax = 500;
            NPC.damage = 50;
            NPC.defense = 60;
            NPC.HitSound = SoundID.NPCHit36 with { Volume = 0.5f };
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            SpawnModBiomes = [ModContent.GetInstance<MoonUndergroundBiome>().Type];
            AI_State = ActionState.Idle;
            NPC.Opacity = 0f;

        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;
            NPC.rotation = NPC.Center.DirectionTo(Main.player[NPC.target].Center).ToRotation();

            if (NPC.HasPlayerTarget && clearLineOfSight && AI_State == ActionState.Idle)
                AI_State = ActionState.Attack;

            if (Lighting.GetColor(NPC.Center.ToTileCoordinates()).GetBrightness() >= 0.1f && Vector2.Distance(NPC.Center, player.Center) < 200f)
                AI_Rage += 0.01f;

            if (AI_Rage > 0.1f && Vector2.Distance(NPC.Center, player.Center) < 200f)
                AI_State = ActionState.Flee;
            else
                AI_State = ActionState.Attack;

            if (AI_Rage > 3f || NPC.life < (NPC.lifeMax / 2))
                AI_State = ActionState.Enrage;

            switch (AI_State)
            {
                case ActionState.Idle:
                    Idle();
                    break;
                case ActionState.Attack:
                    Attack();
                    break;
                case ActionState.Flee:
                    Flee();
                    break;
                case ActionState.Enrage:
                    Enrage();
                    break;
            }

            if (AI_State != ActionState.Idle)
                if (NPC.Opacity < 0.5f)
                    NPC.Opacity += 0.01f;

            if (AI_State != ActionState.Idle)
                NPC.dontTakeDamage = false;
            else
                NPC.dontTakeDamage = true;
        }

        public void Idle()
        {
            //Does nothing
            NPC.Opacity = 0f;//just to be sure
            AI_Speed = 1f;
            NPC.velocity *= 0f;
        }

        public void Attack()
        {
            Player player = Main.player[NPC.target];
            Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            AI_Speed += 0.02f;

            if (AI_Speed > 2f)
                AI_Speed = 2f;

            NPC.velocity = ((NPC.velocity + (direction * 0.8f)).SafeNormalize(Vector2.UnitX)) * AI_Speed;
        }

        public void Flee()
        {
            Player player = Main.player[NPC.target];
            Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            AI_Speed -= 0.08f;

            if (AI_Speed < -2f)
                AI_Speed = -2f;

            NPC.velocity = ((NPC.velocity + (direction * 0.8f)).SafeNormalize(Vector2.UnitX)) * AI_Speed;
        }

        public void Enrage()
        {
            Player player = Main.player[NPC.target];
            Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            AI_Speed += 0.05f;

            if (AI_Speed > 8f)
                AI_Speed = 8f;

            NPC.velocity = ((NPC.velocity + (direction * 3f)).SafeNormalize(Vector2.UnitX)) * AI_Speed;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>()) ? 0.1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SpaceDust>(), 4));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                    dust.velocity.X = (dust.velocity.X + Main.rand.Next(0, 100) * 0.02f) * hit.HitDirection;
                    dust.velocity.Y = 1f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                    dust.noGravity = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(texture, NPC.position - Main.screenPosition, null, (new Color(255,255,255) * (NPC.Opacity)*0.2f), NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi, texture.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(texture, NPC.position - Main.screenPosition, null, drawColor * (NPC.Opacity), NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi, texture.Size() / 2f, NPC.scale, effects, 0);

            /*
            // Debug collision hitbox
            Rectangle hitbox = collisionHitbox;
            hitbox.X -= (int)screenPos.X;
            hitbox.Y -= (int)screenPos.Y;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Purple * 0.5f);
            */

            return NPC.IsABestiaryIconDummy;
    }
}
}