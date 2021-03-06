using System;
using System.Collections.Generic;
using System.Globalization;
using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;
using Input = Bonebreaker.Inputs.Input;

public enum Orientation
{
    Left = -1, Right = 1
}

public abstract class Character : Body
{
    [Export] public PlayerStats Stats;
    [Export] public bool Debug;

    public AnimationPlayer Animator;
    public Orientation Orientation;

    private ProgressBar Healthbar;
    
    public int Health { get; set; }
    public sfloat2 Velocity { get; set; }
    public new bool IsGrounded { get; set; }

    public State _CurrentState;
    public State _IdleState, _RunningState, _AscendingState, _FallingState, _HitState;
    public JabAction _JabAction;
    public ActionState _DashAbility;

    [Export] private Vector2 DamagedFromLeftPivot;
    [Export] private Vector2 DamagedFromRightPivot;
    
    public int playerIndex;
    public bool playerControlled;

    public string username = "";
    
    private bool focused;
    

    public override void _Notification (int what)
    {
        if (what == 1004)
            focused = true;
        if (what == 1005)
            focused = false;
    }

    public void _network_spawn (Dictionary data)
    {
        playerIndex = (int)data["player_index"];
        SetNetworkMaster((int)data["peer_id"]);
        Name = "Player " + (int)data["peer_id"];
        username = (string)data["player_name"];

        if ((int)data["peer_id"] == GetTree().GetNetworkUniqueId())
        {
            playerControlled = true;
            focused = true;
            GetNode<Node2D>("Selecter").Visible = true;
        }
        else
        {
            GetNode<Label>("Username").Visible = true;
            GetNode<Label>("Username").Text = (string)data["player_name"];
        }
        
        Healthbar.GetNode<Label>("Username").Text = username == "" ? GetCharacterName : username;
    }

    public Dictionary _get_local_input ()
    {
        if (!focused)
            return new InputState().Serialize();
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
        Health = Stats.Health;

        GatherReferences();
        SetupStates();
        ResolveCorrectState(new InputState(), 0);
        SetupHealthbar();
    }

    public void GatherReferences ()
    {
        Animator = GetNode<AnimationPlayer>("Animator");
        Hurtbox.Connect("Ticked", this, nameof(Ticked));
    }

    private void SetupHealthbar ()
    {
        var _healthbarInstance = GD.Load<PackedScene>("res://prefabs/healthbar.tscn").Instance();
        var _hudRoot = GetTree().GetNodesInGroup("hud_root")[0] as Node;
        _hudRoot.FindNode("Healthbars").AddChild(_healthbarInstance);

        Healthbar = _healthbarInstance as ProgressBar;
        Healthbar.Value = 100;
        Health = 100;
    }

    public void DealDamage (Vector2 offset)
    {
        var hit = Physics.CastAABB(Position + new sfloat2(offset.x, offset.y), Hitbox.Size, false, 
            new List<Predicate<AABB>> { (aabb => aabb.Type == (int)Boxes.Hurtbox), (aabb => aabb != Hurtbox) });

        foreach ((AABB, Hit) box in hit)
        {
            box.Item1.EmitSignal(nameof(AABB.Ticked), this, Stats.Damage);
        }
    }

    public void Ticked (AABB dealer, int amount)
    {
        int dir = (int)sfloat.Sign(dealer.Position.X - Position.X);
        Orientation = (Orientation)dir;
        
        _CurrentState.Exit(_HitState);
        _CurrentState = _HitState;
        _CurrentState.Enter(null, 0);

        Health -= amount;
        Healthbar.Value = Mathf.Clamp(Health, 0, 100);

        if (Health < 0)
        {
            GD.Print("Dead !");
        }

        Vector2 popupLocation = Orientation == Orientation.Left ? DamagedFromLeftPivot : DamagedFromRightPivot;
        popupLocation += GlobalPosition;
        
        var _popupInstance = GD.Load<PackedScene>("res://prefabs/damage_popup.tscn").Instance() as Node2D;
        var _root = GetTree().GetNodesInGroup("level_root")[0] as Node;
        _root.AddChild(_popupInstance);
        _popupInstance.Position = popupLocation;

        _popupInstance.GetChild<Label>(0).Text = amount.ToString();
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
    protected virtual void SetupStates ()
    {
        _IdleState = new IdleState(this);
        _RunningState = new RunningState(this);
        _AscendingState = new AscendingState(this);
        _FallingState = new FallingState(this);
        _JabAction = new JabAction(this);
        _HitState = new HitState(this);
        _DashAbility = new ActionState(this);

        _CurrentState = _IdleState;
        _CurrentState.Enter(null, 0);
    }

    private void _network_process (float delta, Dictionary input, int tick)
    {
        InputState inp = InputState.Deserialize(input);
        
        ResolveCorrectState(inp, tick);
        
        _CurrentState.Tick(tick, (sfloat)delta, inp);
        
        sfloat2 traveled = MoveAndSlide(Velocity * (sfloat)delta, Collided);
        
        if(traveled != sfloat2.Zero)
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

    protected virtual string GetCharacterName => "Unknown";

    public Dictionary _save_state ()
    {
        return new Dictionary
        {
            { "health", Health },
            { "position", Position.SerializeToString() },
            { "collider_position", Collider.Position.SerializeToString() },
            { "hurtbox_position", Hurtbox.Position.SerializeToString() },
            { "hurtbox_size", Hurtbox.HalfExtents.SerializeToString() },
            { "hitbox_position", Hitbox.Position.SerializeToString() },
            { "hitbox_size", Hitbox.HalfExtents.SerializeToString() },
            { "is_grounded", IsGrounded },
            { "velocity", Velocity.SerializeToString() },
            { "state", _CurrentState.ToString() },
            { "jab_state", _JabAction._Serialize() },
            { "hit_state", _HitState._Serialize() },
            { "dash_ability", _DashAbility._Serialize() }
        };
    }

    public void _load_state (Dictionary state)
    {
        Health = (int)state["health"];
        
        Position = sfloat2.FromString((string)state["position"]);
        Collider.Position = sfloat2.FromString((string)state["collider_position"]);
        
        Hurtbox.Position = sfloat2.FromString((string)state["hurtbox_position"]);
        Hurtbox.HalfExtents = sfloat2.FromString((string)state["hurtbox_size"]);
        
        Hitbox.Position = sfloat2.FromString((string)state["hitbox_position"]);
        Hitbox.Size = sfloat2.FromString((string)state["hitbox_size"]);
        
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
            case "Dash":
                _CurrentState = _DashAbility;
                break;
        }

        _JabAction._Deserialize(state["jab_state"] as Dictionary);
        _HitState._Deserialize(state["hit_state"] as Dictionary);
        _DashAbility._Deserialize(state["dash_ability"] as Dictionary);
    }
}
