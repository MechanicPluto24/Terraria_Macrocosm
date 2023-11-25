using Macrocosm.Content.Rockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.DataStructures
{
    [Obsolete("Use the TaskAttribute in a MacrocosmSubworld sub-class.", false)]
    public abstract class GenPassCollection
    {
        private PassLegacy CreateGenPass(MethodInfo methodInfo)
        {
            return new(
                methodInfo.Name,
                (GenerationProgress progress, GameConfiguration configuration) =>
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
                                Macrocosm.Instance.Logger.Error("GenPassAttribute method mismatched parameters.");
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
                                Macrocosm.Instance.Logger.Error("GenPassAttribute method mismatched parameters.");
                            }
                            break;
                        default:
                            Macrocosm.Instance.Logger.Error("GenPassAttribute method too many parameters.");
                            break;
                    }
                }
            );
        }

        private bool TryInsertGenPass(List<GenPass> tasks, GenPassAttribute genPassAttribute, MethodInfo methodInfo)
        {
            if (tasks.Any(genPass => genPass.Name == methodInfo.Name))
            {
                return false;
            }

            PassLegacy pass = CreateGenPass(methodInfo);
            if (genPassAttribute.InsertName == null)
            {
                if (genPassAttribute.InsertMode == InsertMode.Before)
                {
                    tasks.Insert(0, pass);
                }
                else
                {
                    tasks.Add(pass);
                }

                return true;
            }

            int index = tasks.FindIndex(genPass => genPass.Name == genPassAttribute.InsertName);
            if (index == -1)
            {
                Macrocosm.Instance.Logger.Error("GenPassAttribute GenPass name not found.");
                return false;
            }

            tasks.Insert(
                genPassAttribute.InsertMode switch { InsertMode.Before => index, _ => index + 1 },
                pass
            );


            return true;
        }

        public List<GenPass> Tasks
        {
            get
            {
                (GenPassAttribute, MethodInfo)? firstGenPassAttribute = null;
                (GenPassAttribute, MethodInfo)? lastGenPassAttribute = null;
                (GenPassAttribute, MethodInfo)? postGenPassAttribute = null;
                List<(GenPassAttribute, MethodInfo)> genPassAttributes = new();
                foreach (MethodInfo methodInfo in GetType().GetRuntimeMethods())
                {
                    GenPassAttribute genPassAttribute;
                    if ((genPassAttribute = methodInfo.GetCustomAttribute<GenPassAttribute>()) is null)
                    {
                        continue;
                    }

                    if (genPassAttribute.InsertMode == InsertMode.First)
                    {
                        if (firstGenPassAttribute is not null)
                        {
                            Macrocosm.Instance.Logger.Error("More than one first GenPass.");
                        }

                        firstGenPassAttribute = (genPassAttribute, methodInfo);
                        continue;
                    }
                    else if (genPassAttribute.InsertMode == InsertMode.Last)
                    {
                        if (lastGenPassAttribute is not null)
                        {
                            Macrocosm.Instance.Logger.Error("More than one last GenPass.");
                        }

                        lastGenPassAttribute = (genPassAttribute, methodInfo);
                        continue;
                    }
                    else if (genPassAttribute.InsertMode == InsertMode.PostGen)
                    {
                        postGenPassAttribute = (genPassAttribute, methodInfo);
                    }

                    genPassAttributes.Add((genPassAttribute, methodInfo));
                }

                List<GenPass> tasks = new();
                if (firstGenPassAttribute.HasValue)
                {
                    tasks.Add(CreateGenPass(firstGenPassAttribute.Value.Item2));
                }

                bool staysSame = false;
                while (!staysSame)
                {
                    staysSame = true;
                    foreach ((GenPassAttribute genPassAttribute, MethodInfo methodInfo) in genPassAttributes)
                    {
                        if (TryInsertGenPass(tasks, genPassAttribute, methodInfo))
                        {
                            staysSame = false;
                        }
                    }
                }

                if (lastGenPassAttribute.HasValue)
                {
                    tasks.Add(CreateGenPass(lastGenPassAttribute.Value.Item2));
                }

                if (postGenPassAttribute.HasValue)
                {
                    tasks.Add(CreateGenPass(postGenPassAttribute.Value.Item2));
                }

                if (tasks.Count < genPassAttributes.Count)
                {
                    Macrocosm.Instance.Logger.Error("Error while inserting passes. Missed some passes.");
                }

                return tasks;
            }
        }

        [GenPass(InsertMode.PostGen)]
        public void CommonPostSubworldGenHooks(GenerationProgress progress)
        {
            RocketManager.OnWorldGenerated();
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class GenPassAttribute : Attribute
    {
        public string InsertName { get; }
        public InsertMode InsertMode { get; }
        public double LoadWeight { get; }

        public GenPassAttribute(string insertName, InsertMode insertMode, double loadWeight = 0.0)
        {
            InsertName = insertName;
            InsertMode = insertMode;
            LoadWeight = loadWeight;
        }

        public GenPassAttribute(InsertMode insertMode, double loadWeight = 0.0) : this(null, insertMode, loadWeight) { }
    }

    public enum InsertMode
    {
        Before,
        After,
        First,
        Last,
        PostGen
    }
}
