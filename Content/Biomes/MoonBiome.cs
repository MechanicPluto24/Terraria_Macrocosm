using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Items.Torches;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class MoonBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override float GetWeight(Player player) => 0.5f;

        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUndergroundBackgroundStyle>();

        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/Moon";
        public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Moon";
        public override string MapBackground => BackgroundPath;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld");

        public override int BiomeTorchItemType => ModContent.ItemType<LuminiteTorch>();

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player) => SubworldSystem.IsActive<Moon>();

        public override void OnInBiome(Player player)
        {
        }

        public override void OnEnter(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }
    }
}
