using Macrocosm.Common.Config;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Debuffs.RadDebuffs;
using Macrocosm.Content.LoadingScreens;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Players
{
    public class IrradiationPlayer : ModPlayer
    {
    public float IrradiationLevels=0f;
    public float BaseIrradiationReduction=0.0001f;
    public float AdditonalIrradiationReduction=0f;
    public float RadNoiseIntensity=0f;
    public int Tier1Type=0;
    public int Tier2Type=0;
    public int Tier3Type=0;
    public int[] Tier1Debuffs= new int[]{
    ModContent.BuffType<MildNecrosis>(),
    ModContent.BuffType<WeakBones>()
    };
    public int[] Tier2Debuffs= new int[]{
    ModContent.BuffType<Blindness>(),
    ModContent.BuffType<Paralysis>()
    };
    public int[] Tier3Debuffs= new int[]{
    ModContent.BuffType<OrganFailure>(),
    ModContent.BuffType<Necrosis>()
    };


    public void HandleDebuffs(){
        if (IrradiationLevels>=1f){
        Player.AddBuff(Tier1Debuffs[Tier1Type],60);
        }
        else{
            Tier1Type=Main.rand.Next(0,2);
        }
        if (IrradiationLevels>=2.5f){
        Player.AddBuff(Tier2Debuffs[Tier2Type],60);
        }
        else{
            Tier2Type=Main.rand.Next(0,2);
        }
        if (IrradiationLevels>=4f){
        Player.AddBuff(Tier3Debuffs[Tier3Type],60);
        }
        else{
            Tier3Type=Main.rand.Next(0,2);
        }
    }



    public override void PostUpdateMiscEffects()
    {
    IrradiationLevels-=(BaseIrradiationReduction+AdditonalIrradiationReduction);
    if (IrradiationLevels<0f)
        IrradiationLevels=0f;
    
        if (IrradiationLevels>=0.5f){
                    if (!Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
                        Filters.Scene.Activate("Macrocosm:RadiationNoise");

                    RadNoiseIntensity = 0.2f * IrradiationLevels;

                    Filters.Scene["Macrocosm:RadiationNoise"].GetShader().UseIntensity(RadNoiseIntensity);
        }
        else
        {
        if (Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
            Filters.Scene.Deactivate("Macrocosm:RadiationNoise");
        }
        HandleDebuffs();
         
    }
    public override void Load()
        {
            IrradiationLevels = 0f;
        }
        public override void Initialize()
        {
            IrradiationLevels = 0f;
        }
        public override void UpdateDead()
        {
            IrradiationLevels = 0f;
        }
        

    }
}

