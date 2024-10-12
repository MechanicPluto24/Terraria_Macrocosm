using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing.Sky
{
    public abstract class MacrocosmSkyEntity : ModType
    {
        protected sealed override void Register()
        {
            ModTypeLookup<MacrocosmSkyEntity>.Register(this);
        }

        public sealed override void SetupContent() => SetStaticDefaults();

        public Vector2 Position;
        public Asset<Texture2D> Texture;
        public SpriteFrame Frame;
        public float Depth;
        public SpriteEffects Effects;
        public bool IsActive = true;
        public float Rotation;

        public virtual string TexturePath => this.GetNamespacePath();
        public Rectangle SourceRectangle => Frame.GetSourceRectangle(Texture.Value);

        protected Player player;
        protected FastRandom random;

        protected MacrocosmSkyEntity(Player player, FastRandom random)
        {
            this.player = player;
            this.random = random;
            Texture = ModContent.Request<Texture2D>(TexturePath);
        }

        public virtual List<MacrocosmSkyEntity> CreateGroup(Player player, FastRandom random)
        {
            return new List<MacrocosmSkyEntity> { this };
        }

        protected void NextFrame()
        {
            Frame.CurrentRow = (byte)((Frame.CurrentRow + 1) % Frame.RowCount);
        }

        public abstract Color GetColor(Color backgroundColor);

        public abstract void Update(GameTime gameTime, int frameCount);

        protected void SetPositionInWorldBasedOnScreenSpace(Vector2 actualWorldSpace)
        {
            Vector2 vector = actualWorldSpace - Main.Camera.Center;
            Vector2 position = Main.Camera.Center + vector * (Depth / 3f);
            Position = position;
        }

        public abstract Vector2 GetDrawPosition();

        public virtual void Draw(SpriteBatch spriteBatch, float depthScale, float minDepth, float maxDepth)
        {
            CommonDraw(spriteBatch, depthScale, minDepth, maxDepth);
        }

        public void CommonDraw(SpriteBatch spriteBatch, float depthScale, float minDepth, float maxDepth)
        {
            if (!(Depth <= minDepth) && !(Depth > maxDepth))
            {
                Vector2 drawPositionByDepth = GetDrawPositionByDepth();
                Color color = GetColor(Main.ColorOfTheSkies) * Main.atmo;
                Vector2 origin = SourceRectangle.Size() / 2f;
                float scale = depthScale / Depth;
                spriteBatch.Draw(Texture.Value, drawPositionByDepth - Main.Camera.UnscaledPosition, SourceRectangle, color, Rotation, origin, scale, Effects, 0f);
            }
        }

        internal Vector2 GetDrawPositionByDepth() => (GetDrawPosition() - Main.Camera.Center) * new Vector2(1f / Depth, 0.9f / Depth) + Main.Camera.Center;
    }
}
