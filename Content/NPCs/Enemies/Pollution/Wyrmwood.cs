using Macrocosm.Common.Bases.NPCs;
using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Pollution
{
    public class WyrmwoodHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<WyrmwoodBody>();
        public override int TailType => ModContent.NPCType<WyrmwoodTail>();

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                CustomTexturePath = Texture.Replace("Head", "") + "_Bestiary",
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPC.ApplyImmunity
            (
                BuffID.Confused
            );

            MoRHelper.AddElementToNPC(Type, MoRHelper.Shadow);
            MoRHelper.AddNPCToElementList(Type, MoRHelper.NPCType_Dark);
        }
        public override float FallSpeed => 0f;
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.lifeMax = 1200;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.width = 26;
            NPC.height = 58;
            SpawnModBiomes = [ModContent.GetInstance<PollutionBiome>().Type];
            NPC.aiStyle = -1;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<PollutionBiome>() && Main.hardMode ? 1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 5;
            MaxSegmentLength = 9;
            CanFly = true;
            CommonWormInit(this);
        }

        public static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 5.5f;
            worm.Acceleration = 0.04f;
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

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // tick down the attack counter.
                if (attackCounter > 0)
                    attackCounter--;

                Player target = Main.player[NPC.target];
                // If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
                if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 200 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
                {
                    // some projectile attack here?
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }

    public class WyrmwoodBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            NPC.ApplyImmunity
            (
                BuffID.Confused
            );

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
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
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.damage = 20;
            NPC.defense = 20;
            NPC.npcSlots = 0f;
            NPC.width = 28;
            NPC.height = 28;
            NPC.aiStyle = -1;
            Main.npcFrameCount[Type] = 1;
            attackCounter = Main.rand.Next(400, 500);
        }

        public override void Init()
        {
            FlipSprite = true;

            WyrmwoodHead.CommonWormInit(this);
        }
        public override void CustomBodyAI(Worm worm)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // tick down the attack counter.
                if (attackCounter > 0)
                    attackCounter--;

                Player target = Main.player[NPC.target];
                if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 1000f)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 5), ModContent.ProjectileType<WyrmwoodProjectile>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer);
                    attackCounter = Main.rand.Next(400, 500);
                }
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void FindFrame(int frameHeight)
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }

    public class WyrmwoodTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPC.ApplyImmunity
            (
                BuffID.Confused
            );

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.damage = 5;
            NPC.defense = 30;
            NPC.width = 28;
            NPC.height = 44;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;
        }

        public override void Init()
        {
            FlipSprite = true;
            WyrmwoodHead.CommonWormInit(this);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}
