using System;
using System.Collections.Generic;
using Bonebreaker.Inputs;
using Bonebreaker.Network;
using Godot;
using Godot.Collections;
using Input = Bonebreaker.Inputs.Input;

public enum Orientation
{
    Left = -1, Right = 1
}

// TODO big cleanup !
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

    public string username = "";
    
    private bool focused;
    

    public override void _Notification (int what)
    {
        if (what == 1004)
            focused = true;
        if (what == 1005)
            focused = false;
    }

    // Called from GDSCRIPT
    // ReSharper disable once UnusedMember.Global
    public void _NetworkSpawn (Dictionary data)
    {
        SetNetworkMaster((int)data["peer_id"]);
        Name = "Player " + (int)data["peer_id"];
        username = (string)data["player_name"];

        if ((int)data["peer_id"] == GetTree().GetNetworkUniqueId())
        {
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

    // Called from Animator
    // ReSharper disable once UnusedMember.Global
    public void DealDamage (Vector2 offset)
    {
        var hit = Physics.CastAABB(Position + new sfloat2(offset.x, offset.y), Hitbox.Size, Physics.QueryHurtboxes(),false, 
            new List<Predicate<AABB>> { (aabb => aabb != Hurtbox) });

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
    
    // CALLED FROM GDSCRIPT  
    // ReSharper disable once UnusedMember.Global
    public Dictionary _GetLocalInput ()
    {
        return !focused ? new InputState().Serialize() : Input.singleton.GetStateOfPrimaryDevice().Serialize();
    }
    
    // CALLED FROM GDSCRIPT  
    // ReSharper disable once UnusedMember.Global
    public Dictionary _PredictNetworkInput (Dictionary previous, int tickSinceLastConfirmed)
    {
        if (tickSinceLastConfirmed > 5)
            previous["joystick"] = "0|0";
        
        return previous;
    } 
    
    // CALLED FROM GDSCRIPT 
    // ReSharper disable once UnusedMember.Local
    private void _NetworkProcess (Dictionary input)
    {
        sfloat delta = SyncManagerWrapper.GetDeltaTime();
        int tick = SyncManagerWrapper.GetCurrentTick();
        
        InputState inp = InputState.Deserialize(input);
        
        ResolveCorrectState(inp, tick);
        
        _CurrentState.Tick(tick, delta, inp);
        
        sfloat2 traveled = MoveAndSlide(Velocity * delta, Collided);
        
        if(traveled != sfloat2.Zero)
            IsGrounded = IsGrounded();
    } 

    // CALLED FROM GDSCRIPT
    // ReSharper disable once UnusedMember.Global
    public void _InterpolateState (Dictionary old, Dictionary _new, float weight)
    {
        string pos1 = (string)old["position"];
        string pos2 = (string)_new["position"];

        Position = sfloat2.Lerp(sfloat2.FromString(pos1), sfloat2.FromString(pos2), (sfloat)weight);
    } 

    // CALLED FROM GDSCRIPT
    // ReSharper disable once UnusedMember.Global
    public Dictionary _SaveState ()
    {
        return new Dictionary
        {
            // common
            { "health", Health },
            { "position", Position.SerializeToString() },
            { "orientation", Orientation == Orientation.Left ? "l": "r" },
            // physics
            { "ground_tag", GroundTag },
            { "collider_position", Collider.Position.SerializeToString() },
            { "hurtbox_position", Hurtbox.Position.SerializeToString() },
            { "hurtbox_size", Hurtbox.HalfExtents.SerializeToString() },
            { "hitbox_position", Hitbox.Position.SerializeToString() },
            { "hitbox_size", Hitbox.HalfExtents.SerializeToString() },
            { "is_grounded", IsGrounded ? "1" : "0" },
            { "velocity", Velocity.SerializeToString() },
            // states
            { "state", _CurrentState.ToString() },
            { "jab_state", _JabAction._Serialize() },
            { "hit_state", _HitState._Serialize() },
            { "dash_ability", _DashAbility._Serialize() }
        };
    }

    // CALLED FROM GDSCRIPT
    // ReSharper disable once UnusedMember.Global
    public void _LoadState (Dictionary state)
    {
        // common
        Health = (int)state["health"];
        Position = sfloat2.FromString((string)state["position"]);
        Orientation = (string)state["orientation"] == "r" ? Orientation.Right : Orientation.Left;
        
        // physics
        GroundTag = (string)state["ground_tag"];
        Collider.Position = sfloat2.FromString((string)state["collider_position"]);
        Hurtbox.Position = sfloat2.FromString((string)state["hurtbox_position"]);
        Hurtbox.HalfExtents = sfloat2.FromString((string)state["hurtbox_size"]);
        Hitbox.Position = sfloat2.FromString((string)state["hitbox_position"]);
        Hitbox.HalfExtents = sfloat2.FromString((string)state["hitbox_size"]);
        
        Velocity = sfloat2.FromString((string)state["velocity"]);
        IsGrounded = (string)state["is_grounded"] == "1";

        // states
        _CurrentState = (string)state["state"] switch
        {
            "Idle" => _IdleState,
            "Running" => _RunningState,
            "Ascending" => _AscendingState,
            "Falling" => _FallingState,
            "Jab Action" => _JabAction,
            "Dash" => _DashAbility,
            "Hit" => _HitState,
            _ => _CurrentState
        };

        _JabAction._Deserialize(state["jab_state"] as Dictionary);
        _HitState._Deserialize(state["hit_state"] as Dictionary);
        _DashAbility._Deserialize(state["dash_ability"] as Dictionary);
    }
}
