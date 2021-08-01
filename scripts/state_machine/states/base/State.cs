using Bonebreaker.Inputs;
using Bonebreaker.Player;
using Bonebreaker.StateMachine;

public class State
{
	public State (Character owner)
	{
		Owner = owner;
	}
	
	public Character Owner;
	
	#region Public Functions
	
	public void Enter (State previousState = null)
	{
		_CheckExitConditions(Owner.GetInput());
		_Enter(previousState);
	}
	
	public void Exit (State nextState = null)
	{
		_Exit(nextState);	
	}
	
	public void Tick (sfloat delta, uint frame, InputState input)
	{
		_CheckExitConditions(input);
		_Tick(delta, frame, input);
	}

	public void CheckExitConditions (InputState input)
	{
		_CheckExitConditions(input);
	}
	
	#endregion
	
	#region Overridable
	
	protected virtual void _Enter (State previousState) {}
	protected virtual void _Exit (State nextState) {}
	protected virtual void _Tick (sfloat delta, uint frame, InputState input) {}
	protected virtual void _CheckExitConditions (InputState input) {}
	
	#endregion
}
