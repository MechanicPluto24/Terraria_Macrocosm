using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Pollution
{
    public class Trashnado : ModNPC
    {
        private static List<Asset<Texture2D>> junkTextures = new();
        private static List<int> junkTilts = new();
        private static List<int> junkOffsets = new();

        private int TrashOrbitTimer;
        private float TrashRotation;

      

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPC.ApplyImmunity
            (
                BuffID.Bleeding,
                BuffID.BloodButcherer,
                BuffID.Poisoned,
                BuffID.Venom
            );

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 52;
            NPC.height = 68;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 400;

            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            SpawnModBiomes = [ModContent.GetInstance<PollutionBiome>().Type];
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<PollutionBiome>() && Main.hardMode ? 1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

      
        public static string GetTrash()//Todo, make this more extensive. Also keep this public
        {
            int Rand = Main.rand.Next(1,12);
            switch (Rand){
                case 1:
                    return "Terraria/Images/Projectile_1";
                    break;
                
                case 2:
                    return "Terraria/Images/Projectile_42";
                    break;
                case 3:
                    return "Terraria/Images/Projectile_471";
                    break;
                case 4:
                    return "Terraria/Images/Item_23";
                    break;
                case 5:
                    return "Terraria/Images/Item_9";
                    break;
                case 6:
                    return "Terraria/Images/Item_71";
                    break;
                case 7:
                    return "Terraria/Images/Item_90";
                    break;
                case 8:
                    return "Terraria/Images/Gore_99";
                    break;
                case 9:
                    return "Terraria/Images/Gore_268";
                    break;
                case 10:
                    return "Terraria/Images/Gore_639";
                    break;
                default:
                    return "Terraria/Images/Item_172";
                    break;
                
            }   
            return "Terraria/Images/Item_172";
        }
        int smogTimer=0;
        int attackTimer;
        public override void AI()
        {
            if(++smogTimer%3==0)
            {
                Smoke smoke = Particle.Create<Smoke>((p) =>
                            {
                                p.Position = NPC.Center+new Vector2(0,NPC.height/2);
                                p.Velocity = new Vector2(0f, 1f).RotatedByRandom(MathHelper.TwoPi);
                                p.Acceleration = new Vector2(0f,0f);
                                p.Scale = new(0.2f);
                                p.Rotation = 0f;
                                p.Color = (new Color(80, 80, 80) * Main.rand.NextFloat(0.75f, 1f)).WithAlpha(215);
                                p.VanillaFadeIn = true;
                                p.Opacity = NPC.Opacity;
                                p.ScaleVelocity = new(0.0075f);
                                p.WindFactor = Main.windSpeedCurrent > 0 ? 0.035f : 0.01f;
                            });
            }
            if(junkTextures.Count<1)
            { 
                junkTextures.Add(ModContent.Request<Texture2D>(GetTrash()));
                junkTilts.Add(Main.rand.Next(-16,16));
                junkOffsets.Add(Main.rand.Next(0,90));
                junkTextures.Add(ModContent.Request<Texture2D>(GetTrash()));
                junkTilts.Add(Main.rand.Next(-16,16));
                junkOffsets.Add(Main.rand.Next(0,90));
                junkTextures.Add(ModContent.Request<Texture2D>(GetTrash()));
                junkTilts.Add(Main.rand.Next(-16,16));
                junkOffsets.Add(Main.rand.Next(0,90));
                junkTextures.Add(ModContent.Request<Texture2D>(GetTrash()));
                junkTilts.Add(Main.rand.Next(-16,16));
                junkOffsets.Add(Main.rand.Next(0,90));
                junkTextures.Add(ModContent.Request<Texture2D>(GetTrash()));
                junkTilts.Add(Main.rand.Next(-16,16));
                junkOffsets.Add(Main.rand.Next(0,90));
                junkTextures.Add(ModContent.Request<Texture2D>(GetTrash()));
                junkTilts.Add(Main.rand.Next(-16,16));
                junkOffsets.Add(Main.rand.Next(0,90));
            }
            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];

            Utility.AIFighter(NPC, ref NPC.ai, player.Center, accelerationFactor: 0.08f, velMax: 4f, maxJumpTilesX: 1, maxJumpTilesY: 4);

            if(NPC.velocity.Y>0)
                NPC.velocity.Y*=0.9f;


            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = -NPC.direction;


            TrashOrbitTimer += 1;
            attackTimer++;
            if(Vector2.Distance(NPC.Center, player.Center)<600f)
            {
                if(attackTimer>23)
                {
                    Vector2 projVelocity = Utility.PolarVector(8f, -Main.rand.NextFloat(0, MathHelper.Pi));
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity+new Vector2(player.Center.X>NPC.Center.X ? 4:-4,0), ModContent.ProjectileType<TrashnadoProjectile>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, ai1: NPC.target, ai2: 10f);
                    attackTimer=0;
                }
            }
        }

        

        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 8;
            NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame + 0) * frameHeight;

            if (NPC.frameCounter++ >= (ticksPerFrame * (Main.npcFrameCount[Type] - 1)) - 1)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        SpriteBatchState state;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TrashRotation += 0.01f;

            DrawTrash(spriteBatch, drawColor, true);
            SpriteEffects effect = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int frameHeight = TextureAssets.Npc[Type].Height() / Main.npcFrameCount[Type];
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, new Vector2(TextureAssets.Npc[Type].Width() / 2f, frameHeight / 2f), NPC.scale, effect, 0f);

            DrawTrash(spriteBatch, drawColor, false);

            return NPC.IsABestiaryIconDummy;
        }

        private void DrawTrash(SpriteBatch spriteBatch, Color drawColor, bool isFront)
        {
            for(int i=0; i < junkTextures.Count;i++)
            {
            float OrbitOffset = junkOffsets[i];
            float speedFactor = 7.5f-i;
            float OrbitTilt = junkTilts[i];
            Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians((TrashOrbitTimer+OrbitOffset) * speedFactor)) * 45f, -(float)Math.Sin(MathHelper.ToRadians((TrashOrbitTimer+OrbitOffset) * speedFactor)) * 12f);

            if((int)((TrashOrbitTimer+OrbitOffset)* speedFactor) % 360 < 180 == isFront)
                spriteBatch.Draw(junkTextures[i].Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(OrbitTilt)) - Main.screenPosition+new Vector2(0,-20+(8*i)), null, drawColor, TrashRotation, junkTextures[i].Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }
        }
    }
}
