﻿using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;

public class State : Reference
{
    protected Player Owner;

    public virtual void _Init ()
    {
        
    }
    
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
        _Animate();
    }
    
    /// <summary>
    /// For our inheritor to implement their own Tick logic
    /// </summary>
    protected virtual void _Tick (int frame, sfloat delta, InputState input) {}


    protected virtual void _Animate () { }
    
    /// <summary>
    /// For external classes to call the Enter function
    /// </summary>
    public void Enter (State previous, int tick)
    {
        _Enter(previous, tick);
        
        if(Owner.Debug)
            GD.Print("Entering State " + this);
    }
    
    /// <summary>
    /// For our inheritor to implement their own Enter logic
    /// </summary>
    protected virtual void _Enter (State previous, int tick) {}
    
    /// <summary>
    /// For external classes to call the Exit function
    /// </summary>
    public void Exit (State next)
    {
        _Exit(next);
        
        if(Owner.Debug)
            GD.Print("Exiting State " + this);
    }

    /// <summary>
    /// For our inheritor to implement their own Exit logic
    /// </summary>
    protected virtual void _Exit (State next) {}

    /// <summary>
    /// Check if we need to Exit this state
    /// </summary>
    /// <returns>The state we should exit toward</returns>
    public State ShouldExit (InputState input, int tick)
    {
        if (input.LightJustPressed && Owner._CurrentState != Owner._JabAction)
            return Owner._JabAction;
        
        return _ShouldExit(input, tick);
    }

    /// <summary>
    /// For our inheritor to implement their own ShouldExit logic
    /// </summary>
    protected virtual State _ShouldExit (InputState input, int tick)
    {
        return null;
    }

    public virtual Dictionary _Serialize ()
    {
        return null;
    }

    public virtual void _Deserialize (Dictionary state)
    {
        
    }
}
