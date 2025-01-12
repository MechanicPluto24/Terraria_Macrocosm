using Macrocosm.Common.Global.Items;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.DrawLayers
{
    class HeldItemLayer : PlayerDrawLayer
    {
        public override void SetStaticDefaults()
        {
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Item item = drawInfo.heldItem;
            if (item is null)
                return;

            if (item.TryGetGlobalItem(out CustomDrawGlobalItem result))
            {
                if (result.CustomHeldTexture != null) 
                {
                    Utility.DrawHeldItemLayer(ref drawInfo, item, result.CustomHeldTexture.Value, drawInfo.itemColor, ignoreNoUseGraphic: true);
                    if(result.CustomHeldTextureGlowmask != null)
                    {
                        Color color = result.GlowmaskColor.HasValue ? result.GlowmaskColor.Value.WithAlpha((byte)item.alpha) : drawInfo.itemColor;
                        Utility.DrawHeldItemLayer(ref drawInfo, item, result.CustomHeldTextureGlowmask.Value, color, ignoreNoUseGraphic: true);
                    }

                }
                else if (result.Glowmask != null)
                {
                    Color color = result.GlowmaskColor.HasValue ? result.GlowmaskColor.Value.WithAlpha((byte)item.alpha) : drawInfo.itemColor;
                    Utility.DrawHeldItemLayer(ref drawInfo, item, result.Glowmask.Value, color, ignoreNoUseGraphic: false);
                }
            }
        }
    }
}
