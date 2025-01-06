using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class OrbitSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override string MapBackground => Macrocosm.TexturesPath + "MapBackgrounds/Space";
        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/OrbitDay") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/OrbitNight");

        public override bool IsSceneEffectActive(Player player) => OrbitSubworld.IsOrbitSubworld(MacrocosmSubworld.CurrentID);
    }
}
