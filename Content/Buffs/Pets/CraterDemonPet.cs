using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Pets;

// TODO: PetBuff
public class CraterDemonPet : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.vanityPet[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        bool unused = false;
        player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<Projectiles.Friendly.Pets.CraterDemonPet>());
    }
}
