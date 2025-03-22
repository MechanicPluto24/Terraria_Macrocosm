using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing
{
    public class RenderManager : ILoadable
    {
        private class Request
        {
            public Texture2D Texture { get; }
            public Effect Effect { get; }

            public Request(Texture2D texture, Effect effect)
            {
                Texture = texture;
                Effect = effect;
            }

            public int RequestID => GetHashCode();
            public int ParametersHash => Effect.GetParametersHashCode();
            public override int GetHashCode() => HashCode.Combine(Texture.Name.GetHashCode(), Effect.Name.GetHashCode());
        }

        private class Result : IDisposable
        {
            public Texture2D Texture { get; }
            public int ParametersID { get; }

            private static RenderTarget2D renderTarget;

            public Result(Request request)
            {
                Texture = Prepare(request);
                ParametersID = request.ParametersHash;
            }

            private RenderTarget2D Prepare(Request request)
            {
                renderTarget = new RenderTarget2D(GraphicsDevice, request.Texture.Width, request.Texture.Height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(Color.Transparent);

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, request.Effect);
                Main.spriteBatch.Draw(request.Texture, Vector2.Zero, Color.White);
                Main.spriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);
                return renderTarget;
            }

            public void Dispose()
            {
                renderTarget.Dispose();
            }
        }

        public void Load(Mod mod)
        {
            On_TilePaintSystemV2.PrepareAllRequests += On_TilePaintSystemV2_PrepareAllRequests;
        }

        public void Unload()
        {
            On_TilePaintSystemV2.PrepareAllRequests -= On_TilePaintSystemV2_PrepareAllRequests;
        }

        private void On_TilePaintSystemV2_PrepareAllRequests(On_TilePaintSystemV2.orig_PrepareAllRequests orig, TilePaintSystemV2 self)
        {
            orig(self);
            ProcessRequests();
        }

        private static GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;

        private static readonly Dictionary<int, Request> requests = new();
        private static readonly Dictionary<int, Result> results = new();

        public static Texture2D GetOrRequestProcessedTexture(Texture2D texture, params Effect[] effects)
        {
            foreach (var effect in effects)
                texture = GetOrRequestProcessedTexture(texture, effect);

            return texture;
        }

        public static Texture2D GetOrRequestProcessedTexture(Texture2D texture, Effect effect)
        {
            Request request = new(texture, effect);
            int requestId = request.RequestID;
            int parametersHash = request.ParametersHash;

            if (results.TryGetValue(requestId, out var result) && result.ParametersID == parametersHash)
                return result.Texture;

            AddRequest(requestId, request);
            return texture;
        }

        public static void AddRequest(Texture2D texture, Effect effect)
        {
            Request request = new(texture, effect);
            AddRequest(request.RequestID, request);
        }

        private static void AddRequest(int requestId, Request request)
        {
            if (!requests.ContainsKey(requestId))
                requests[requestId] = request;
        }

        private static void ProcessRequests()
        {
            if (requests.Count == 0) 
                return;

            foreach (var kvp in requests)
            {
                int hashCode = kvp.Key;
                Request request = kvp.Value;
                if(results.TryGetValue(hashCode, out Result value))
                    value.Dispose();

                results[hashCode] = new Result(request);
            }

            requests.Clear();
        }
    }
}
