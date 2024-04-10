using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Items.Materials.Tech;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Macrocosm.Content.Items.Blocks;
using System;
using System.Linq;

namespace Macrocosm.Content.Rockets.UI
{
    public class UIAssemblyTab : UIPanel, ITabUIElement
    {
        public LaunchPad LaunchPad { get; set; } = new();

        private Rocket Rocket => LaunchPad.Rocket;
        private Inventory Inventory => LaunchPad.Inventory;

        public UIAssemblyTab()
        {
        }

        private Asset<Texture2D> GetBlueprint<T>() where T : ModItem
            => ModContent.RequestIfExists(ModContent.GetInstance<T>().Texture + "_Blueprint", out Asset<Texture2D> blueprint) ? blueprint : null;

        private int[] assemblyRequiredAmountsBySlotId;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            SetPadding(3f);

            BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
            BorderColor = UITheme.Current.TabStyle.BorderColor;

            assemblyRequiredAmountsBySlotId = new int[15];
            Array.Fill(assemblyRequiredAmountsBySlotId, 1);

            for (int i = 0; i < Inventory.Size; i++)
            {
                var slot = Inventory.ProvideItemSlot(i);
                slot.SizeLimit += 8;

                switch (i)
                {
                    // Command Pod/Satellite
                    case 0:
                        slot.Top = new(0, 0.075f);
                        slot.Left = new(0, 0.65f + 0.06f);
                        slot.Whitelist = [ModContent.ItemType<RocketPlating>()];
                        slot.BlueprintTexture = GetBlueprint<RocketPlating>();
                        assemblyRequiredAmountsBySlotId[i] = 50;
                        break;

                    case 1:
                        slot.Top = new(0, 0.075f);
                        slot.Left = new(0, 0.65f + 0.12f);
                        slot.Whitelist = [ModContent.ItemType<LexanGlass>()];
                        slot.BlueprintTexture = GetBlueprint<LexanGlass>();
                        assemblyRequiredAmountsBySlotId[i] = 25;
                        break;

                    case 2:
                        slot.Top = new(0, 0.075f);
                        slot.Left = new(0, 0.65f + 0.18f);
                        slot.Whitelist = [ModContent.ItemType<Computer>()];
                        slot.BlueprintTexture = GetBlueprint<Computer>();
                        assemblyRequiredAmountsBySlotId[i] = 5;
                        break;

                    // Service Module
                    case 3:
                        slot.Top = new(0, 0.2f);
                        slot.Left = new(0, 0.08f + 0.06f);
                        slot.Whitelist = [ModContent.ItemType<RocketPlating>()];
                        slot.BlueprintTexture = GetBlueprint<RocketPlating>();
                        assemblyRequiredAmountsBySlotId[i] = 50;
                        break;

                    case 4:
                        slot.Top = new(0, 0.2f);
                        slot.Left = new(0, 0.08f + 0.12f);
                        break;

                    case 5:
                        slot.Top = new(0, 0.2f);
                        slot.Left = new(0, 0.08f + 0.18f);
                        break;

                    // Reactor Module
                    case 6:
                        slot.Top = new(0, 0.34f);
                        slot.Left = new(0, 0.65f + 0.06f);
                        slot.Whitelist = [ModContent.ItemType<RocketPlating>()];
                        slot.BlueprintTexture = GetBlueprint<RocketPlating>();
                        assemblyRequiredAmountsBySlotId[i] = 50;
                        break;

                    case 7:
                        slot.Top = new(0, 0.34f);
                        slot.Left = new(0, 0.65f + 0.12f);
                        slot.Whitelist = [ModContent.ItemType<ReactorComponent>()];
                        slot.BlueprintTexture = GetBlueprint<ReactorComponent>();
                        break;

                    case 8:
                        slot.Top = new(0, 0.34f);
                        slot.Left = new(0, 0.65f + 0.18f);
                        slot.Whitelist = [ModContent.ItemType<ReactorHousing>()];
                        slot.BlueprintTexture = GetBlueprint<ReactorHousing>();
                        assemblyRequiredAmountsBySlotId[i] = 2;
                        break;

                    // Engine Module
                    case 9:
                        slot.Top = new(0, 0.5f);
                        slot.Left = new(0, 0.08f + 0.06f);
                        slot.Whitelist = [ModContent.ItemType<RocketPlating>()];
                        slot.BlueprintTexture = GetBlueprint<RocketPlating>();
                        assemblyRequiredAmountsBySlotId[i] = 50;
                        break;

                    case 10:
                        slot.Top = new(0, 0.5f);
                        slot.Left = new(0, 0.08f + 0.12f);
                        break;

                    case 11:
                        slot.Top = new(0, 0.5f);
                        slot.Left = new(0, 0.08f + 0.18f);
                        break;

                    // Boosters
                    case 12:
                        slot.Top = new(0, 0.6f);
                        slot.Left = new(0, 0.65f + 0.06f);
                        slot.Whitelist = [ModContent.ItemType<RocketPlating>()];
                        slot.BlueprintTexture = GetBlueprint<RocketPlating>();
                        assemblyRequiredAmountsBySlotId[i] = 50;
                        break;

                    case 13:
                        slot.Top = new(0, 0.6f);
                        slot.Left = new(0, 0.65f + 0.12f);
                        break;

                    case 14:
                        slot.Top = new(0, 0.6f);
                        slot.Left = new(0, 0.65f + 0.18f);
                        slot.Whitelist = [ModContent.ItemType<RocketLandingLeg>()];
                        slot.BlueprintTexture = GetBlueprint<RocketLandingLeg>();
                        assemblyRequiredAmountsBySlotId[i] = 3;
                        break;
                }

                if(assemblyRequiredAmountsBySlotId[i] > 1)
                {
                    UIText amountRequiredText = new("x" + assemblyRequiredAmountsBySlotId[i].ToString(), textScale: 0.8f)
                    {
                        Top = new(0, 0.98f),
                        HAlign = 0.5f
                    };
                    slot.Append(amountRequiredText);
                }

                Append(slot);
            }
        }

        public override void OnDeactivate()
        {
        }

        public void OnTabOpen()
        {
        }

        public void OnTabClose()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Rocket.Active)
            {
                Rocket.Modules["CommandPod"].IsBlueprint = Inventory.Items[0].type == 0 || Inventory.Items[1].type == 0 || Inventory.Items[2].type == 0;
                Rocket.Modules["ServiceModule"].IsBlueprint = Inventory.Items[3].type == 0 || Inventory.Items[4].type == 0 || Inventory.Items[5].type == 0;
                Rocket.Modules["ReactorModule"].IsBlueprint = Inventory.Items[6].type == 0 || Inventory.Items[7].type == 0 || Inventory.Items[8].type == 0;
                Rocket.Modules["EngineModule"].IsBlueprint = Inventory.Items[9].type == 0 || Inventory.Items[10].type == 0 || Inventory.Items[11].type == 0;
                Rocket.Modules["BoosterLeft"].IsBlueprint = Inventory.Items[12].type == 0 || Inventory.Items[13].type == 0 || Inventory.Items[14].type == 0;
                Rocket.Modules["BoosterRight"].IsBlueprint = Inventory.Items[12].type == 0 || Inventory.Items[13].type == 0 || Inventory.Items[14].type == 0;
            }
            else
            {
                foreach(var module in Rocket.Modules)
                {
                    module.Value.IsBlueprint = false;
                }
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            CalculatedStyle dimensions = GetDimensions();

            if(!Rocket.Active)
                Rocket.Draw(Rocket.DrawMode.Blueprint, spriteBatch, dimensions.Center() - Rocket.Bounds.Size() / 2f, useRenderTarget: false);
            else
                Rocket.Draw(Rocket.DrawMode.Dummy, spriteBatch, dimensions.Center() - Rocket.Bounds.Size() / 2f, useRenderTarget: false);
        }
    }
}
