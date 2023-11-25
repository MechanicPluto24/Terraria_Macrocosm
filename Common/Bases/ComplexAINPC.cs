using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases
{
    public record StateTime(float Frames)
    {
        public float Seconds => Frames / 60f;
    }

    public abstract class ComplexAINPC<TState> : ModNPC where TState : Enum
    {
        [AttributeUsage(AttributeTargets.Method)]
        protected class StateMethodAttribute : Attribute
        {
            public TState State { get; }
            public StateMethodAttribute(TState state)
            {
                State = state;
            }
        }

        public virtual void SetDefaults2() { }

        public TState State { get => (TState)(object)(int)NPC.ai[0]; private set => NPC.ai[0] = (int)(object)value; }
        protected StateTime StateTime { get; private set; } = new(0);

        private Dictionary<TState, Action> stateMethodMap;
        private Dictionary<TState, Action> GetStateMethodMap()
        {
            Dictionary<TState, Action> methodMap = new();
            foreach (MethodInfo method in GetType().GetRuntimeMethods())
            {
                StateMethodAttribute stateMethodAttribute;
                if ((stateMethodAttribute = method.GetCustomAttribute<StateMethodAttribute>()) is null)
                {
                    continue;
                }

                methodMap[stateMethodAttribute.State] = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
            }

            return methodMap;
        }

        protected void SetState(TState state)
        {
            NPC.ai[1] = 0;
            State = state;
        }

        protected bool ValidTarget(Player player)
        {
            return player is not null && !player.dead && player.active;
        }

        protected bool ValidTarget(int whoAmI)
        {
            return whoAmI >= 0 && whoAmI < Main.player.Length && ValidTarget(Main.player[whoAmI]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>whoAmI of targeted player or -1 if no targets are valid</returns>
        protected int GetRandomTargetInRange(float range = float.MaxValue)
        {
            float rangeSQ = range * range;
            IEnumerable<int> targets = Main.player
                .Where(player => ValidTarget(player) && player.DistanceSQ(NPC.Center) < rangeSQ)
                .Select(player => player.whoAmI);

            int count = targets.Count();
            if (count == 0)
            {
                return -1;
            }

            return targets.ElementAt(Main.rand.Next(count));
        }

        public sealed override void SetDefaults()
        {
            NPC.width = 99;
            NPC.height = 99;
            NPC.damage = 99;
            NPC.defense = 9;
            NPC.lifeMax = 999;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 99f;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.scale = 1f;
            NPC.knockBackResist = 0f;

            stateMethodMap = GetStateMethodMap();

            SetDefaults2();
        }

        public sealed override void AI()
        {
            if (stateMethodMap.TryGetValue(State, out Action action))
            {
                StateTime = new(NPC.ai[1]);
                action();
                NPC.ai[1]++;
            }
        }
    }
}
