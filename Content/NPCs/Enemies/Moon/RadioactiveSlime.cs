using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Players;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class RadioactiveSlime : ModNPC
    {
        private static Asset<Texture2D> glow;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 36;
            NPC.height = 22;
            NPC.damage = 50;
            NPC.defense = 80;
            NPC.lifeMax = 2100;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 1;
            AIType = NPCID.BlueSlime;
            AnimationType = NPCID.BlueSlime;
            Banner = Item.NPCtoBanner(NPCID.BlueSlime);
            BannerItem = Item.BannerToItem(Banner);
            SpawnModBiomes = [ModContent.GetInstance<IrradiationBiome>().Type];
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            //player.AddBuff(ModContent.BuffType<Irradiated>(), 600, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.SpawnTileType == ModContent.TileType<Tiles.Blocks.Terrain.IrradiatedRock>() ? 0.1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {

        }
        public override bool PreAI()
        {
            Player player = Main.LocalPlayer;
            float distance = Vector2.Distance(player.Center, NPC.Center);
            if (distance < 100f)
                player.GetModPlayer<IrradiationPlayer>().IrradiationLevel += 0.02f * (1f - distance / 100f);

            if (Main.rand.NextBool(12))
            {
                Vector2 Pos = NPC.Center + (new Vector2(1, 0).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 100f));
                Dust.NewDustDirect(Pos, 0, 0, ModContent.DustType<IrradiatedDust>(), Main.rand.NextFloat(0, 0.03f), Main.rand.NextFloat(0, 0.03f));
            }
            return true;

        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<IrradiatedDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }

        public override Color? GetAlpha(Color drawColor)
            => (Color.White.WithOpacity((0.3f + Main.DiscoColor.GetBrightness() * 0.7f)));


        private SpriteBatchState state;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            glow = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle7");
            spriteBatch.Draw(glow.Value, NPC.Center - Main.screenPosition, null, new Color(48, 237, 74, 0), 0f, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}