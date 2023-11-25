using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.Subworlds
{
    public partial class MacrocosmSubworld
    {
        [AttributeUsage(AttributeTargets.Method)]
        protected class TaskAttribute : Attribute
        {
            public double Weight { get; }
            public TaskAttribute(double weight = 1d)
            {
                Weight = weight;
            }
        }

        /// <summary> The structure map of this subworld. Does not save, use only in <see cref="TaskAttribute"> Task</see>s </summary>
        public StructureMap StructureMap { get; private set; } = new();

        /// <summary> The subworld generation tasks. To create a task, use the <see cref="TaskAttribute"/> </summary>
        public sealed override List<GenPass> Tasks
        {
            get
            {
                List<GenPass> tasks = new();
                foreach (MethodInfo methodInfo in GetType().GetRuntimeMethods())
                {
                    TaskAttribute taskAttribute = methodInfo.GetCustomAttribute<TaskAttribute>();
                    if (taskAttribute is null)
                    {
                        continue;
                    }

                    tasks.Add(new PassLegacy(
                        methodInfo.Name,
                        (progress, configuration) =>
                        {
                            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                            switch (parameterInfos.Length)
                            {
                                case 0:
                                    methodInfo.Invoke(this, null);
                                    break;
                                case 1:
                                    if (parameterInfos[0].ParameterType == progress?.GetType())
                                    {
                                        methodInfo.Invoke(this, new object[] { progress });
                                    }
                                    else if (parameterInfos[0].ParameterType == configuration?.GetType())
                                    {
                                        methodInfo.Invoke(this, new object[] { configuration });
                                    }
                                    else
                                    {
                                        Macrocosm.Instance.Logger.Error("TaskAttribute method mismatched parameters.");
                                    }
                                    break;
                                case 2:
                                    if (parameterInfos[0].ParameterType == progress?.GetType() && parameterInfos[1].ParameterType == configuration?.GetType())
                                    {
                                        methodInfo.Invoke(this, new object[] { progress, configuration });
                                    }
                                    else if (parameterInfos[0].ParameterType == configuration?.GetType() && parameterInfos[1].ParameterType == progress?.GetType())
                                    {
                                        methodInfo.Invoke(this, new object[] { configuration, progress });
                                    }
                                    else
                                    {
                                        Macrocosm.Instance.Logger.Error("TaskAttribute method mismatched parameters.");
                                    }
                                    break;
                                default:
                                    Macrocosm.Instance.Logger.Error("TaskAttribute method too many parameters.");
                                    break;
                            }
                        },
                        taskAttribute.Weight
                     ));
                }

                // (Re)construct structure map of this subworld when SubLib fetches worldgen tasks (i.e. subworld generation commences)
                StructureMap = new();

                return tasks;
            }
        }
    }
}
