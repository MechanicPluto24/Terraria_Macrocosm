
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Gores
{
    public class MinigunShell : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.timeLeft = 240;
        }

        public override bool Update(Gore gore)
        {
            gore.timeLeft -= 5;
            return true;
        }
    }
}
