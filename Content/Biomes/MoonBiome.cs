using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Macrocosm.Backgrounds;
using Macrocosm.Content.Subworlds.Moon;

namespace Macrocosm.Content.Biomes
{

    //TODO: add music, sky, whatever left 

    public class MoonBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "Assets/FilterIcons/Moon";
        public override string BackgroundPath => "Assets/Map/Moon";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBgStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();

        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MoonDay") : MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MoonNight");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Moon");
        }
 
        public override void OnInBiome(Player player)
        {
            player.GetModPlayer<MacrocosmPlayer>().ZoneMoon = true;
        }

        public override void OnLeave(Player player)
        {
            player.GetModPlayer<MacrocosmPlayer>().ZoneMoon = false;
        }
        
        public override bool IsBiomeActive(Player player)
        {
            return SubworldSystem.IsActive<Moon>();
        }
    }
}

// The old code in MacrocosmPlayer

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

