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
    public abstract class AutocrafterTEBase : ConsumerTE, IInventoryOwner
    {
        public Inventory Inventory { get; set; }
        protected virtual int InventorySize => 50;
        public Vector2 InventoryPosition => Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;

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
            // Create new inventory if none found on world load
            Inventory ??= new(InventorySize, this);

            // Assign inventory owner if the inventory was found on load
            // IInvetoryOwner does not work well with TileEntities >:(
            if (Inventory.Owner is null)
                Inventory.Owner = this;
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
