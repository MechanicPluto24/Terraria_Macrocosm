using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Systems;

namespace Macrocosm.Content.Biomes
{
    public class IndoorSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override float GetWeight(Player player) => 1f;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Indoors");

        public override bool IsSceneEffectActive(Player player) => SubworldSystem.Current is MacrocosmSubworld subworld && RoomOxygenSystem.CheckRoomOxygen(player.Center);
    }
}
