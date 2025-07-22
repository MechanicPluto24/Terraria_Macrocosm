using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Radiation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class IrradiationPlayer : ModPlayer
    {
        public static List<LocalizedText> DeathMessages { get; } = [];

        public override void Load()
        {
            for (int i = 0; i < Utility.FindAllLocalizationThatStartsWith("Mods.Macrocosm.DeathMessages.Irradiated").Length; i++)
                DeathMessages.Add(Language.GetOrRegister($"Mods.Macrocosm.DeathMessages.Irradiated.Message{i}"));
        }

        /// <summary>
        /// The player's current irradiation level.
        /// Not synced.
        /// </summary>
        public float IrradiationLevel { get; set; } = 0f;

        /// <summary> 
        /// Irradiation reduction per tick.
        /// Not synced.
        /// </summary>
        public float IrradiationReduction { get; set; } = 0.002f;

        /// <summary>
        /// Visual static noise intensity
        /// </summary>
        public float RadiationNoiseIntensity { get; set; } = 0f;

        int[] mildDebuffs;
        int[] moderateDebuffs;
        int[] severeDebuffs;

        public override void OnEnterWorld()
        {
            IrradiationLevel = 0f;
        }

        public override void Initialize()
        {
            IrradiationLevel = 0f;
        }

        public override void UpdateDead()
        {
            IrradiationLevel = 0f;

            RadiationNoiseIntensity *= 0.95f;
            UpdateEffects();
        }

        public override void ResetEffects()
        {
            RadiationNoiseIntensity = 0f;
            IrradiationReduction = 0.002f;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Main.gamePaused)
                return;

            UpdateRadiation();
            UpdateSymptomDebuffs();
            UpdateEffects();
        }

        private void UpdateRadiation()
        {
            IrradiationLevel -= IrradiationReduction;
            IrradiationLevel = MathHelper.Clamp(IrradiationLevel, 0f, 6f);

            RadiationNoiseIntensity += 0.05f * IrradiationLevel;
            RadiationNoiseIntensity = MathHelper.Clamp(RadiationNoiseIntensity, 0f, 0.5f);

            //Main.NewText($"Irradiation: {IrradiationLevel}, RadNoise: {RadiationNoiseIntensity}");

            if (IrradiationLevel >= 0.55)
                Player.AddBuff(ModContent.BuffType<Irradiated>(), 2);

            if (IrradiationLevel <= 0.45)
                Player.ClearBuff(ModContent.BuffType<Irradiated>());
        }

        private void UpdateSymptomDebuffs()
        {
            if (!Main.rand.NextBool(600))
                return;

            int buffType = 0;

            mildDebuffs ??= BuffSets.GetRadiationDebuffs(RadiationSeverity.Mild);
            moderateDebuffs ??= BuffSets.GetRadiationDebuffs(RadiationSeverity.Moderate);
            severeDebuffs ??= BuffSets.GetRadiationDebuffs(RadiationSeverity.Severe);

            if (IrradiationLevel >= 4f)
            {
                if (Main.rand.NextFloat() <= 0.5f)
                    buffType = severeDebuffs.GetRandom();
                else if (Main.rand.NextFloat() <= 0.7f)
                    buffType = moderateDebuffs.GetRandom();
                else
                    buffType = mildDebuffs.GetRandom();
            }
            else if (IrradiationLevel >= 2.5f)
            {
                if (Main.rand.NextFloat() <= 0.7f)
                    buffType = moderateDebuffs.GetRandom();
                else
                    buffType = mildDebuffs.GetRandom();
            }
            else if (IrradiationLevel >= 1f)
            {
                buffType = mildDebuffs.GetRandom();
            }

            int duration = BuffSets.TypicalDuration[buffType];
            if (duration <= 0)
                duration = 60;
            duration = (int)(duration * Main.rand.NextFloat(0.6f, 1f));

            if (buffType > 0)
                Player.AddBuff(buffType, duration);
        }

        private void UpdateEffects()
        {
            if (Main.dedServ)
                return;

            if (RadiationNoiseIntensity >= 0.01f)
            {
                if (!Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
                    Filters.Scene.Activate("Macrocosm:RadiationNoise");

                Filters.Scene["Macrocosm:RadiationNoise"].GetShader().UseIntensity(RadiationNoiseIntensity);
            }
            else
            {
                if (Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
                    Filters.Scene.Deactivate("Macrocosm:RadiationNoise");
            }
        }
    }
}

