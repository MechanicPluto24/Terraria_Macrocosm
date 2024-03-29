using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Players
{
    public class StaminaPlayer : ModPlayer
    {
        private float staminaRegenCooldown = 90f;
        private float staminaRegenPeriod = 60f;
        public void ResetStaminaCooldown(float value) => staminaRegenCooldown = value;

        private float meleeStamina = 1f;
        public float MeleeStamina
        {
            get => meleeStamina;
            set => meleeStamina = MathHelper.Clamp(value, 0.01f, 1f);
        }

        public override void PostUpdateMiscEffects()
        {
            if (MeleeStamina < 1f)
            {
                staminaRegenCooldown--;
                if (staminaRegenCooldown <= 0f)
                {
                    staminaRegenCooldown = 0f;
                    MeleeStamina += 0.2f / staminaRegenPeriod;
                }
            }
            else
            {
                ResetStaminaCooldown(90f);
            }
        }


    }

}
