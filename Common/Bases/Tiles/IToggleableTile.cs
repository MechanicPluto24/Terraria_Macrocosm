namespace Macrocosm.Common.Bases.Tiles
{
    public interface IToggleableTile
    {
        public void ToggleTile(int i, int j, bool skipWire = false);
    }
}
