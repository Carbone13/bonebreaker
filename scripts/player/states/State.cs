using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;

public class State
{
    protected Player Owner;

    public void SetOwner (Player owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// For external classes to call the Tick functions
    /// </summary>
    public void Tick (int frame, sfloat delta, InputState input)
    {
        _Tick(frame, delta, input);
    }
    
    /// <summary>
    /// For our inheritor to implement their own Tick logic
    /// </summary>
    protected virtual void _Tick (int frame, sfloat delta, InputState input) {}

    /// <summary>
    /// For external classes to call the Enter function
    /// </summary>
    public void Enter (State previous)
    {
        _Enter(previous);
        
        if(Owner.Debug)
            GD.Print("Entering State " + this);
    }
    
    /// <summary>
    /// For our inheritor to implement their own Enter logic
    /// </summary>
    protected virtual void _Enter (State previous) {}
    
    /// <summary>
    /// For external classes to call the Exit function
    /// </summary>
    public void Exit (State next)
    {
        _Exit(next);
        
        //if(Owner.Debug)
            //GD.Print("Exiting State " + this);
    }

    /// <summary>
    /// For our inheritor to implement their own Exit logic
    /// </summary>
    protected virtual void _Exit (State next) {}

    /// <summary>
    /// Check if we need to Exit this state
    /// </summary>
    /// <returns>The state we should exit toward</returns>
    public State ShouldExit (InputState input)
    {
        return _ShouldExit(input);
    }

    /// <summary>
    /// For our inheritor to implement their own ShouldExit logic
    /// </summary>
    protected virtual State _ShouldExit (InputState input)
    {
        return null;
    }
}
