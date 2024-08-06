using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using System;
using Macrocosm.Content.Trails;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using Macrocosm.Content.Trails;
namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    //TODO Clean up this mess, make trails and add tile targeting.
    public class LuminiteElemental : ModNPC, IMoonEnemy
    {
        public static Color EyeColour => new Color(92, 228, 162);
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] =1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 38;
            NPC.height = 38;
            NPC.damage = 75;
            NPC.defense = 80;
            NPC.lifeMax = 580;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.Dig;
            NPC.value = 60f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
			NPC.noGravity = true;

            SpawnModBiomes = [ModContent.GetInstance<UndergroundMoonBiome>().Type];
        }

      

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>()) ? 0.07f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 2, 12));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SpawnDusts(3);

            if (NPC.life <= 0)
                SpawnDusts(15);
        }
        //Nice things for behaviour.
        enum Behavior : int
        {
        Idle =0,
        Attacking=1,
        Fleeing=2
        }
        public float timer=0f;
        public float ChasingVel=1f;
        public Vector2 destination = new Vector2(0,0);
        public override bool PreAI(){
            NPC.TargetClosest(true);
            if (Main.rand.NextBool(25))
                SpawnDusts();

            if (!NPC.HasPlayerTarget||Main.player[NPC.target].Distance(NPC.Center) > 800f)
                NPC.ai[0]=0f;
            if (NPC.HasPlayerTarget&&Main.player[NPC.target].Distance(NPC.Center) <= 800f)
                NPC.ai[0]=1f;
            if (NPC.life<NPC.lifeMax/3)
                NPC.ai[0]=2f;
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center,0.36f,0.89f,0.64f);
            if(NPC.ai[0]==(float)Behavior.Idle){
                IdleAround();
            }
            if(NPC.ai[0]==(float)Behavior.Attacking){
                Attack();
            }
            if(NPC.ai[0]==(float)Behavior.Fleeing){
                Flee();
            }
            timer++;
    
        }
        public void IdleAround(){
        if (timer%100==0){
            Vector2 offset = new Vector2(Main.rand.Next(-200,200),Main.rand.Next(-200,200));
            destination = NPC.Center+offset;
            timer=0;
        }
        Vector2 IAmRunningOutOfVaribleNames= (destination-NPC.Center).SafeNormalize(Vector2.UnitX);
        NPC.velocity=((NPC.velocity+(IAmRunningOutOfVaribleNames*0.1f)).SafeNormalize(Vector2.UnitX))*0.6f;
        ChasingVel=1f;
        }
        public void Attack(){
        if (timer%180==0){
            Player target = Main.player[NPC.target];
            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
            if (clearLineOfSight && target.active && !target.dead){
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.rand.Next(3, 7); i++)
                {
                    Vector2 projVelocity = Utility.PolarVector(1.5f, Main.rand.NextFloat(0,MathHelper.Pi*2));
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteShard>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer,NPC.target);
                    proj.netUpdate = true;
                }
            }
            }
            timer=0;
        }
        Player playertarget = Main.player[NPC.target];
        ChasingVel+=0.03f;
        if (ChasingVel>6f)
            ChasingVel=6f;
        Vector2 Homing=(playertarget.Center-NPC.Center).SafeNormalize(Vector2.UnitX);
        NPC.velocity=((NPC.velocity+(Homing*0.8f)).SafeNormalize(Vector2.UnitX))*ChasingVel;
        }


        public void Flee(){
        if (timer%90==0){
            Player target = Main.player[NPC.target];
            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
            if (clearLineOfSight && target.active && !target.dead){
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    Vector2 projVelocity = Utility.PolarVector(1.5f, Main.rand.NextFloat(0,MathHelper.Pi*2));
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteShard>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer,NPC.target);
                    proj.netUpdate = true;
                }
            }
            }
            timer=0;
        }


        //this took forever.
        int[] stuff = Utility.FindNearestTile(NPC.Center, 101,  TileID.LunarOre);
        Vector2 Target=new Vector2(stuff[0],stuff[1]);
        if (stuff[0] !=-1)
            NPC.velocity = (new Vector2(Target.X,Target.Y)-NPC.Center).SafeNormalize(Vector2.UnitX)*5f;
        else{
             Player playertarget = Main.player[NPC.target];
        ChasingVel+=0.03f;
        if (ChasingVel>9f)
            ChasingVel=9f;
        Vector2 Homing=(playertarget.Center-NPC.Center).SafeNormalize(Vector2.UnitX);
        NPC.velocity=((NPC.velocity+(Homing*0.8f)).SafeNormalize(Vector2.UnitX))*ChasingVel;
        }
        if ((int)Math.Abs(Target.X-NPC.Center.X)<30&&(int)Math.Abs(Target.Y-NPC.Center.Y)<30&&stuff[0] !=-1)
            NPC.life++;
            SpawnDusts(1);
        }



        

        public void SpawnDusts(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LuminiteDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
        //Pain.
        int animateTimer;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor){
            Texture2D LuminiteTexture =  ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Enemies/Moon/LuminitePebble").Value;
            
            
           
            //second rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f)<0f){
                Vector2 orbit= new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer*2))*45f,-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f);
                spriteBatch.Draw(LuminiteTexture, NPC.Center+orbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, null, Color.White, NPC.rotation, LuminiteTexture.Size()/2, NPC.scale, SpriteEffects.None, 0f);
                
            }
            //second rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f)<0f){
                Vector2 orbit= new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer*2))*45f,-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f);
                spriteBatch.Draw(LuminiteTexture, NPC.Center+orbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, null, Color.White, NPC.rotation, LuminiteTexture.Size()/2, NPC.scale, SpriteEffects.None, 0f);
            }
            

            


            Texture2D texture =  ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Enemies/Moon/LuminiteElemental").Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, texture.Size()/2, NPC.scale, SpriteEffects.None, 0f);
            Texture2D textureEye =  ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/HighRes/Star6").Value;
            spriteBatch.Draw(textureEye, ((NPC.Center+new Vector2(5f,-1f))+NPC.velocity.SafeNormalize(Vector2.UnitX)*1.1f) - Main.screenPosition, null, new Color(0.36f,0.89f,0.64f,0), NPC.rotation, textureEye.Size()/2, NPC.scale*0.04f, SpriteEffects.None, 0f);



          
             
             
           
            //first rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f)>=0f){
                Vector2 orbit= new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer*2))*45f,-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f);
                spriteBatch.Draw(LuminiteTexture, NPC.Center+orbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, null, Color.White, NPC.rotation, LuminiteTexture.Size()/2, NPC.scale, SpriteEffects.None, 0f);
            }
            //second rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f)>=0f){
                Vector2 orbit= new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer*2))*45f,-(float)Math.Sin(MathHelper.ToRadians(animateTimer*2))*12f);
                spriteBatch.Draw(LuminiteTexture, NPC.Center+orbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, null, Color.White, NPC.rotation, LuminiteTexture.Size()/2, NPC.scale, SpriteEffects.None, 0f);
            }




            animateTimer++;
            animateTimer++;
            if (animateTimer>=180)
                animateTimer=0;

            if(NPC.IsABestiaryIconDummy)
                return true;
            else
                return false;
        }
    }
}