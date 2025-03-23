using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing.Sky
{
    public class MacrocosmAmbientSky : CustomSky, ILoadable
    {
        public static MacrocosmAmbientSky Instance => (MacrocosmAmbientSky)SkyManager.Instance[Key];
        private static string Key => $"{nameof(Macrocosm)}:{nameof(MacrocosmAmbientSky)}";

        private bool isActive;
        private readonly SlotVector<MacrocosmSkyEntity> entities = new(500);
        private int frameCounter;
        private delegate MacrocosmSkyEntity EntityFactoryMethod(Player player, int seed);


        public void Load(Mod mod)
        {
            SkyManager.Instance[Key] = new MacrocosmAmbientSky();
        }

        public void Unload()
        {
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        private bool AnActiveSkyConflictsWithAmbience()
        {
            if (!SkyManager.Instance["MonolithMoonLord"].IsActive())
                return SkyManager.Instance["MoonLord"].IsActive();

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (SlotVector<MacrocosmSkyEntity>.ItemPair item in entities)
            {
                MacrocosmSkyEntity value = item.Value;
                value.Update(null, frameCounter);
                if (!value.IsActive)
                {
                    entities.Remove(item.Id);
                    if (Main.netMode != NetmodeID.Server && entities.Count == 0 && SkyManager.Instance[Key].IsActive())
                        SkyManager.Instance.Deactivate(Key);
                }
            }

            frameCounter++;

            if (Main.netMode != NetmodeID.Server && AnActiveSkyConflictsWithAmbience() && SkyManager.Instance[Key].IsActive())
                SkyManager.Instance.Deactivate(Key);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer && SkyManager.Instance[Key].IsActive())
            {
                entities.Clear();
                SkyManager.Instance.Deactivate(Key);
            }

            foreach (SlotVector<MacrocosmSkyEntity>.ItemPair item in entities)
                item.Value.Draw(spriteBatch, 3f, minDepth, maxDepth);
        }

        private void DrawUpdate()
        {
        }

        public override bool IsActive() => isActive;

        public override void Reset()
        {
        }

        // TODO: netcode
        public void Spawn<T>(Player player, int seed) where T : MacrocosmSkyEntity
        {
            FastRandom random = new(seed);

            var entityInstance = (T)Activator.CreateInstance(typeof(T), player, random);
            var entitiesToAdd = entityInstance.CreateGroup(player, random);

            foreach (var entity in entitiesToAdd)
                entities.Add(entity);

            if (Main.netMode != NetmodeID.Server && !AnActiveSkyConflictsWithAmbience() && !SkyManager.Instance[Key].IsActive())
                SkyManager.Instance.Activate(Key, default);
        }
    }
}
