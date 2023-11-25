namespace Macrocosm.Content.Rockets.Modules
{
    public class CommandPod : RocketModule
    {
        public CommandPod(Rocket rocket) : base(rocket)
        {
        }

        public override int DrawPriority => 4;

        public override int Width => 68;
        public override int Height => 78;
    }
}
