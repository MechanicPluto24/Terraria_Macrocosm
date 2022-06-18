using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Buffs.Debuffs;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies {
    public class Clavite : ModNPC {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Clavite");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.MeteorHead];
        }
        public override void SetDefaults() {
            NPC.width = 60;
            NPC.height = 60;
            NPC.lifeMax = 2500;
            NPC.damage = 60;
            NPC.defense = 60;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            AnimationType = NPCID.MeteorHead;
            Banner = Item.NPCtoBanner(NPCID.MeteorHead);
            BannerItem = Item.BannerToItem(Banner);
        }

        public override void AI() {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
                NPC.TargetClosest(true);
            }

            Move(Vector2.Zero);
            bool playerActive = player != null && player.active && !player.dead;
            BaseAI.LookAt(playerActive ? player.Center : (NPC.Center + NPC.velocity), NPC, 0);
        }
        public void Move(Vector2 offset, float speed = 3f, float turnResistance = 0.5f) {
            Player player = Main.player[NPC.target];
            Vector2 moveTo = player.Center + offset; // Gets the point that the NPC will be moving to.
            Vector2 move = moveTo - NPC.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed) {
                move *= speed / magnitude;
            }
            move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed) {
                move *= speed / magnitude;
            }
            NPC.velocity = move;
        }

        private float Magnitude(Vector2 mag) => (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);

        public override void OnHitPlayer(Player player, int damage, bool crit) {
            if (player.GetModPlayer<MacrocosmPlayer>().accMoonArmor) { // Now only suit breaches players with said suit 
                player.AddBuff(ModContent.BuffType<SuitBreach>(), 600, true);
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.SpawnTileType == ModContent.TileType<Tiles.Regolith>() ? .1f : 0f;
        }
        
        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<CosmicDust>()));             // Always drop 1 cosmic dust
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 16% chance to drop 1-6 Artemite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 16% chance to drop 1-6 Chandrium Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 16% chance to drop 1-6 Selenite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 16% chance to drop 1-6 DianiteOre Ore
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y *= Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}
