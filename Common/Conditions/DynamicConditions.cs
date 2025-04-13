using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Localization;

namespace Macrocosm.Common.Conditions
{
    public static class DynamicConditions
    {
        private static readonly Dictionary<(Type, string), Condition> _conditionCache = new();
        public static Condition Get<T>(string memberName) => Get(typeof(T), memberName);

        public static Condition Get(Type type, string memberName)
        {
            var key = (type, memberName);
            if (_conditionCache.TryGetValue(key, out var condition))
                return condition;

            var prop = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static);
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                var getter = prop.GetGetMethod();
                bool predicate() => (bool)getter.Invoke(null, null); 
                return Create(type, memberName, predicate, key);
            }

            var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Static);
            if (field != null && field.FieldType == typeof(bool))
            {
                bool predicate() => (bool)field.GetValue(null);
                return Create(type, memberName, predicate, key);
            }

            throw new ArgumentException($"No public static bool property or field named '{memberName}' found in {type.FullName}");
        }

        public static bool TryGet<T>(string memberName, out Condition condition)
        {
            try
            {
                condition = Get<T>(memberName);
                return true;
            }
            catch
            {
                condition = null;
                return false;
            }
        }

        private static Condition Create(Type type, string memberName, Func<bool> predicate, (Type, string) key)
        {
            string localizationKey = $"Mods.Macrocosm.Conditions.{memberName}";
            var condition = new Condition(localizationKey, predicate);
            _conditionCache[key] = condition;
            _ = Language.GetOrRegister(localizationKey);
            return condition;
        }
    }
}
