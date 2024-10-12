using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Global.NPCs
{
    /// <summary> Global NPC for NPC instances </summary>
    public class MacrocosmNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => false;

        /// <summary> If this is only set on a local client, the logic accessing this needs to be synced </summary>
        public bool TargetedByHomingProjectile { get; set; }

        public override void SetDefaults(NPC npc)
        {
            if (npc.ModNPC is ModNPC modNPC)
            {
                if (NPCSets.MoonNPC[npc.type])
                    modNPC.SpawnModBiomes = npc.ModNPC.SpawnModBiomes.Prepend(ModContent.GetInstance<MoonBiome>().Type).ToArray();
                //else if (NPCSets.MarsEnemies[npc.type])
                //    modNPC.SpawnModBiomes = npc.ModNPC.SpawnModBiomes.Prepend(ModContent.GetInstance<MarsBiome>().Type).ToArray();
                else
                    modNPC.SpawnModBiomes = npc.ModNPC.SpawnModBiomes.Prepend(ModContent.GetInstance<EarthBiome>().Type).ToArray();
            }
        }

        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            if (npc.ModNPC is null)
            {
                EarthBiome earthBiome = ModContent.GetInstance<EarthBiome>();
                bestiaryEntry.Info.Add(new ModBiomeBestiaryInfoElement(Mod, earthBiome.DisplayName.Key, earthBiome.BestiaryIcon, null, null));
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Make all DropsMoonstone NPCs drop Moonstone while on the Moon
            if (NPCSets.DropsMoonstone[npc.type])
                npcLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Moonstone>(), 10, 1, 5, new SubworldDropCondition<Moon>(canShowInBestiary: true)));
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.ModNPC is null)
                return;

            npc.ModNPC.NetWriteFields(binaryWriter, bitWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (npc.ModNPC is null)
                return;

            npc.ModNPC.NetReadFields(binaryReader, bitReader);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (TargetedByHomingProjectile)
                DrawCrosshair(npc, spriteBatch, screenPos, drawColor);
        }

        private void DrawCrosshair(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D crosshair = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/Crosshair").Value;
            Color color = new(255, 255, 255, 64);
            Vector2 position = npc.Center - screenPos;
            float rotation = (float)(Main.timeForVisualEffects / 20);
            spriteBatch.Draw(crosshair, position, null, color, rotation, crosshair.Size() / 2, 1.5f, SpriteEffects.None, 0f);
        }
    }
}