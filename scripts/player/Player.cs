using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;
using Input = Bonebreaker.Inputs.Input;

public enum Orientation
{
    Left, Right
}

public sealed class Player : Body
{
    [Signal] public delegate void AnimationFinished ();
    
    [Export] public PlayerStats Stats;
    [Export] public bool Debug;

    public AnimationPlayer Animator;
    public Orientation Orientation;
    
    public sfloat2 Velocity { get; set; }
    public new bool IsGrounded { get; set; }

    public State _CurrentState;
    public State _IdleState, _RunningState, _AscendingState, _FallingState;
    public JabAction _JabAction;
    
    public int playerIndex;
    public bool playerControlled;

    public void _network_spawn (Dictionary data)
    {
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
        Stats.ComputeStats();

        GatherReferences();
        SetupStates();
        ResolveCorrectState(new InputState(), 0);
    }

    public void GatherReferences ()
    {
        Animator = GetNode<AnimationPlayer>("Animator");
    }

    /// <summary>
    /// Will check if the current state need to Exit using its ShouldExit function
    /// If it does, we do the same for the new state until it stabilize onto one state
    /// </summary>
    public void ResolveCorrectState (InputState input, int _tick)
    {
        if (_CurrentState == null)
        {
            GD.PrintErr("Trying to resolve the Correct State of Node " + Name + " but this Node is not currently in any state !");    
        }

        State old = _CurrentState;
        State next = _CurrentState.ShouldExit(input, _tick);

        if (next == null) return;
        
        while (next != null)
        {
            _CurrentState = next;
            next = _CurrentState.ShouldExit(input, _tick);
        }

        old.Exit(_CurrentState);
        _CurrentState.Enter(old, _tick);
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
        _JabAction = new JabAction();
        
        
        _IdleState.SetOwner(this);
        _RunningState.SetOwner(this);
        _AscendingState.SetOwner(this);
        _FallingState.SetOwner(this);
        _JabAction.SetOwner(this);
        _JabAction._Init();

        _CurrentState = _IdleState;
        _CurrentState.Enter(null, 0);
    }

    private void _network_process (float delta, Dictionary input, int tick)
    {
        InputState inp = InputState.Deserialize(input);
        
        ResolveCorrectState(inp, tick);
        
        _CurrentState.Tick(tick, (sfloat)delta, inp);
        
        MoveAndSlide(Velocity * (sfloat)delta, Collided);
        IsGrounded = IsGrounded();
    }
    
    public void _interpolate_state (Dictionary old, Dictionary _new, float weight)
    {
        string pos1 = (string)old["position"];
        string pos2 = (string)_new["position"];
        string col_pos1 = (string)old["collider_position"];
        string col_pos2 = (string)_new["collider_position"];
        
        Position = sfloat2.Lerp(sfloat2.FromString(pos1), sfloat2.FromString(pos2), (sfloat)weight);
        Collider.Position = sfloat2.Lerp(sfloat2.FromString(col_pos1), sfloat2.FromString(col_pos2), (sfloat)weight);
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
            { "collider_position", Collider.Position.SerializeToString() },
            { "is_grounded", IsGrounded },
            { "velocity", Velocity.SerializeToString() },
            { "state", _CurrentState.ToString() },
            { "state_info", _CurrentState._Serialize() },
            { "jab_state", _JabAction._Serialize() }
        };
    }

    public void _load_state (Dictionary state)
    {
        Position = sfloat2.FromString((string)state["position"]);
        Collider.Position = sfloat2.FromString((string)state["collider_position"]);
        Velocity = sfloat2.FromString((string)state["velocity"]);
        IsGrounded = (bool)state["is_grounded"];

        switch ((string)state["state"])
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
            case "Jab Action":
                _CurrentState = _JabAction;
                break;
        }
        
        _CurrentState._Deserialize(state["state_info"] as Dictionary);
        _JabAction._Deserialize(state["jab_state"] as Dictionary);
    }
}
