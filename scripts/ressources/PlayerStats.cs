using Godot;
using System;

public class PlayerStats : Resource
{
    [Export] public int Health { get; set; }
    [Export] public int MoveSpeed { get; set; }
    [Export] public int JumpHeight { get; set; }
    [Export] public float JumpApexTime { get; set; }
    [Export] public float InAirDamping { get; set; }
    public sfloat Gravity { get; private set; }
    public sfloat JumpVelocity { get; private set; }
    
    public void ComputeStats ()
    {
        Gravity = ((sfloat)2 * (sfloat)JumpHeight) / ((sfloat)JumpApexTime * (sfloat)JumpApexTime);
        JumpVelocity = libm.sqrtf((sfloat)2 * Gravity * (sfloat)JumpHeight);
    }
}
