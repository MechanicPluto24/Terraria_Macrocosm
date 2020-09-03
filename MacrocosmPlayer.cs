using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Macrocosm;
using Macrocosm.Subworlds;
using SubworldLibrary;
using Terraria.ID;

namespace Macrocosm
{
    class MacrocosmPlayer : ModPlayer
    {
        public bool ZoneMoon = false;
        public bool ZoneBasalt = false;
        public override void UpdateBiomes()
        {
            ZoneMoon = MacrocosmWorld.moonBiome > 20;
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
            if (Subworld.IsActive<Moon>())
            {
                ZoneMoon = true;
                player.gravity = 0.068f;
            }
        }
        public override void UpdateBiomeVisuals()
        {
            player.ManageSpecialBiomeVisuals("Macrocosm:MoonSky", ZoneMoon, player.Center);
        }
    }
}
