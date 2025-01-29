using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Macrocosm.Content.Rockets.UI.Customization
{
    public class UIRocketPreviewLarge : UIPanel, IRocketUIDataConsumer, IFixedUpdateable
    {
        public Rocket Rocket { get; set; } = new();
        public Rocket RocketDummy { get; set; }

        public string CurrentModuleName { get; private set; } = "CommandPod";

        public int CurrentModuleIndex
        {
            get => Rocket.ModuleTemplateNames.IndexOf(CurrentModuleName);
            private set => CurrentModuleName = Rocket.ModuleTemplateNames[value];
        }

        private bool animationActive = false;
        public bool AnimationActive
        {
            get => animationActive;
            set
            {
                animationCounter = 0f;
                animationActive = value;
            }
        }


        private bool zoomedOut = false;
        public bool ZoomedOut
        {
            get => zoomedOut;
            set
            {
                bool prevValue = zoomedOut;
                zoomedOut = value;
                AnimationActive = true;

                if (prevValue && !value)
                    OnZoomedIn();

                if (!prevValue && value)
                    OnZoomedOut();

                zoomedOut = value;
                AnimationActive = true;
            }
        }

        public Action<string, int> OnModuleChange { get; set; } = (_, _) => { };
        public Action OnZoomedIn { get; set; } = () => { };
        public Action OnZoomedOut { get; set; } = () => { };

        //private int lastModuleIndex;
        private float animationCounter;
        private float moduleOffsetX;
        private float moduleOffsetY;
        private float zoom;

        private float zoomedOutModuleOffsetX = 30f;
        private float zoomedOutModuleOffsetY = 15f;
        private float zoomedOutZoom = 1f;

        private Vector2 GetModuleOffset(string moduleName)
        {
            return moduleName switch
            {
                "CommandPod" => new Vector2(-220f, 140f),
                "PayloadPod" => new Vector2(-220f, 140f),
                "ServiceModule" => new Vector2(-220f, -40f),
                "UnmannedTug" => new Vector2(-220f, -40f),
                "ReactorModule" => new Vector2(-220f, -320f),
                "EngineModule" => new Vector2(-80f, -460f),
                "BoosterLeft" => new Vector2(40f, -520f),
                "BoosterRight" => new Vector2(-250f, -520f),
                _ => new Vector2(0f, 0f)
            };
        }

        private float GetModuleZoom(string moduleName)
        {
            return moduleName switch
            {
                "CommandPod" => 0.35f,
                "PayloadPod" => 0.35f,
                "ServiceModule" => 0.35f,
                "UnmannedTug" => 0.35f,
                "ReactorModule" => 0.35f,
                "EngineModule" => 0.55f,
                "BoosterLeft" => 0.52f,
                "BoosterRight" => 0.52f,
                _ => 0.35f 
            };
        }

        public UIRocketPreviewLarge()
        {
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = Color.Transparent;
        }

        public void SetModule(string moduleName)
        {
            var index = Rocket.Modules.ToList().FindIndex(m => m.Name == moduleName && m.Active);
            if (index != -1)
            {
                bool changed = CurrentModuleName != moduleName;

                CurrentModuleName = moduleName;
                AnimationActive = !ZoomedOut;

                if (changed)
                    OnModuleChange(CurrentModuleName, CurrentModuleIndex);
            }
        }

        public void SetModule(int moduleIndex)
        {
            bool changed = CurrentModuleIndex != moduleIndex;

            CurrentModuleIndex = moduleIndex;
            AnimationActive = !ZoomedOut;

            if (changed)
                OnModuleChange(CurrentModuleName, CurrentModuleIndex);
        }

        public void NextModule()
        {
            int currentIndex = CurrentModuleIndex;
            if (currentIndex == -1 || currentIndex == Rocket.Modules.Length - 1)
                SetModule(0);
            else
                SetModule(currentIndex + 1);
        }

        public void PreviousModule()
        {
            int currentIndex = CurrentModuleIndex;
            if (currentIndex == -1 || currentIndex == 0)
                SetModule(Rocket.Modules.Length - 1);
            else
                SetModule(currentIndex - 1);
        }

        // Use for animation
        public void FixedUpdate()
        {
            if (animationCounter >= 1f)
                animationActive = false;

            if (AnimationActive)
            {
                animationCounter += 0.04f;

                float animation = Utility.QuadraticEaseInOut(animationCounter);

                Vector2 targetOffset = GetModuleOffset(CurrentModuleName);
                float targetZoom = GetModuleZoom(CurrentModuleName);
                if (ZoomedOut)
                {
                    moduleOffsetX = MathHelper.Lerp(targetOffset.X, zoomedOutModuleOffsetX, animation);
                    moduleOffsetY = MathHelper.Lerp(targetOffset.Y, zoomedOutModuleOffsetY, animation);
                    zoom = MathHelper.Lerp(targetZoom, zoomedOutZoom, animation);
                }
                else
                {
                    moduleOffsetX = MathHelper.Lerp(moduleOffsetX, targetOffset.X, animation);
                    moduleOffsetY = MathHelper.Lerp(moduleOffsetY, targetOffset.Y, animation);
                    zoom = MathHelper.Lerp(zoom, targetZoom, animation);
                }
            }
        }

        private SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Matrix matrix = Matrix.CreateScale(Main.UIScale / zoom, Main.UIScale / zoom, 1f);

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(state.SpriteSortMode, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, state.DepthStencilState, CustomRasterizerStates.ScissorTest, state.Effect, matrix);

            RocketDummy.Draw(Rocket.DrawMode.Dummy, spriteBatch, (GetDimensions().Position() + new Vector2(moduleOffsetX, moduleOffsetY)) * zoom, useRenderTarget: false);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
