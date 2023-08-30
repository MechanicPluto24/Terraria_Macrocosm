using Macrocosm.Common.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria;

namespace Macrocosm.Common.Utils
{ 
	public static partial class Utility
    {
		/// <summary> Saves the SpriteBatch parameters. Try to store the state and call this only once per object instance. </summary>
		public static SpriteBatchState SaveState(this SpriteBatch spriteBatch)
        {
            Main.NewText("Saving state");
            if (spriteBatch.BeginCalled())
            {
                var type = spriteBatch.GetType();
				return new SpriteBatchState(
				   true,
				   (SpriteSortMode)type.GetField("sortMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
				   (BlendState)type.GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
				   (SamplerState)type.GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
				   (DepthStencilState)type.GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
				   (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
				   (Effect)type.GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
				   (Matrix)type.GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch)
			   );
			}
               
            else
                return new SpriteBatchState(
                    false,
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    Main.DefaultSamplerState,
                    DepthStencilState.Default,
                    RasterizerState.CullCounterClockwise,
                    null,
                    Main.GameViewMatrix.TransformationMatrix
                );
        }

        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

		public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SpriteBatchState state)
			=> spriteBatch.Begin(state.SpriteSortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

		public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SamplerState samplerState, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, samplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

		public static void Begin(this SpriteBatch spriteBatch, SamplerState samplerState, SpriteBatchState state)
		   => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, samplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

		public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        /// <summary> Begins the SpriteBatch with the parameters stored in a SpriteBatchState </summary>
        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state)
                => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        public static void EndIfBeginCalled(this SpriteBatch spriteBatch)
        {
            if (spriteBatch.BeginCalled())
                spriteBatch.End();
        }

        public static bool BeginCalled(this SpriteBatch spriteBatch)
            => (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
    }
}