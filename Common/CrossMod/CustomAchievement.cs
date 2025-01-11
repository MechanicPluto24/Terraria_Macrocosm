using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.CrossMod
{
    /// <summary> 
    /// Achievement for the TMLAchievements mod. Autoloads texture, which should be 128x64. 
    /// <br/> Use <see cref="SetupConditions"/> to add conditions, optional, defaults to unique condition identified by the achievement name.
    /// </summary>
    public abstract class CustomAchievement : ModTexturedType
    {
        /// <summary>
        /// Order of the achievement
        /// <br/> The achievement "Timber!!" has an order of 1f, while "Champion of Terraria" has an order of 36f
        /// </summary>
        public abstract float Order { get; }

        /// <summary> The category </summary>
        public abstract AchievementCategory Category { get; }

        public override string Texture => ModContent.HasAsset(base.Texture) ? base.Texture : Macrocosm.TexturesPath + "AchievementTemplate";

        /// <summary> Path to a custom border texture, should be 72x72 </summary>
        public virtual string BorderTexture => null;

        /// <summary> Show many of the conditions are completed. </summary>
        public virtual bool ShowProgressBar => conditions.Count > 1;

        /// <summary> Show achievement card </summary>
        public virtual bool ShowCard => true;

        private readonly List<string> conditions = new();
        private readonly List<string> eventNames = new();
        private readonly Dictionary<string, float> valueEvents = new();

        // Nothing really happens on regular loading, besides registering the ModType
        public sealed override void SetupContent() => SetStaticDefaults();
        public sealed override void SetStaticDefaults() { }
        protected sealed override void Register() { }

        // After setting up content, the achievement is registered to TMLAchievements
        public void PostSetupContent()
        {
            if (ModLoader.TryGetMod("TMLAchievements", out Mod mod))
            {
                SetupConditions();

                // Fallback
                if (conditions.Count == 0)
                    AddEvent(Name);

                mod.Call("AddAchievement", Mod, Name, Category, Texture, BorderTexture, ShowProgressBar, ShowCard, Order, conditions.ToArray());
            }
        }

        protected virtual void SetupConditions() { }


        /// <summary> Add an item-collection condition </summary>
        protected void AddItemCollectCondition(int itemType) => conditions.Add($"Collect_{itemType}");

        /// <summary> Add a craft condition </summary>
        protected void AddItemCraftCondition(int itemType) => conditions.Add($"Craft_{itemType}");

        /// <summary> Add a kill condition </summary>
        protected void AddKillNPCCondition(int npcType) => conditions.Add($"Kill_{npcType}");

        /// <summary> Add a mine condition </summary>
        protected void AddMineTileCondition(int tileType) => conditions.Add($"Mine_{tileType}");

        /// <summary> Add a custom event condition </summary>
        protected void AddEvent(string eventName)
        {
            conditions.Add($"Event_{eventName}");
            eventNames.Add(eventName);
        }

        /// <summary> Add a custom value event condition </summary>
        protected void AddValueEvent(string valueEventName, float targetValue)
        {
            conditions.Add($"ValueEvent_{valueEventName}_{targetValue}");
            valueEvents.Add(valueEventName, targetValue);
        }

        /// <summary>
        /// Trigger a custom event condition that was previously added via AddEvent(...).
        /// TMLAchievements will mark that condition as complete.
        /// <br/>
        /// Example usage:
        /// <c>ModContent.GetInstance&lt;MyAchievement&gt;().TriggerEvent("MyEventName");</c>
        /// </summary>
            // <c>ModContent.GetInstance<MyAchievement>().TriggerEvent("MyEventName");</c>

        public void TriggerEvent(string eventName)
        {
            if (!eventNames.Contains(eventName))
                return;

            if (ModLoader.TryGetMod("TMLAchievements", out Mod mod))
                mod.Call("Event", eventName);
        }

        /// <inheritdoc cref="TriggerEvent(string)"/>
        public static void TriggerEvent<T>(string eventName) where T : CustomAchievement => ModContent.GetInstance<T>().TriggerEvent(eventName);

        /// <summary>
        /// Increase progress on a custom value event condition that was previously added
        /// via AddValueEvent(...). 
        /// <br/>If the total hits or exceeds the target, TMLAchievements
        /// will mark that condition as complete.
        /// <br/>
        /// Example usage:
        /// <c>ModContent.GetInstance&lt;MyAchievement&gt;().IncreaseValue("MyValueEvent", 5f);</c>
        /// </summary>
        // <c>ModContent.GetInstance<MyAchievement>().IncreaseValue("MyValueEvent", 5f);</c>
        public void IncreaseEventValue(string valueEventName, float amount)
        {
            if (!valueEvents.ContainsKey(valueEventName))
                return; 

            if (ModLoader.TryGetMod("TMLAchievements", out Mod mod))
                mod.Call("ValueEvent", valueEventName, amount);
        }

        /// <inheritdoc cref="IncreaseEventValue(string)"/>
        public static void IncreaseEventValue<T>(string eventName, float amount) where T : CustomAchievement => ModContent.GetInstance<T>().IncreaseEventValue(eventName, amount);

        /// <summary> Unlock achievement directly by triggering all its events </summary>
        public void Unlock()
        {
            foreach (string eventName in eventNames)
                TriggerEvent(eventName);

            foreach (string eventName in valueEvents.Keys)
                IncreaseEventValue(eventName, float.MaxValue);
        }

        public static void Unlock<T>() where T : CustomAchievement => ModContent.GetInstance<T>().Unlock();
    }
}
