using Terraria.ModLoader;

namespace Macrocosm.Content.Items.CursorIcons
{
    /// <summary> We have to make do with this until tML adds proper custom icons </summary>
    public abstract class CursorIcon : ModItem
    {
        public static int GetType<T>() where T : ModItem => ModContent.ItemType<T>();
    }

    public class Rocket : CursorIcon
    {
    }

    public class QuestionMark : CursorIcon
    {
    }
}
