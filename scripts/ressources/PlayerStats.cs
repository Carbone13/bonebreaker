using Godot;

public class PlayerStats : Resource
{
    [Export] public int Health { get; set; }
    [Export] public int MoveSpeed { get; set; }
    [Export] public float Acceleration { get; set; }
    [Export] public int JumpHeight { get; set; }
    [Export] public float JumpApexTime { get; set; }
    [Export] public float InAirDamping { get; set; }
    [Export] public float FastFallingMultiplier { get; set; }
    [Export] public int JabCount { get; set; }
    [Export] public int JabResetTicks { get; set; }
    [Export] public int JabCooldown { get; set; }
    [Export] public bool CanDash { get; set; }
    [Export] public int DashSpeed { get; set; }
    [Export] public int DashDuration { get; set; }
    [Export] public int DashCooldownTicks { get; set; }
    [Export] public int Damage { get; set; }
    
    public sfloat Gravity { get; private set; }
    public sfloat JumpVelocity { get; private set; }
    
    public void ComputeStats ()
    {
        Gravity = ((sfloat)2 * (sfloat)JumpHeight) / ((sfloat)JumpApexTime * (sfloat)JumpApexTime);
        JumpVelocity = libm.sqrtf((sfloat)2 * Gravity * (sfloat)JumpHeight);
    }
}
