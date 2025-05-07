using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    // WIP
    public class SpacesuitGlobalNPC : GlobalNPC
    {
        public class SpaceSuitTownNPCProfile : ITownNPCProfile
        {
            private readonly Asset<Texture2D> spaceTexture;
            private readonly int headIndex;

            public SpaceSuitTownNPCProfile(NPC npc, string spaceTexturePath, int? headIndex = null)
            {
                this.headIndex = headIndex ?? TownNPCProfiles.GetHeadIndexSafe(npc);
                if (!Main.dedServ)
                    spaceTexture = Main.Assets.Request<Texture2D>(spaceTexturePath, AssetRequestMode.ImmediateLoad);
            }

            public int RollVariation() => 0;
            public string GetNameForVariant(NPC npc) => npc.GivenName;
            public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => spaceTexture ?? TextureAssets.Npc[npc.type];
            public int GetHeadTextureIndex(NPC npc) => headIndex;
        }


        public override ITownNPCProfile ModifyTownNPCProfile(NPC npc)
        {
            return base.ModifyTownNPCProfile(npc);
        }

    }
}
