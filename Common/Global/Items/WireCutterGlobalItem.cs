using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class WireCutterGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WireCutter;

        public override bool? UseItem(Item item, Player player)
        {
            Point targetCoords = player.TargetCoords();
            if (ConveyorSystem.TryRemove(targetCoords.X, targetCoords.Y))
            {
                return true;
            }

            return null;
        }
    }
}
