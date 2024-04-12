using Macrocosm.Content.Rockets;

namespace Macrocosm.Common.UI
{
    public interface IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }

        public void OnRocketChanged() { }
    }
}
