using Macrocosm.Content.Items.Tools.Wiring;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.BuilderToggles;

public class MachinePowerOverlayBuilderToggle : BuilderToggle
{
    private static LocalizedText OnText { get; set; }
    private static LocalizedText OffText { get; set; }

    public static bool IsAvailable
    {
        get
        {
            Player player = Main.LocalPlayer;
            int probeType = ModContent.ItemType<CircuitProbe>();

            if (player.HasItemInInventoryOrOpenVoidBag(probeType))
                return true;

            for (int slot = 3; slot < 10; slot++)
            {
                if (player.IsItemSlotUnlockedAndUsable(slot) && player.armor[slot].type == probeType)
                    return true;
            }

            return false;
        }
    }

    public static bool IsEnabled => IsAvailable && ModContent.GetInstance<MachinePowerOverlayBuilderToggle>().CurrentState == 0;

    public override bool Active() => IsAvailable;

    public override Position OrderPosition => new After(ActuatorsVisibility);

    public override void SetStaticDefaults()
    {
        OnText = this.GetLocalization(nameof(OnText));
        OffText = this.GetLocalization(nameof(OffText));
    }

    public override string DisplayValue() => CurrentState == 0 ? OnText.Value : OffText.Value;

    public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
    {
        drawParams.Color = CurrentState == 0 ? Color.White : new Color(127, 127, 127);
        return true;
    }
}
