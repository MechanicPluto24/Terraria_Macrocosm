using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria;
using System.Linq;
using System;

namespace Macrocosm.Common.TileFrame
{
    public readonly struct AnimationData(int frameMax, int frameCounterMax, int[] frameData, bool forcedUpdate = false)
    {
        public readonly bool ForcedUpdate = forcedUpdate;

        public readonly int FrameMax = frameMax;
        public readonly int FrameCounterMax = frameCounterMax;
        public readonly int[] FrameData = frameData;

        public override int GetHashCode() => HashCode.Combine(ForcedUpdate, FrameMax, FrameCounterMax, FrameData);

        public override bool Equals(object obj)
        {
            return obj is AnimationData data &&
                   ForcedUpdate == data.ForcedUpdate &&
                   FrameMax == data.FrameMax &&
                   FrameCounterMax == data.FrameCounterMax &&
                   Enumerable.SequenceEqual(FrameData, data.FrameData);
        }

        public static bool operator ==(AnimationData left, AnimationData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AnimationData left, AnimationData right)
        {
            return !(left == right);
        }
    }

    public class TileAnimation : ILoadable
    { 
        private static List<TileAnimation> animations;
        private static Dictionary<Point16, TileAnimation> temporaryAnimations;
        private static List<Point16> awaitingRemoval;
        private static List<TileAnimation> awaitingAddition;

        private bool temporary;
        private Point16 coordinates;
        private ushort tileType;
        private ushort[] otherAcceptedTileTypes;
        private int frame;
        private int frameMax;
        private int frameCounter;
        private int frameCounterMax;
        private int[] frameData;

        public void Load(Mod mod)
        {
            animations = [];
            temporaryAnimations = [];
            awaitingRemoval = [];
            awaitingAddition = [];

            On_Animation.UpdateAll += On_Animation_UpdateAll;
        }

        public void Unload()
        {
            animations = null;
            temporaryAnimations = null;
            awaitingRemoval = null;
            awaitingAddition = null;

            On_Animation.UpdateAll -= On_Animation_UpdateAll;
        }

        public static void NewTemporaryAnimation(AnimationData data, int x, int y, ushort tileType, params ushort[] otherAcceptedTileTypes)
            => NewTemporaryAnimation(data.FrameMax, data.FrameCounterMax, data.FrameData, data.ForcedUpdate, x, y, tileType, otherAcceptedTileTypes);

        public static void NewTemporaryAnimation(int frameMax, int frameCounterMax, int[] frameData, bool forcedUpdate, int x, int y, ushort tileType, params ushort[] otherAcceptedTileTypes)
        {
            Point16 coordinates = new(x, y);
            if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
            {
                TileAnimation animation = new()
                {
                    frameMax = frameMax,
                    frameCounterMax = frameCounterMax,
                    frameData = frameData,

                    frame = 0,
                    frameCounter = 0,

                    tileType = tileType,
                    otherAcceptedTileTypes = otherAcceptedTileTypes,

                    coordinates = coordinates,
                    temporary = true
                };

                if (forcedUpdate)
                {
                    temporaryAnimations[animation.coordinates] = animation;
                    temporaryAnimations[animation.coordinates].Update();
                }
                else
                {
                    awaitingAddition.Add(animation);
                }

                // TODO: netcode
                /*
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTemporaryAnimation(-1, type, tileType, x, y);
                }
                */
            }
        }

        private static void RemoveTemporaryAnimation(short x, short y)
        {
            Point16 point = new(x, y);
            if (temporaryAnimations.ContainsKey(point))
            {
                awaitingRemoval.Add(point);
            }
        }

        private void On_Animation_UpdateAll(On_Animation.orig_UpdateAll orig)
        {
            orig();
            UpdateAll();
        }

        private static void UpdateAll()
        {
            for (int i = 0; i < animations.Count; i++)
                 animations[i].Update();
 
            if (awaitingAddition.Count > 0)
            {
                for (int j = 0; j < awaitingAddition.Count; j++)
                {
                    TileAnimation animation = awaitingAddition[j];
                    temporaryAnimations[animation.coordinates] = animation;
                }

                awaitingAddition.Clear();
            }

            foreach (var temporaryAnimation in temporaryAnimations)
            {
                temporaryAnimation.Value.Update();
            }

            if (awaitingRemoval.Count > 0)
            {
                for (int k = 0; k < awaitingRemoval.Count; k++)
                {
                    temporaryAnimations.Remove(awaitingRemoval[k]);
                }

                awaitingRemoval.Clear();
            }
        }

        public void Update()
        {
            if (temporary)
            {
                Tile tile = Main.tile[coordinates.X, coordinates.Y];
                bool isCorrectType = tile.TileType == tileType;

                if (!isCorrectType)
                {
                    foreach (ushort otherType in otherAcceptedTileTypes)
                    {
                        if (tile.TileType == otherType)
                            isCorrectType = true;
                    }
                }
               
                if (!isCorrectType)
                {
                    RemoveTemporaryAnimation(coordinates.X, coordinates.Y);
                    return;
                }
            }

            frameCounter++;
            if (frameCounter < frameCounterMax)
            {
                return;
            }

            frameCounter = 0;
            frame++;
            if (frame >= frameMax)
            {
                frame = 0;
                if (temporary)
                {
                    RemoveTemporaryAnimation(coordinates.X, coordinates.Y);
                }
            }
        }

        public static bool GetTemporaryFrame(int x, int y, out int frameData)
        {
            Point16 key = new(x, y);
            if (!temporaryAnimations.TryGetValue(key, out var value))
            {
                frameData = 0;
                return false;
            }

            frameData = value.frameData[value.frame];
            return true;
        }
    }

    /*
    public class TileAnimation : ILoadable
    {
        public const int Terraria_Animation_TypeCount = 5;

        private static readonly Dictionary<int, AnimationData> customAnimations = [];

        private readonly struct AnimationData(int frameMax, int frameCounterMax, int[] frameData)
        {
            public readonly int FrameMax = frameMax;
            public readonly int FrameCounterMax = frameCounterMax;
            public readonly int[] FrameData = frameData;

            public override int GetHashCode() => HashCode.Combine(FrameMax, FrameCounterMax, FrameData);

            public override bool Equals(object obj)
            {
                return obj is AnimationData data &&
                       FrameMax == data.FrameMax &&
                       FrameCounterMax == data.FrameCounterMax &&
                       Enumerable.SequenceEqual(FrameData, data.FrameData);
            }
        }

        public static bool GetTemporaryFrame(int x, int y, out int frameData) => Terraria.Animation.GetTemporaryFrame(x, y, out frameData);
        public static void RemoveTemporaryAnimation(int x, int y) => typeof(Terraria.Animation).InvokeStaticMethod("RemoveTemporaryAnimation", (short)x, (short)y);
        public static void NewTemporaryAnimation(int type, ushort tileType, int x, int y, bool existenceCheck = true, bool forcedAddition = true)
        {
            if(!(existenceCheck && Terraria.Animation.GetTemporaryFrame(x,y, out _)))
            {
                Terraria.Animation.NewTemporaryAnimation(type, tileType, x, y);

                if (forcedAddition)
                {
                    ForceAddPendingTemporaryAnimations();
                    ForceUpdateTemporaryAnimations1();
                }
            }
        }

        public static int RegisterTileAnimation(int frameMax, int frameCounterMax, int[] frameData)
        {
            AnimationData input = new(frameMax, frameCounterMax, frameData);
            foreach (var kvp in customAnimations)
                if (kvp.Value.Equals(input))
                    return kvp.Key;

            int key = Terraria_Animation_TypeCount + customAnimations.Count;
            AnimationData data = new(frameMax, frameCounterMax, frameData);
            customAnimations[key] = data;
            return key;
        }

        private static void ForceAddPendingTemporaryAnimations()
        {
            Dictionary<Point16, Terraria.Animation> temporaryAnimations = (Dictionary<Point16, Terraria.Animation>)typeof(Terraria.Animation).GetFieldValue("_temporaryAnimations");
            List<Terraria.Animation> awaitingAddition = typeof(Terraria.Animation).GetFieldValue<List<Terraria.Animation>>("_awaitingAddition");
            if (awaitingAddition.Count > 0)
            {
                for (int j = 0; j < awaitingAddition.Count; j++)
                {
                    Terraria.Animation animation = awaitingAddition[j];
                    Point16 coordinates = typeof(Terraria.Animation).GetFieldValue<Point16>("_coordinates", animation);
                    temporaryAnimations[coordinates] = animation;
                }

                awaitingAddition.Clear();
            }
        }

        private void ForceUpdateAnimations()
        {
            List<Terraria.Animation> animations = typeof(Terraria.Animation).GetFieldValue<List<Terraria.Animation>>("_animations");
            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].Update();
            }
        }

        private static void ForceUpdateTemporaryAnimations1()
        {
            Dictionary<Point16, Terraria.Animation> temporaryAnimations = typeof(Terraria.Animation).GetFieldValue<Dictionary<Point16, Terraria.Animation>>("_temporaryAnimations");
            foreach (var kvp in temporaryAnimations)
            {
                kvp.Value.Update();
            }
        }

        private void ForceUpdateTemporaryAnimations()
        {
            Dictionary<Point16, Terraria.Animation> temporaryAnimations = typeof(Terraria.Animation).GetFieldValue<Dictionary<Point16, Terraria.Animation>>("_temporaryAnimations");
            foreach (var kvp in temporaryAnimations)
            {
                kvp.Value.Update();
            }
        }

        public void Load(Mod mod)
        {
            Terraria.On_Animation.SetDefaults += On_Animation_SetDefaults;
            Terraria.On_Animation.RemoveTemporaryAnimation += On_Animation_RemoveTemporaryAnimation; ;
        }

        public void Unload()
        {
            Terraria.On_Animation.SetDefaults -= On_Animation_SetDefaults;
            Terraria.On_Animation.RemoveTemporaryAnimation -= On_Animation_RemoveTemporaryAnimation; ;
        }

        private void On_Animation_SetDefaults(Terraria.On_Animation.orig_SetDefaults orig, Terraria.Animation self, int type)
        {
            orig(self, type);

            if (customAnimations.ContainsKey(type))
            {
                typeof(Terraria.Animation).SetFieldValue("_frameMax", customAnimations[type].FrameMax, self);
                typeof(Terraria.Animation).SetFieldValue("_frameCounterMax", customAnimations[type].FrameCounterMax, self);
                typeof(Terraria.Animation).SetFieldValue("_frameData", customAnimations[type].FrameData, self);
            }
        }

        private void On_Animation_RemoveTemporaryAnimation(Terraria.On_Animation.orig_RemoveTemporaryAnimation orig, short x, short y)
        {
            orig(x, y);
        }
    }
    */
}
