using System.IO;
using Terraria;
using Terraria.ModLoader;
using Macrocosm;
using Macrocosm.Subworlds;
using SubworldLibrary;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Macrocosm.Buffs.Debuffs;

namespace Macrocosm
{
    public class MacrocosmPlayer : ModPlayer
    {
        public bool accMoonArmor = false;
        public bool ZoneMoon = false;
        public bool ZoneBasalt = false;
        public override void ResetEffects()
        {
            accMoonArmor = false;
        }
        public override void PostUpdateBuffs()
        {
            // Dust.NewDust(player.Center, 1, 1, ModContent.DustType<Dusts.RegolithDust>());
            if (Subworld.IsActive<Moon>())
            {
                if (!player.GetModPlayer<MacrocosmPlayer>().accMoonArmor) // Now die if you dont have moon armor
                {
                    player.AddBuff(ModContent.BuffType<SuitBreach>(), 2);
                }
            }    
        }
        public override void UpdateBiomes()
        {
            ZoneMoon = Subworld.IsActive<Moon>();
            ZoneBasalt = MacrocosmWorld.moonBiome > 20;
        }
        public override bool CustomBiomesMatch(Player other)
        {
            MacrocosmPlayer modOther = other.GetModPlayer<MacrocosmPlayer>();
            return ZoneMoon == modOther.ZoneMoon && ZoneBasalt == modOther.ZoneBasalt;
        }
        public override void CopyCustomBiomesTo(Player other)
        {
            MacrocosmPlayer modOther = other.GetModPlayer<MacrocosmPlayer>();
            modOther.ZoneMoon = ZoneMoon;
            modOther.ZoneBasalt = ZoneBasalt;
        }
        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ZoneMoon;
            flags[1] = ZoneBasalt;
            writer.Write(flags);
        }
        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneMoon = flags[0];
            ZoneBasalt = flags[1];
        }
        public override void PostUpdate()
        {
            /* if (Main.player[Main.myPlayer].GetModPlayer<MacrocosmPlayer>().ZoneMoon)
            {
                Main.sunTexture = ModContent.GetTexture("Macrocosm/Assets/Earth.png");
            }
            else
            {
                Main.sunTexture = ModContent.GetTexture("Terraria/Sun");
            } */
        }
        public override void PostUpdateMiscEffects()
        {
            if (ZoneMoon)
            {
                player.gravity = 0.068f;
            }
        }
        public override void UpdateBiomeVisuals()
        {
            player.ManageSpecialBiomeVisuals("Macrocosm:MoonSky", ZoneMoon, player.Center);
        }
		public override Texture2D GetMapBackgroundImage()
		{
            if (ZoneMoon)
			{
                return ModContent.GetTexture($"{typeof(Macrocosm).Name}/Map/Moon");
			}
			return null;
		}
	}
}
