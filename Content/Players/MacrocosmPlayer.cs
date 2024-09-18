using Macrocosm.Common.Netcode;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Content.Buffs.Potions;
using Macrocosm.Content.Debuffs.Environment;
using Macrocosm.Content.Items.Consumables.Potions;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Players
{
    /// <summary>
    /// Miscellaneous class for storing custom player data. 
    /// Complex, very specific systems should be implemented in a separate ModPlayer.
    /// </summary>
    public class MacrocosmPlayer : ModPlayer
    {
        #region Equip data
        /// <summary> 
        /// The player's space protection level.
        /// Not synced.
        /// </summary>
        public float SpaceProtection { get; set; } = 0f;

        /// <summary> 
        /// Chance to not consume ammo from equipment and weapons, stacks additively with the vanilla chance.
        /// Not synced.
        /// </summary>
        public float ChanceToNotConsumeAmmo
        {
            get => chanceToNotConsumeAmmo;
            set => chanceToNotConsumeAmmo = MathHelper.Clamp(value, 0f, 1f);
        }

        private float chanceToNotConsumeAmmo = 0f;
        #endregion

        #region Weapon data
        /// <summary> 
        /// Chandrium whip hit stacks. 
        /// Not synced.
        /// </summary>
        public int ChandriumWhipStacks = 0;
        #endregion

        #region Consumables data
        public bool MedkitActive => Player.HasBuff<MedkitLow>() || Player.HasBuff<MedkitMedium>() || Player.HasBuff<MedkitHigh>();

        // Used for identifying medkit tier
        public int MedkitItemType = ItemType<Medkit>();
        private Medkit Medkit => (ContentSamples.ItemsByType[MedkitItemType].ModItem as Medkit);
        private int medkitTimer;
        private int medkitHitCooldown;
        #endregion

        #region Player flags
        /// <summary> 
        /// Whether this player is aware that they can use zombie fingers to unlock chests.
        /// Persistent. Not synced.
        /// </summary>
        public bool KnowsToUseZombieFinger = false;
        #endregion

        public override void ResetEffects()
        {
            SpaceProtection = 0f;
            ChanceToNotConsumeAmmo = 0f;

            Player.buffImmune[BuffType<Depressurized>()] = false;
        }

        #region Hooks
        public override void OnEnterWorld()
        {
            CircuitSystem.SearchCircuits();
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            bool consumeAmmo = true;

            if (Main.rand.NextFloat() < ChanceToNotConsumeAmmo)
                consumeAmmo = false;

            return consumeAmmo;
        }

        public override void PostUpdateBuffs()
        {
            Update_EnvironmentalDebuffs();
            Update_RocketImmunities();
            Update_MedkitHealing();
        }

        public override void PostUpdateEquips()
        {
            Update_EquipImmunities();
            Update_SetBonuses();
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            OnHurt_ChangeMedkit();
        }
        #endregion

        #region Equipment & Environment Effects
        private void Update_EnvironmentalDebuffs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                Player.AddBuff(BuffType<Depressurized>(), 2);
        }

        private void Update_RocketImmunities()
        {
            if (Player.GetModPlayer<RocketPlayer>().InRocket)
            {
                Player.buffImmune[BuffType<Depressurized>()] = true;
            }
        }

        private void Update_EquipImmunities()
        {
            if (SpaceProtection >= (float)3f)
                Player.buffImmune[BuffType<Depressurized>()] = true;
        }

        private void Update_SetBonuses()
        {
            if (SpaceProtection >= 3f)
                Player.setBonus = Language.GetTextValue("Mods.Macrocosm.Items.SetBonuses.SpaceProtection_" + (int)(SpaceProtection / 3f));
        }
        #endregion

        #region Consumables Effects
        private void Update_MedkitHealing()
        {
            if (MedkitActive)
            {
                int healPeriod = Medkit.HealPeriod;
                if (medkitTimer++ >= healPeriod)
                {
                    medkitTimer = 0;
                    int healAmount = Medkit.HealthPerPeriod;

                    if (Player.HasBuff<MedkitLow>())
                        Player.Heal((int)(healAmount * 0.33f));
                    else if (Player.HasBuff<MedkitMedium>())
                        Player.Heal((int)(healAmount * 0.66f));
                    else if (Player.HasBuff<MedkitHigh>())
                        Player.Heal(healAmount);
                }

                if (medkitHitCooldown > 0)
                    medkitHitCooldown--;
            }
            else
            {
                medkitTimer = 0;
                medkitHitCooldown = 0;
            }
        }

        private void OnHurt_ChangeMedkit()
        {
            if (medkitHitCooldown > 0)
                return;

            if (Player.HasBuff<MedkitMedium>())
            {
                int index = Player.FindBuffIndex(BuffType<MedkitMedium>());
                int time = Player.buffTime[index];
                Player.DelBuff(index);
                Player.AddBuff(BuffType<MedkitLow>(), time);

                medkitHitCooldown = Medkit.HitCooldown;
            }

            if (Player.HasBuff<MedkitHigh>())
            {
                int index = Player.FindBuffIndex(BuffType<MedkitHigh>());
                int time = Player.buffTime[index];
                Player.DelBuff(index);
                Player.AddBuff(BuffType<MedkitMedium>(), time);

                medkitHitCooldown = Medkit.HitCooldown;
            }
        }
        #endregion

        #region Biome & Visual Effects
        public override void PostUpdateMiscEffects()
        {
            Update_Graveyard();
        }

        private static void Update_Graveyard()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                Main.SceneMetrics.GraveyardTileCount = 0;
            else
                Main.SceneMetrics.GraveyardTileCount += TileCounts.Instance.GraveyardModTileCount;
        }

        #endregion

        #region Netcode
        public override void CopyClientState(ModPlayer clientClone)
        {
            // TODO: copy data that has to be netsynced
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            // TODO: SyncPlayer if netsynced data is different
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncMacrocosmPlayer);
            packet.Write((byte)Player.whoAmI);

            // TODO: add netsynced data here 
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
        {
            int playerWhoAmI = reader.ReadByte();
            MacrocosmPlayer macrocosmPlayer = Main.player[playerWhoAmI].GetModPlayer<MacrocosmPlayer>();

            // TODO: read netsynced data here
        }

        #endregion

        #region Saving & Loading
        public override void SaveData(TagCompound tag)
        {
            if (KnowsToUseZombieFinger)
                tag[nameof(KnowsToUseZombieFinger)] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            KnowsToUseZombieFinger = tag.ContainsKey(nameof(KnowsToUseZombieFinger));
        }
        #endregion
    }
}
