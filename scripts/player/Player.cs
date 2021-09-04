using System.Drawing.Printing;
using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;
using Input = Bonebreaker.Inputs.Input;

public sealed class Player : Body
{
    [Export] public PlayerStats Stats;
    [Export] public bool Debug;

    public int id;
    public bool local;
    
    public sfloat2 Velocity { get; set; }
    public bool IsGrounded { get; set; }

    private State _CurrentState;
    public State _IdleState, _RunningState, _AscendingState, _FallingState;

    private int _tick;

    public int playerIndex;
    public bool playerControlled;

    public void _network_spawn (Dictionary data)
    {
        GlobalPosition = (Vector2)data["start_transform"];
        playerIndex = (int)data["player_index"];
        SetNetworkMaster((int)data["peer_id"]);
        Name = "Player " + (int)data["peer_id"];

        if ((int)data["peer_id"] == GetTree().GetNetworkUniqueId())
            playerControlled = true;
    }
    
    public Dictionary _get_local_input ()
    {
        return Input.singleton.GetStateOfPrimaryDevice().Serialize();
    }
    
    public Dictionary _predict_network_input (Dictionary previous, int tickSinceLastConfirmed)
    {
        if (tickSinceLastConfirmed > 5)
            previous["joystick"] = "0|0";
        
        return previous;
    }
    
    public override void _Ready ()
    {
        base._Ready();
        SetupStates();
        ResolveCorrectState(Input.singleton.GetStateOfPrimaryDevice());
        Stats.ComputeStats();
    }

    /// <summary>
    /// Will check if the current state need to Exit using its ShouldExit function
    /// If it does, we do the same for the new state until it stabilize onto one state
    /// </summary>
    public void ResolveCorrectState (InputState input)
    {
        if (_CurrentState == null)
        {
            GD.PrintErr("Trying to resolve the Correct State of Node " + Name + " but this Node is not currently in any state !");    
        }

        State old = _CurrentState;
        State next = _CurrentState.ShouldExit(input);

        if (next == null) return;
        
        while (next != null)
        {
            _CurrentState = next;
            next = _CurrentState.ShouldExit(input);
        }

        old.Exit(_CurrentState);
        _CurrentState.Enter(old);
    }
    
    /// <summary>
    /// Link this Node states with the wanted ones, you can override it and provide whatever State object you want
    /// </summary>
    private void SetupStates ()
    {
        _IdleState = new IdleState();
        _RunningState = new RunningState();
        _AscendingState = new AscendingState();
        _FallingState = new FallingState();
        
        _IdleState.SetOwner(this);
        _RunningState.SetOwner(this);
        _AscendingState.SetOwner(this);
        _FallingState.SetOwner(this);

        _CurrentState = _IdleState;
        _CurrentState.Enter(null);
    }

    private void _network_process (float delta, Dictionary input)
    {
        InputState inp = InputState.Deserialize(input);
        
        ResolveCorrectState(inp);
        _CurrentState.Tick(0, (sfloat)delta, inp);
        
        MoveAndSlide(Velocity * (sfloat)delta, Collided);
        
        IsGrounded = IsGrounded();
    }
    
    public void _interpolate_state (Dictionary old, Dictionary _new, float weight)
    {
        string pos1 = (string)old["position"];
        string pos2 = (string)_new["position"];

        Position = sfloat2.Lerp(sfloat2.FromString(pos1), sfloat2.FromString(pos2), (sfloat)weight);
    }

    private void Collided (sfloat2 normal)
    {
        if (normal.X != sfloat.Zero)
        {
            Velocity = new sfloat2(sfloat.Zero, Velocity.Y);
        }

        if (normal.Y != sfloat.Zero)
        {
            Velocity = new sfloat2(Velocity.X, sfloat.Zero);
        }
    }

    public Dictionary _save_state ()
    {
        return new Dictionary
        {
            { "position", Position.SerializeToString() },
            { "velocity", Velocity.SerializeToString() },
            { "state", _CurrentState.ToString() }
        };
    }

    public void _load_state (Dictionary state)
    {
        string pos = (string)state["position"];
        Position = sfloat2.FromString(pos);
        Collider.Position = sfloat2.FromString(pos);
        
        string vel = (string)state["velocity"];
        Velocity = sfloat2.FromString(vel);

        switch (state["position"])
        {
            case "Idle":
                _CurrentState = _IdleState;
                break;
            case "Running":
                _CurrentState = _RunningState;
                break;
            case "Ascending":
                _CurrentState = _AscendingState;
                break;
            case "Falling":
                _CurrentState = _FallingState;
                break;
        }
    }
}
