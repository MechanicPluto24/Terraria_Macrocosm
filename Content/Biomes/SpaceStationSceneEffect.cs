using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using SubworldLibrary;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Backgrounds.Blank;

namespace Macrocosm.Content.Biomes
{
    public class SpaceStationSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;
        public override float GetWeight(Player player) => 1f;

        public override string MapBackground => null;

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<BlankSufraceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<BlankUndergroundBackgroundStyle>();

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld");

        public override bool IsSceneEffectActive(Player player) => SubworldSystem.Current is OrbitSubworld;
    }
}
