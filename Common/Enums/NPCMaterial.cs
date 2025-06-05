using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Enums
{
    /// <summary> Describes what an NPC is made of. Used for some weapon interactions. </summary>
    public enum NPCMaterial
    {
        /// <summary> Default value </summary>
        None,
        /// <summary> Flesh and Plant-like enemies. Includes Zombies, Demon Eyes, and Snatchers. </summary>
        Organic,
        /// <summary> Slime-like enemies. Includes most slime variants, Beetles, and Jellyfish. </summary>
        Slime,
        /// <summary> Ghostly and Fae enemies. Includes Ghosts, Chaos Elementals, and Pixies. </summary>
        Supernatural,
        /// <summary> Golems, Skeletons, and Rock-like enemies. Includes Granite Elementals, Rock Golem, and Skeletron. </summary>
        Earth,
        /// <summary> Enemies with metallic parts and armor. Includes Paladin, Possessed Armor, and Mimics. </summary>
        Metal,
        /// <summary> Robots and Drones. Includes Mechanical Bosses, Martian drones, and Martian Saucer. </summary>
        Machine,
        /// <summary> Demonic and hellish enemies. Includes Demons, Vampire, and most Xaoc enemies. </summary>
        Demon,
        /// <summary> Any enemy that doesn't fit in the previous criteria, or designed specifically to have no unique weapon interactions. </summary>
        Almighty
    }
}
