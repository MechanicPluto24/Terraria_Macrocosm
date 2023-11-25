
namespace Macrocosm.Common.UI
{
    public interface ITabUIElement
    {
        //public ITabUIElement Next { get; set; }
        //public ITabUIElement Prev { get; set; }

        public void OnTabOpen() { }
        public void OnTabClose() { }
    }
}
