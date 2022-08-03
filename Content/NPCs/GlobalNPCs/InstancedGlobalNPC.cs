using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Macrocosm.NPCs.GlobalNPCs {
    public class InstancedGlobalNPC : GlobalNPC {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => false;

        public bool[] targetedBy = new bool[Main.maxPlayers];

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {

            if (targetedBy[Main.LocalPlayer.whoAmI]) {
                DrawCrosshair(npc, spriteBatch, screenPos, drawColor);
            }

           // ChatManager.DrawColorCodedString(spriteBatch, FontAssets.DeathText.Value, npc.whoAmI.ToString(), npc.position + new Vector2(10, 20) - screenPos, Color.White, 0f, Vector2.Zero, Vector2.One * 0.5f);
        }

        private void DrawCrosshair(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D crosshair = ModContent.Request<Texture2D>("Macrocosm/Assets/UI/Crosshair").Value;
            Color color = Color.White;
            Vector2 position = npc.Center - screenPos - crosshair.Size() / 2;
            spriteBatch.Draw(crosshair, position, color); 
        }
    }
}