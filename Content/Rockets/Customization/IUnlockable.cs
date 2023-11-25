namespace Macrocosm.Content.Rockets.Customization
{
    public interface IUnlockable
    {
        public bool Unlocked { get; set; }
        public bool UnlockedByDefault { get; }
        public string GetKey();

        // public void OnUnlocked() { }
    }
}
