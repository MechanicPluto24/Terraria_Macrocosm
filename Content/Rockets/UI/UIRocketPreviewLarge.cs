using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Macrocosm.Content.Rockets.UI
{
    public class UIRocketPreviewLarge : UIPanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }
        public Rocket RocketDummy { get; set; }

        public string CurrentModuleName { get; private set; } = "CommandPod";

        public int CurrentModuleIndex
        {
            get => Rocket.ModuleNames.IndexOf(CurrentModuleName);
            private set => CurrentModuleName = Rocket.ModuleNames[value];
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
                if (zoomedOut && !value)
                    OnZoomedIn();

                if (!zoomedOut && value)
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

        private float[] moduleZooms = { 0.35f, 0.35f, 0.35f, 0.55f, 0.52f, 0.52f };
        private float[] moduleOffsetsX = { -220f, -220f, -220f, -80f, 40f, -250f };
        private float[] moduleOffsetsY = { 140f, -40f, -320f, -460f, -520f, -520f };

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
            bool changed = CurrentModuleName != moduleName;

            CurrentModuleName = moduleName;

            AnimationActive = !ZoomedOut;

            if (changed)
                OnModuleChange(CurrentModuleName, CurrentModuleIndex);
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
            if (CurrentModuleIndex == Rocket.Modules.Count - 1)
                SetModule(0);
            else
                SetModule(CurrentModuleIndex + 1);
        }

        public void PreviousModule()
        {
            if (CurrentModuleIndex == 0)
                SetModule(Rocket.Modules.Count - 1);
            else
                SetModule(CurrentModuleIndex - 1);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (animationCounter >= 1f)
                animationActive = false;

            if (AnimationActive)
            {
                animationCounter += 0.04f;

                float animation = Utility.QuadraticEaseInOut(animationCounter);

                if (ZoomedOut)
                {
                    moduleOffsetX = MathHelper.Lerp(moduleOffsetsX[CurrentModuleIndex], zoomedOutModuleOffsetX, animation);
                    moduleOffsetY = MathHelper.Lerp(moduleOffsetsY[CurrentModuleIndex], zoomedOutModuleOffsetY, animation);
                    zoom = MathHelper.Lerp(moduleZooms[CurrentModuleIndex], zoomedOutZoom, animation);
                }
                else
                {
                    moduleOffsetX = MathHelper.Lerp(moduleOffsetX, moduleOffsetsX[CurrentModuleIndex], animation);
                    moduleOffsetY = MathHelper.Lerp(moduleOffsetY, moduleOffsetsY[CurrentModuleIndex], animation);
                    zoom = MathHelper.Lerp(zoom, moduleZooms[CurrentModuleIndex], animation);
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

            RocketDummy.DrawDummy(spriteBatch, (GetDimensions().Position() + new Vector2(moduleOffsetX, moduleOffsetY)) * zoom, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
