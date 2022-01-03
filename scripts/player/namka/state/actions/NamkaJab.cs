using Bonebreaker.Inputs;

public class NamkaJab : JabAction
{
    public NamkaJab (Character owner) : base(owner)
    {
    }

    protected override void _Enter (State previous, int tick)
    {
        shouldExit = true;
    }
}
