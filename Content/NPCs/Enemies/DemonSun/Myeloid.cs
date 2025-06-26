using Macrocosm.Common.Bases.NPCs;
using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Items.Food;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.DemonSun
{
    public class Myeloid : ComplexAINPC<Myeloid.AIState>
    {
        public enum AIState
        {
            Fly,
            Dash
        }

        private static Asset<Texture2D> glowmask;
        private static Asset<Texture2D> glowX4;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 1;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DemonSunNPC[Type] = true;

            NPCSets.Material[Type] = NPCMaterial.Organic;
            Redemption.AddElementToNPC(Type, Redemption.ElementID.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Inorganic);
        }

        public override void SetDefaults2()
        {
            NPC.width = 56;
            NPC.height = 56;
            NPC.lifeMax = 1500;
            NPC.damage = 80;
            NPC.defense = 60;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        
        private Vector2? flyToOffset = null;

        [StateMethod(AIState.Fly)]
        private void Fly()
        {

            if (flyToOffset is null)
            {
                if ((NPC.target = GetRandomTargetInRange(1000)) == -1)
                {
                    return;
                }

                flyToOffset = Main.rand.NextVector2Unit() * 100f;
            }

            Player player = Main.player[NPC.target];


            NPC.Move(player.Center + flyToOffset.Value, Vector2.Zero, 5, 0.1f);
            NPC.velocity += NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * MathF.Sin(Main.GameUpdateCount * 0.03f);
            NPC.rotation = NPC.velocity.X * 0.1f;


            if (NPC.Center.DistanceSQ(player.Center) < 300f * 300f)
            {
                flyToOffset = null;
                SetState(AIState.Dash);
            }

        }

        private readonly int dashWaitFrames = 40;
        private readonly int chompFrames = 6;

        [StateMethod(AIState.Dash)]
        private void Dash()
        {
            if (StateTime.Frames < dashWaitFrames)
            {
                NPC.velocity *= 0.98f;

                if (StateTime.Frames == dashWaitFrames - 7)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
                }
                return;
            }
            else if (StateTime.Frames == dashWaitFrames)
            {
                NPC.velocity = NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 26f;
            }
            else if (StateTime.Frames == dashWaitFrames + chompFrames)
            {
                /*SoundEngine.PlaySound(SoundID.);*/
            }

            NPC.velocity *= 0.96f;
            NPC.rotation = NPC.velocity.X * 0.1f;

            if (NPC.velocity.LengthSquared() < 6f)
            {
                SetState(AIState.Fly);
            }
            if (Utility.CastLength(NPC.Center, NPC.rotation.ToRotationVector2(), 82f, false) < 81f)
                NPC.velocity *= -0.5f;
        }

        
        public override void HitEffect(NPC.HitInfo hit)
        {
            
            
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
            if (Main.dedServ)
                return;

            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                for (int i = 0; i < 30; i++)
                {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
                for (int i = 0; i < 2; i++)
                {
                    NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-15, 16), (int)NPC.Center.Y + Main.rand.Next(-5, 6), ModContent.NPCType<HellPustule>(), 0, 0f);
                }
            }
        }

        
    }
}
