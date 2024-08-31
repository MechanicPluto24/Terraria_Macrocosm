using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using System;
using Macrocosm.Common.DataStructures;
using Macrocosm.Content.Dusts;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class KyaniteScarabSmall : ModNPC
    {
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;

            NPCSets.MoonNPC[NPC.type] = true;
            NPCSets.DropsMoonstone[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 20;
            NPC.damage = 50;
            NPC.defense = 62;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.knockBackResist = 0.6f;
            NPC.aiStyle = -1;
            SpawnModBiomes = [ModContent.GetInstance<MoonUndergroundBiome>().Type];
        }
        int InterestTimer=0;
        bool HasRock=false;
        public override void AI()
        {
            Player target = Main.player[NPC.target];
            Point luminiteCoords = Utility.GetClosestTile(NPC.Center, TileID.LunarOre, 100);
            Vector2 luminitePosition = luminiteCoords.ToWorldCoordinates();
            if(luminiteCoords != default&&Vector2.Distance(NPC.Center, target.Center) > 600f){
               
                if(Vector2.DistanceSquared(NPC.Center, luminitePosition) >40&&InterestTimer<1200){
                    NPC.velocity = (luminitePosition - NPC.Center).SafeNormalize(Vector2.UnitX) * 3f;
                    InterestTimer++;
                }
                else
                {
                NPC.velocity.X*=0.2f;
                if(InterestTimer<1200)
                    InterestTimer=1200;
                if(NPC.velocity.Y==0){
                    InterestTimer++;
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,ModContent.DustType<ProtolithDust>());
                    dust.velocity.X = (dust.velocity.X + Main.rand.Next(0, 100) * 0.02f);
                    dust.velocity.Y = 1f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                    dust.noGravity = true;
                    if (InterestTimer>1320){
                        InterestTimer=0;
                        HasRock=true;
                    }


                }

                }




            }
            else{
            Utility.AIZombie(NPC, ref NPC.ai, fleeWhenDay: false, allowBoredom: false);
            }
           
            NPC.spriteDirection = NPC.direction;
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

            if (Math.Abs(NPC.velocity.Y) > 0.01f)
                NPC.frame.Y = (Main.npcFrameCount[Type] - 1) * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteSmallGore1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteSmallGore3").Type);
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteSmallGore2").Type);
            }
        }
        SpriteBatchState state;
        Vector2 drawPos;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D rockTexture = ModContent.Request<Texture2D>(Texture + "_Pebble").Value;
            SpriteEffects Effect = NPC.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = NPC.direction == -1 ? (NPC.Center + new Vector2(-10f, (float)(NPC.height/2))) - Main.screenPosition:(NPC.Center + new Vector2(22f, (float)(NPC.height/2))) - Main.screenPosition;
            Color colour = NPC.GetAlpha(drawColor);
            if(HasRock)
                spriteBatch.Draw(rockTexture, drawPos, null, colour, NPC.rotation, NPC.Size / 2, NPC.scale, Effect, 0f);


           
        }
    }
}
