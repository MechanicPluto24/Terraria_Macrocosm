using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters
{
    public abstract class AutocrafterTEBase : ConsumerTE
    {
        public abstract int OutputSlots { get; }
        public virtual int InputSlots => OutputSlots * 15;
        public sealed override int InventorySize => InputSlots + OutputSlots;

        protected virtual bool RecipeAllowed(Recipe recipe)
        {
            // "By Hand" recipes allowed by default
            if (recipe.requiredTile.All(tile => tile == -1))
                return true;

            // AvailableCraftingStations contains all recipe.requiredTile
            return recipe.requiredTile.Where(tile => tile != -1).All(tile => AvailableCraftingStations.Contains(tile));
        }

        protected virtual int[] AvailableCraftingStations => [];

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
        }

        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            //basePosition.X -= 12;
            base.DrawMachinePowerInfo(spriteBatch, basePosition, lightColor);
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }
    }
}
