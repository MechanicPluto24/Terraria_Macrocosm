namespace Macrocosm.Content.NPCs.Global
{
    public interface IMacrocosmEnemy
    {
    }

    public interface IMoonEnemy : IMacrocosmEnemy
    {
        bool DropMoonstone => true;
    }
}
