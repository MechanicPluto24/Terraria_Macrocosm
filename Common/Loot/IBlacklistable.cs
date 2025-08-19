namespace Macrocosm.Common.Loot;

public interface IBlacklistable
{
    public int ItemID { get; }
    public bool Blacklisted { get; set; }
}
