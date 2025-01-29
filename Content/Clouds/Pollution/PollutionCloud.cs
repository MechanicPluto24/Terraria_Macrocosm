using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Clouds.Pollution
{
    public abstract class PollutionCloud : ModCloud
    {
        public override bool RareCloud => false;

        public override float SpawnChance() => !Main.gameMenu && Main.LocalPlayer.InModBiome<PollutionBiome>() ? 25f : 0f;

        public override void OnSpawn(Cloud cloud) { }

        int despawnTimer = 0;
        float opacity = 0.8f;
        public override bool Draw(SpriteBatch spriteBatch, Cloud cloud, int cloudIndex, ref DrawData drawData)
        {
            if (!Main.LocalPlayer.InModBiome<PollutionBiome>())
            {
                opacity -= 0.005f;
                if(opacity < 0f)
                    cloud.active = false;
            }
            else
            {
                opacity = 0.7f;
            }

            drawData.color *= opacity;
            if (drawData.color.A > 200)
                drawData.color.A = 200;
            return true;
        }
    }
    public class PollutionCloud1 : PollutionCloud
    {
    }
    public class PollutionCloud2 : PollutionCloud
    {
    }
    public class PollutionCloud3 : PollutionCloud
    {
    }
    public class PollutionCloud4 : PollutionCloud
    {
    }
    public class PollutionCloud5 : PollutionCloud
    {
    }
    public class PollutionCloud6 : PollutionCloud
    {
    }
    public class PollutionCloud7 : PollutionCloud
    {
    }
    public class PollutionCloud8 : PollutionCloud
    {
    }
}
