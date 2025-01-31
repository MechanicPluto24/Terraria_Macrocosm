using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
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

        public string CurrentModuleName => Rocket.Modules[CurrentModuleIndex].Name;
        public int CurrentModuleIndex { get; set; } = 0;

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

        public Action<int> OnModuleChange { get; set; } = (_) => { };
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
            return default;
        }

        private float GetModuleZoom(string moduleName)
        {
            return 0;
        }

        private Vector2 GetModuleOffset(int slot)
        {
            return slot switch
            {
                0 => new Vector2(-220f, 140f),
                1 => new Vector2(-220f, -40f),
                2 => new Vector2(-220f, -320f),
                3 => new Vector2(-80f, -460f),
                4 => new Vector2(40f, -520f),
                5 => new Vector2(-250f, -520f),
                _ => new Vector2(0f, 0f)
            };
        }

        private float GetModuleZoom(int slot)
        {
            return slot switch
            {
                 0 => 0.35f,
                 1 => 0.35f,
                 2 => 0.35f,
                 3 => 0.55f,
                 4 => 0.52f,
                 5 => 0.52f,
                _ => 0f
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
            var index = Rocket.Modules.ToList().FindIndex(m => m.Name == moduleName);
            if (index != -1)
            {
                CurrentModuleIndex = index;
                bool changed = CurrentModuleName != moduleName;
                AnimationActive = !ZoomedOut;

                if (changed)
                    OnModuleChange( CurrentModuleIndex);
            }
        }

        public void SetModule(int moduleIndex)
        {
            bool changed = CurrentModuleIndex != moduleIndex;

            CurrentModuleIndex = moduleIndex;
            AnimationActive = !ZoomedOut;

            if (changed)
                OnModuleChange(CurrentModuleIndex);
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

                Vector2 targetOffset = GetModuleOffset(CurrentModuleIndex);
                float targetZoom = GetModuleZoom(CurrentModuleIndex);
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
