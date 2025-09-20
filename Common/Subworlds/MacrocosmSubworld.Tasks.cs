using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.Subworlds;

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

    /// <summary> The subworld generation tasks. To create a task, use the <see cref="TaskAttribute"/> </summary>
    public sealed override List<GenPass> Tasks
    {
        get
        {
            List<GenPass> tasks = new();
            foreach (MethodInfo task in GetType().GetRuntimeMethods())
            {
                TaskAttribute taskAttribute = task.GetCustomAttribute<TaskAttribute>();
                if (taskAttribute is null)
                {
                    continue;
                }

                tasks.Add(new PassLegacy(
                    task.Name,
                    (progress, configuration) =>
                    {
                        ParameterInfo[] parameters = task.GetParameters();
                        switch (parameters.Length)
                        {
                            case 0:
                                task.Invoke(this, null);
                                break;
                            case 1:
                                if (parameters[0].ParameterType == progress?.GetType())
                                    task.Invoke(this, [progress]);
                                else if (parameters[0].ParameterType == configuration?.GetType())
                                    task.Invoke(this, [configuration]);
                                else
                                    Macrocosm.Instance.Logger.Error("TaskAttribute method mismatched parameters.");
                                break;
                            case 2:
                                if (parameters[0].ParameterType == progress?.GetType() && parameters[1].ParameterType == configuration?.GetType())
                                    task.Invoke(this, [progress, configuration]);
                                else if (parameters[0].ParameterType == configuration?.GetType() && parameters[1].ParameterType == progress?.GetType())
                                    task.Invoke(this, [configuration, progress]);
                                else
                                    Macrocosm.Instance.Logger.Error("TaskAttribute method mismatched parameters.");
                                break;
                            default:
                                Macrocosm.Instance.Logger.Error("TaskAttribute method too many parameters.");
                                break;
                        }
                    },
                    taskAttribute.Weight
                 ));
            }

            return tasks;
        }
    }
}
