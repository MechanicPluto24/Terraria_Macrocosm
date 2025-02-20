using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics;
using Terraria;

namespace Macrocosm.Content.Liquids
{
    public class Oil : ModLiquid
    {
        public override void SetStaticDefaults()
        {
            LiquidFallLength = 3;
            VisualViscosity = 160;
            DefaultOpacity = 0.95f;
            AddMapEntry(Color.Black, CreateMapEntryName());
        }

        public override bool PreDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
        {
            return true;
        }

        public override bool PreSlopeDraw(int i, int j, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
        {
            return true;
        }

        public override bool PreRetroDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return true;
        }

        public override void RetroDrawEffects(int i, int j, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
        {
        }
    }
}
