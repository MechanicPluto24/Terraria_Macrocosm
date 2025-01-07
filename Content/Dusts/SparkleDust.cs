using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class SparkleDust : ModDust
    {
        public override string Texture => Macrocosm.TexturesPath + "Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = true;
            dust.noGravity = true;
            //if (dust.scale <= 1f && dust.scale >= 0.8f)
            //  dust.scale = 0.25f;
            base.OnSpawn(dust);

        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.005f;
            dust.velocity *= 0.95f;
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }

        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == ModContent.DustType<SparkleDust>() && d.active)
                {
                    Texture2D tex = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Star4").Value;
                    if (d.customData != null)
                        sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f, SpriteEffects.None, 0);
                    sb.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.customData != null ? ((int)d.customData == 2 ? d.scale * 10 : 1) : 1), 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
