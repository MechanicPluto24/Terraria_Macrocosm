using Macrocosm.Common.Netcode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.NPCs.Global
{
    /// <summary> Global NPC for NPC instances </summary>
    public class MacrocosmNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => false;

        /// <summary> If this is only set on a local client, the logic accessing this needs to be synced </summary>
        public bool TargetedByHomingProjectile;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.ModNPC is null)
                return;

            //if (!npc.ModNPC.NetWriteFields(binaryWriter, bitWriter))
            //	binaryWriter.Dispose();

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