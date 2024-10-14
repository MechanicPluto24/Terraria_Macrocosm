using Macrocosm.Common.Bases.NPCs;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Environment.Debris;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class MoonWormHead : WormHead
    {
        private bool CheckCollision()
        {
            int minTilePosX = (int)(NPC.Left.X / 16) - 1;
            int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
            int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
            int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

            // Ensure that the tile range is within the world bounds
            if (minTilePosX < 0)
                minTilePosX = 0;

            if (maxTilePosX > Main.maxTilesX)
                maxTilePosX = Main.maxTilesX;

            if (minTilePosY < 0)
                minTilePosY = 0;

            if (maxTilePosY > Main.maxTilesY)
                maxTilePosY = Main.maxTilesY;

            bool collision = false;

            // This is the initial check for collision with tiles.
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // If the tile is solid or is considered a platform, then there's valid collision
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                        if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
                        {
                            // Collision found
                            collision = true;
                        }
                    }
                }
            }

            return collision;
        }
        public override int BodyType => ModContent.NPCType<MoonWormBody>();
        public override int TailType => ModContent.NPCType<MoonWormTail>();

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                CustomTexturePath = Texture.Replace("Head", "") + "_Bestiary",
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.damage = 100;
            NPC.lifeMax = 10000;
            NPC.defense = 160;
            FlipSprite = true;
            NPC.width = 86;
            NPC.height = 86;
            NPC.aiStyle = -1;
            SpawnModBiomes = [ModContent.GetInstance<MoonNightBiome>().Type];
        }
        public override float FallSpeed => 0.2f;
        public override bool UseSmoothening => true;
        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<AlienResidue>(), 1, 4, 10));
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
			});
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && !Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 200 ? .01f : 0f;
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 14;
            MaxSegmentLength = 22;
            FlipSprite = true;
            NPC.position.Y += 600;
            SoundEngine.PlaySound(SoundID.NPCDeath10);
            CommonWormInit(this);
        }

        public static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 15.5f;
            worm.Acceleration = 0.12f;
        }

        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }

        public bool CheckForSoildTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile.HasUnactuatedTile == true)
                return true;
            else
            {
                return false;
            }
        }

        public bool burst = false; // Has the worm bursted out of the ground.
        public int TouchedGround=0;
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (CheckCollision())
                   TouchedGround=20;
                

                NPC.despawnEncouraged = false;
                // tick down the attack counter.
                if (attackCounter > 0)
                    attackCounter--;

                Player target = Main.player[NPC.target];

                // If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
                if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 2000 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1) && burst == false&&(TouchedGround>=1))
                {
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                    burst = true;

                    // Spawn debris
                    for (int i = 0; i < 50; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextFloat(1.0f, 30.0f), ModContent.ProjectileType<RegolithDebris>(), 0, 0f, -1);

                    // Spawn damaging rubble
                    for (int i = 0; i < 10; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextFloat(10.0f, 20.0f), ModContent.ProjectileType<MoonRubble>(), 50, 0f, -1);

                    // Spawn smoke
                    for (int i = 0; i < 90; i++)
                    {
                        Smoke smoke = Particle.Create<Smoke>((p) =>
                        {
                            p.Position = NPC.Center;
                            p.Velocity = NPC.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextFloat(1.0f, 15.0f);
                            p.Scale = new(1f);
                            p.Rotation = 0f;
                            p.Color = Smoke.GetTileHitColor(Utility.GetClosestTile(NPC.Center, -1, addTile: (t) => Main.tileSolid[t.TileType] && !t.IsActuated));
                            p.FadeIn = true;
                            p.Opacity = 0f;
                            p.ScaleVelocity = new(0.0075f);
                        });
                    }
                }
                TouchedGround--;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 14; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormHead1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormHead2").Type);

                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
    }
    public class MoonWormBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.damage = 120;
            NPC.defense = 69;
            NPC.width = 54;
            NPC.height = 54;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;
        }
        public override bool UseSmoothening => true;
        public override void Init()
        {
            FlipSprite = true;
            NPC.position.Y += 600;
            MoonWormHead.CommonWormInit(this);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 14; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (NPC.life <= 0)
            {
                if ((int)NPC.frameCounter == 1)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormSegment1").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormSegment2").Type);
                }
                else
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormAlternate1").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormAlternate2").Type);
                }

                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
        private bool CheckCollision()
        {
            int minTilePosX = (int)(NPC.Left.X / 16) - 1;
            int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
            int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
            int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

            // Ensure that the tile range is within the world bounds
            if (minTilePosX < 0)
                minTilePosX = 0;

            if (maxTilePosX > Main.maxTilesX)
                maxTilePosX = Main.maxTilesX;

            if (minTilePosY < 0)
                minTilePosY = 0;

            if (maxTilePosY > Main.maxTilesY)
                maxTilePosY = Main.maxTilesY;

            bool collision = false;

            // This is the initial check for collision with tiles.
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // If the tile is solid or is considered a platform, then there's valid collision
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                        if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
                        {
                            // Collision found
                            collision = true;
                        }
                    }
                }
            }

            return collision;
        }

        float SmoothFallSpeed = 0f;
        public override void CustomBodyAI(Worm worm)
        {
            if (!CheckCollision())
                SmoothFallSpeed += 0.1f;
            else
                SmoothFallSpeed -= 0.1f;
            if (SmoothFallSpeed > 3f)
                SmoothFallSpeed = 3f;
            if (SmoothFallSpeed < 0f)
                SmoothFallSpeed = 0f;
            NPC.position.Y += SmoothFallSpeed;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.frameCounter = Main.rand.Next(2);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }
    }

    public class MoonWormTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.damage = 100;
            NPC.defense = 75;
            NPC.width = 50;
            NPC.height = 50;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0f;
        }
        private bool CheckCollision()
        {
            int minTilePosX = (int)(NPC.Left.X / 16) - 1;
            int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
            int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
            int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

            // Ensure that the tile range is within the world bounds
            if (minTilePosX < 0)
                minTilePosX = 0;
            if (maxTilePosX > Main.maxTilesX)
                maxTilePosX = Main.maxTilesX;
            if (minTilePosY < 0)
                minTilePosY = 0;
            if (maxTilePosY > Main.maxTilesY)
                maxTilePosY = Main.maxTilesY;

            bool collision = false;

            // This is the initial check for collision with tiles.
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // If the tile is solid or is considered a platform, then there's valid collision
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                        if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
                        {
                            // Collision found
                            collision = true;
                        }
                    }
                }
            }

            return collision;
        }
        float SmoothFallSpeed = 0f;
        public override void CustomTailAI(Worm worm)
        {
            if (!CheckCollision())
                SmoothFallSpeed += 0.1f;
            else
                SmoothFallSpeed -= 0.1f;
            if (SmoothFallSpeed > 3f)
                SmoothFallSpeed = 3f;
            if (SmoothFallSpeed < 0f)
                SmoothFallSpeed = 0f;
            NPC.position.Y += SmoothFallSpeed;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 14; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormTail").Type);

                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }

        public override void Init()
        {
            FlipSprite = true;
            NPC.position.Y += 600;
            MoonWormHead.CommonWormInit(this);
        }
    }
}
