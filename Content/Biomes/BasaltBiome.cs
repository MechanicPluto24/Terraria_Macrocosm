using Terraria;

namespace Macrocosm.Content.Biomes {
    public class BasaltBiome : MoonBiome {
        // Inherited from the base MoonBiome

        // public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        // public override string BestiaryIcon => base.BestiaryIcon;
        // public override string BackgroundPath => base.BackgroundPath;
        // public override Color? BackgroundColor => base.BackgroundColor;
        // public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => new MoonSurfaceBgStyle();

        //public override int Music => base.Music;

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Basalt");
        }

        public override void OnInBiome(Player player) {
            base.OnInBiome(player);
            player.GetModPlayer<MacrocosmPlayer>().ZoneBasalt = true;
        }

        public override bool IsBiomeActive(Player player) {
            return MacrocosmWorld.moonBiome > 20;
        }
    }
}


