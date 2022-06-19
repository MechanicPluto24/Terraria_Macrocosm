using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Items.Materials;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies
{
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
            //Banner = Item.NPCtoBanner(NPCID.MeteorHead);  removed these as per Pluto's instructions - Feldy
            //BannerItem = Item.BannerToItem(Banner);
        }

        public override void AI() {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
                NPC.TargetClosest(true);
            }

            Move(Vector2.Zero);
            bool playerActive = player != null && player.active && !player.dead;
            LookAt(playerActive ? player.Center : (NPC.Center + NPC.velocity), NPC, 0);
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
                dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if(Main.netMode == NetmodeID.Server)
            {
                return; // don't run on the server
            }

            if(NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreHead1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreHead2").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 2, Mod.Find<ModGore>("ClaviteGoreJaw1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreJaw2").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 1.5f, Mod.Find<ModGore>("ClaviteGoreEye1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 2, Mod.Find<ModGore>("ClaviteGoreEye2").Type);
            }
        }


        // Helper methods shamelessly copied from BaseMod.BaseAI by Grox the Great 
        // The only leftover reference from BaseMod 
        // TODO: maybe remove when we rework Clavite AI - Feldy 
        
        public static void LookAt(Vector2 lookTarget, Entity c, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false)
        {
            int spriteDirection = (c is NPC ? ((NPC)c).spriteDirection : c is Projectile ? ((Projectile)c).spriteDirection : 0);
            float rotation = (c is NPC ? ((NPC)c).rotation : c is Projectile ? ((Projectile)c).rotation : 0f);
            LookAt(lookTarget, c.Center, ref rotation, ref spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
            if (c is NPC)
            {
                ((NPC)c).spriteDirection = spriteDirection;
                ((NPC)c).rotation = rotation;
            }
            else
            if (c is Projectile)
            {
                ((Projectile)c).spriteDirection = spriteDirection;
                ((Projectile)c).rotation = rotation;
            }
        }

        /*
         * Makes the rotation value and sprite direction 'look' at the given target.
         * lookType : the type of look code to run.
         *        0 -> Rotate the entity and change sprite direction based on the look target.
         *        1 -> change spriteDirection based on the look target.
         *        2 -> Rotate the entity based on the look target.
         *        3 -> Smoothly rotate and change sprite direction based on the look target.
         *        4 -> Smoothly rotate based on the look target.       
         * rotAddon : the amount to add to the rotation. (only used by lookType 3/4)
         * rotAmount: the amount to rotate by. (only used by lookType 3/4)
         */
        public static void LookAt(Vector2 lookTarget, Vector2 center, ref float rotation, ref int spriteDirection, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.075f, bool flipSpriteDir = false)
        {
            if (lookType == 0)
            {
                if (lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (flipSpriteDir) { spriteDirection *= -1; }
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                rotation = -((float)Math.Atan2((double)rotX, (double)rotY) - 1.57f + rotAddon);
                if (spriteDirection == 1) { rotation -= (float)Math.PI; }
            }
            else
            if (lookType == 1)
            {
                if (lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (flipSpriteDir) { spriteDirection *= -1; }
            }
            else
            if (lookType == 2)
            {
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                rotation = -((float)Math.Atan2((double)rotX, (double)rotY) - 1.57f + rotAddon);
            }
            else
            if (lookType == 3 || lookType == 4)
            {
                int oldDirection = spriteDirection;
                if (lookType == 3 && lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (lookType == 3 && flipSpriteDir) { spriteDirection *= -1; }
                if (oldDirection != spriteDirection)
                {
                    rotation += (float)Math.PI * spriteDirection;
                }
                float pi2 = (float)Math.PI * 2f;
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                float rot = ((float)Math.Atan2((double)rotY, (double)rotX) + rotAddon);
                if (spriteDirection == 1) { rot += (float)Math.PI; }
                if (rot > pi2) { rot -= pi2; } else if (rot < 0) { rot += pi2; }
                if (rotation > pi2) { rotation -= pi2; } else if (rotation < 0) { rotation += pi2; }
                if (rotation < rot)
                {
                    if ((double)(rot - rotation) > (float)Math.PI) { rotation -= rotAmount; } else { rotation += rotAmount; }
                }
                else
                if (rotation > rot)
                {
                    if ((double)(rotation - rot) > (float)Math.PI) { rotation += rotAmount; } else { rotation -= rotAmount; }
                }
                if (rotation > rot - rotAmount && rotation < rot + rotAmount) { rotation = rot; }
            }
        }


    }
}
