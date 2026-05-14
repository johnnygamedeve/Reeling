using Sandbox;

public sealed class Fish : Component
{
	[Property] public float MinRestDuration { get; set; } = 1f;
	[Property] public float MaxRestDuration { get; set; } = 3f;
	[Property] public float MinPullDuration { get; set; } = 1f;
	[Property] public float MaxPullDuration { get; set; } = 5f;
	//[Property, Range( 0f, 1f )]
	//public float MaxPullStrength { get; set; } = 1f;
	[Property] public bool AlwaysPull { get; set; } = false;

	public float PullStrength { get; private set; }

	private enum BehaviorState { Resting, Pulling }
	private BehaviorState _state = BehaviorState.Resting;
	private float _timer;

	protected override void OnStart()
	{
		EnterRest();
	}

	protected override void OnUpdate()
	{
		if ( AlwaysPull )
		{
			PullStrength = 1;
			return;
		}

		// existing timer logic
		_timer -= Time.Delta;
		if ( _timer > 0 ) return;

		if ( _state == BehaviorState.Resting ) EnterPull();
		else EnterRest();
	}

	private void EnterRest()
	{
		_state = BehaviorState.Resting;
		PullStrength = 0f;
		_timer = Game.Random.Float( MinRestDuration, MaxRestDuration );
	}

	private void EnterPull()
	{
		_state = BehaviorState.Pulling;
		PullStrength = 1f;// was: Game.Random.Float(...)
		_timer = Game.Random.Float( MinPullDuration, MaxPullDuration );
	}
}
