using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Macrocosm.Content.Buffs.Debuffs;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Subworlds.Moon;
using Terraria.ID;
using Macrocosm.Content.Biomes;

namespace Macrocosm.Content
{
    public class MacrocosmPlayer : ModPlayer
    {
        public bool accMoonArmor = false;
        public int accMoonArmorDebuff = 0;
        public bool ZoneMoon = false;
        public bool ZoneBasalt = false;
        public override void ResetEffects()
        {
            accMoonArmor = false;
        }

        public override void PostUpdateBuffs() 
        {
            if (SubworldSystem.IsActive<Moon>()) 
            {
                if (!Player.GetModPlayer<MacrocosmPlayer>().accMoonArmor) {
                    Player.AddBuff(ModContent.BuffType<SuitBreach>(), 2);
                }
            }
        }
      
        public override void PostUpdateMiscEffects() 
        {
            if (ZoneMoon)
                Player.gravity = 0.068f;

            if (accMoonArmorDebuff > 0)
                Player.buffImmune[ModContent.BuffType<SuitBreach>()] = false;
        }

		public override void PostUpdate() 
        {
			if(accMoonArmorDebuff > 0)
                accMoonArmorDebuff--;
		}

        //const float oldMaxScreenPosY = 11864f;  // Ignore the magic numbers :peepohappy: (bruhh)
        const float maxScreenPosY = 15164f;

        public override void ModifyScreenPosition()
        {
            if (SubworldSystem.AnyActive(Mod)) 
            {
                if (Main.screenPosition.Y >= maxScreenPosY) 
                { 
                    Main.screenPosition = new Vector2(Main.screenPosition.X, maxScreenPosY);
                }
            }
        }
    }
}
