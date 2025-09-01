using SubworldLibrary;
using Terraria.ModLoader;

namespace Macrocosm.Common.Subworlds
{
    /// <summary> Offers MacrocosmSubworlds access to select ModSystem hooks </summary>
    class MacrocosmSubworldHooks : ModSystem
    {
        #region ModSystem update hooks
        public override void PreUpdateEntities()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateEntities();
        }

        public override void PreUpdatePlayers()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdatePlayers();
        }

        public override void PostUpdatePlayers()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdatePlayers();
        }

        public override void PreUpdateNPCs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateNPCs();
        }

        public override void PostUpdateNPCs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateNPCs();
        }

        public override void PreUpdateGores()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateGores();
        }

        public override void PostUpdateGores()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateGores();
        }

        public override void PreUpdateProjectiles()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateProjectiles();
        }

        public override void PostUpdateProjectiles()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateProjectiles();
        }

        public override void PreUpdateItems()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateItems();
        }

        public override void PostUpdateItems()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateItems();
        }

        public override void PreUpdateDusts()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateDusts();
        }

        public override void PostUpdateDusts()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateDusts();
        }

        public override void PreUpdateTime()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateTime();
        }

        public override void PostUpdateTime()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateTime();
        }

        public override void PreUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateWorld();
        }

        public override void PostUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateWorld();
        }

        public override void PreUpdateInvasions()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateInvasions();
        }

        public override void PostUpdateInvasions()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateInvasions();
        }

        public override void PostUpdateEverything()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateEverything();
        }

        #endregion
    }
}
