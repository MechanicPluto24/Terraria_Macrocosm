using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Subworlds.Moon
{

    //TODO: add music, sky, whatever left 
    
    public class BasaltBiome : ModBiome
    {

        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("ExampleMod/ExampleSurfaceBackgroundStyle");

        //public override int Music => base.Music;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Moon");
        }

        public override bool IsBiomeActive(Player player)
        {
            return MacrocosmWorld.moonBiome > 20;
        }
    }
}

/*
   public override void UpdateBiomes() {
        ZoneMoon = Subworld.IsActive<Moon>();
        ZoneBasalt = MacrocosmWorld.moonBiome > 20;
    }
    public override bool CustomBiomesMatch(Player other) {
        var modOther = other.GetModPlayer<MacrocosmPlayer>();
        return ZoneMoon == modOther.ZoneMoon && ZoneBasalt == modOther.ZoneBasalt;
    }
    public override void CopyCustomBiomesTo(Player other) {
        var modOther = other.GetModPlayer<MacrocosmPlayer>();
        modOther.ZoneMoon = ZoneMoon;
        modOther.ZoneBasalt = ZoneBasalt;
    }
    public override void SendCustomBiomes(BinaryWriter writer) {
        var flags = new BitsByte();
        flags[0] = ZoneMoon;
        flags[1] = ZoneBasalt;
        writer.Write(flags);
    }
    public override void ReceiveCustomBiomes(BinaryReader reader) {
        BitsByte flags = reader.ReadByte();
        ZoneMoon = flags[0];
        ZoneBasalt = flags[1];
    }

    public override void UpdateBiomeVisuals() {

    }

 */

