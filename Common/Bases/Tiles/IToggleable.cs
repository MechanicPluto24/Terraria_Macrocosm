namespace Macrocosm.Common.Bases.Tiles
{
    public interface IToggleable
    {
        public void Toggle(int i, int j, bool skipWire = false);
    }
}
