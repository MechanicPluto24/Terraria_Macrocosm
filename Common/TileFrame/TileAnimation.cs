using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using System;
using System.Linq;

namespace Macrocosm.Common.TileFrame
{
    public class TileAnimation : ILoadable
    {
        public const int VanillaAnimationTypeCount = 5;

        private static readonly Dictionary<int, AnimationData> customAnimations = [];

        private readonly struct AnimationData(int frameMax, int frameCounterMax, int[] frameData, int index = int.MinValue)
        {
            // NOTE: Excluded from HashCode and Equals!
            public readonly int Type = index;

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

        public static bool GetTemporaryFrame(int x, int y, out int frameData) => Animation.GetTemporaryFrame(x, y, out frameData);
        public static void RemoveTemporaryAnimation(int x, int y) => typeof(Animation).InvokeStaticMethod("RemoveTemporaryAnimation", (short)x, (short)y);
        public static void NewTemporaryAnimation(int type, ushort tileType, int x, int y, bool forceUpdate = true)
        {
            Animation.NewTemporaryAnimation(type, tileType, x, y);

            if(forceUpdate)
                Animation.UpdateAll();
        }


        public static int RegisterTileAnimation(int frameMax, int frameCounterMax, int[] frameData)
        {
            AnimationData input = new(frameMax, frameCounterMax, frameData);
            foreach (var item in customAnimations.Values)
                 if (item.Equals(input))  
                     return item.Type; 
 
            int key = VanillaAnimationTypeCount + customAnimations.Count;
            AnimationData data = new(frameMax, frameCounterMax, frameData, key);
            customAnimations[key] = data;
            return key;
        }

        public void Load(Mod mod)
        {
            On_Animation.SetDefaults += On_Animation_SetDefaults;
        }

        public void Unload()
        {
            On_Animation.SetDefaults -= On_Animation_SetDefaults;
        }

        private void On_Animation_SetDefaults(On_Animation.orig_SetDefaults orig, Animation self, int type)
        {
            orig(self, type);

            if (customAnimations.ContainsKey(type))
            {
                typeof(Animation).SetFieldValue("_frameMax", customAnimations[type].FrameMax, self);
                typeof(Animation).SetFieldValue("_frameCounterMax", customAnimations[type].FrameCounterMax, self);
                typeof(Animation).SetFieldValue("_frameData", customAnimations[type].FrameData, self);
            }
        }

    }
}
