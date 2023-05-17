using SubworldLibrary;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds;
using Macrocosm.Common.Subworlds;

namespace Macrocosm.Common.Global.GlobalNPCs
{
    public class GravityNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => true;

		public static FieldInfo NPCGravity { get; private set; }
		public override void Load()
		{
			NPCGravity = typeof(NPC).GetField("gravity", BindingFlags.NonPublic | BindingFlags.Static);
			Terraria.On_NPC.UpdateNPC_UpdateGravity += NPC_UpdateNPC_UpdateGravity;
		}

		private static void NPC_UpdateNPC_UpdateGravity(Terraria.On_NPC.orig_UpdateNPC_UpdateGravity orig, NPC self, out float maxFallSpeed)
		{
			orig(self, out maxFallSpeed);

			if (MacrocosmSubworld.AnyActive)
				NPCGravity.SetValue(null, Earth.NPCGravity * MacrocosmSubworld.Current.GravityMultiplier);
		}
	}
}