using SubworldLibrary;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds;

namespace Macrocosm.Common.Global.GlobalNPCs
{
	public class LowGravityNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => true;

		private const float defaultGravity = 0.3f;

		public static FieldInfo NPCGravity { get; private set; }
		public override void Load()
		{
			NPCGravity = typeof(NPC).GetField("gravity", BindingFlags.NonPublic | BindingFlags.Static);
			On.Terraria.NPC.UpdateNPC_UpdateGravity += NPC_UpdateNPC_UpdateGravity;
		}

		private static void NPC_UpdateNPC_UpdateGravity(On.Terraria.NPC.orig_UpdateNPC_UpdateGravity orig, NPC self, out float maxFallSpeed)
		{
			orig(self, out maxFallSpeed);

			if (SubworldSystem.AnyActive<Macrocosm>())
				NPCGravity.SetValue(null, defaultGravity * MacrocosmSubworld.Current().GravityMultiplier);
		}
	}
}