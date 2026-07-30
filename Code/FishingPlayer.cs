using Sandbox;
using System;

public sealed class FishingPlayer : Component
{
	//Reeling logic
	[Property, Range( 0f, 1f )]
	public float ReelDeadzone { get; set; } = 0.05f;

	public float ReelAngularVelocity { get; private set; }
	public float AccumulatedRotation { get; private set; }
	public float reelSpeed;
	private float _previousAngle;
	private bool _hasPreviousAngle;
//	private bool isReelAnimPlaying;

	[Property] public float CastDistance { get; set; } = 1000f;     // where fish spawns
	[Property] public float MaxLineDistance { get; set; } = 1300f;  // where escape fires
	[Property] public float ReelInPower { get; set; } = 10f;        // how much distance per rad of reel — tune
	[Property] public float WinDistance { get; set; } = 40f;
	public float LineDistance { get; private set; }

	[Property] public float FishPullPower { get; set; } = 10f;

	[Property] public float TensionBuildRate { get; set; } = 1f;
	[Property] public float TensionDecayRate { get; set; } = 0.4f;
	public float Tension { get; private set; }

	//state changing logic
	public enum FishingState { Idle, Casting, Waiting, Fighting }
	private FishingState _state = FishingState.Idle;

	public bool IsFighting => _state == FishingState.Fighting;

	bool reelingFast;
	bool pulling;

	//Rod and bones references
	[Property] public SkinnedModelRenderer PlayerModel { get; set; }
	[Property] public GameObject RodObject { get; set; }
	private GameObject _rodInstance;

	[Property] public string HandBoneName { get; set; } = "hold_L";
	[Property] public string CrankBoneName { get; set; } = "Crank";

	private GameObject handBoneObject;
	private GameObject crankBoneObject;
	float crankAngle;
	Rotation OriginalCrankRotation;

	//Fish references
	[Property] public GameObject FishPrefab { get; set; }	
	public Fish HookedFish { get; private set; }
	[Property] public GameObject Hook { get; set; }

	protected override void OnStart()
	{

		if ( PlayerModel != null )
		{
			handBoneObject = PlayerModel.GetBoneObject( HandBoneName );

			if ( !handBoneObject.IsValid() )
			{
				Log.Error( $"Could not find bone: {HandBoneName}" );
				return;
			}
		}
		else
		{
			Log.Warning( "PlayerModel not assigned, can't find hand bone for rod parenting." );
		}

		if ( RodObject != null )
		{
			_rodInstance = RodObject.Clone( WorldPosition );
		}
		else
		{
			Log.Warning( "RodObject not assigned, can't create rod instance." );
		}

		//Parent rod to hand bone, set initial position/rotation
		if ( _rodInstance != null && handBoneObject != null )
		{
			_rodInstance.SetParent( handBoneObject, false );
			_rodInstance.LocalPosition = new Vector3( 1, 5, -25 );
			_rodInstance.LocalRotation = Rotation.From( new Angles( -15, 60, -6 ) );
			Log.Info( _rodInstance.Parent == handBoneObject
				? "Rod parented correctly"
				: $"Rod parent failed. Parent is: {_rodInstance.Parent?.Name ?? "null"}" );

		}
		else
		{
			Log.Warning( "Cannot parent rod: missing rod instance or hand bone." );
		}

		var rodRenderer = _rodInstance?.GetComponent<SkinnedModelRenderer>();

		//For later rotation of rod crank
		if ( rodRenderer != null )
		{
			crankBoneObject = rodRenderer.GetBoneObject( CrankBoneName );
			OriginalCrankRotation = crankBoneObject.LocalRotation;
		}
		else		{
			Log.Warning( "Rod instance does not have a SkinnedModelRenderer, can't find crank bone." );
		}

	}

	protected override void OnUpdate()
	{
		switch ( _state )
		{
			case FishingState.Idle:
				if ( Input.Pressed( "Cast" ) )
				{
					_state = FishingState.Casting;
					Log.Info( "Casting!" );
				}
				break;

			case FishingState.Casting:
				// TODO: cast timer / animation, then → Waiting
				Log.Info( "yeah baby we casting" );
				_state = FishingState.Waiting;
				break;

			case FishingState.Waiting:
				// TODO: random bite timer, then spawn fish → Fighting
				LineDistance = CastDistance;
				var spawnPos = WorldPosition + WorldRotation.Forward * CastDistance;
				var fishObject = FishPrefab.Clone( spawnPos );
				HookedFish = fishObject.GetComponent<Fish>();
				Log.Info( "Fish hooked!" );
				_state = FishingState.Fighting;
				break;

			case FishingState.Fighting:
				// TODO: read fish.PullStrength, update tension/distance, check win/lose
				UpdateReel();

				if ( MathF.Abs( ReelAngularVelocity ) > 1f )
				{
					LineDistance -= MathF.Abs( ReelAngularVelocity ) * ReelInPower * Time.Delta;
					// when reel starts
					PlayerModel.Parameters.Set( "b_reeling", true );

					reelSpeed = MathX.Lerp( reelSpeed, MathF.Abs( ReelAngularVelocity ), Time.Delta * 10f );
					PlayerModel.Parameters.Set( "reel_speed", reelSpeed );

					Log.Info( $"reel ω = {reelSpeed:F2} rad/s" );

					crankAngle += reelSpeed * 20f * Time.Delta;
					
					crankBoneObject.LocalRotation = OriginalCrankRotation * Rotation.FromPitch( crankAngle );

				}
				else
				{
					// when reel stops
					PlayerModel.Parameters.Set( "b_reeling", false );
					PlayerModel.Parameters.Set( "reel_speed", 0 );
					PlayerModel.Parameters.Set( "b_pulling", false );
				}

				LineDistance += HookedFish.PullStrength * FishPullPower * Time.Delta;
				LineDistance = MathX.Clamp( LineDistance, 0f, MaxLineDistance );

				HookedFish.WorldPosition = WorldPosition + WorldRotation.Forward * LineDistance;
				Hook.WorldPosition = HookedFish.WorldPosition;   

				//this reeling decides if the speed is high enough to create tension
				reelingFast = MathF.Abs( ReelAngularVelocity ) > 8f;

				pulling = HookedFish.PullStrength > 0.1f;

				if ( pulling )
				{
					PlayerModel.Parameters.Set( "b_pulling", true );
					Log.Info( "FISH IS ESCAPING!!!!" );
				}
				else
				{
					PlayerModel.Parameters.Set( "b_pulling", false );
					Log.Info( "REEL!!!!" );
				}


				if ( reelingFast && pulling )
					Tension += TensionBuildRate * Time.Delta;
				else
					Tension -= TensionDecayRate * Time.Delta;

				Tension = MathX.Clamp( Tension, 0f, 1f );

				//Log.Info( $"Tension: {Tension}" );

				if ( Tension >= 1f )
				{
					Log.Info( "Line broke!" );
					PlayerModel.Parameters.Set( "reel_speed", 0 );
					PlayerModel.Parameters.Set( "b_pulling", false );
					EndFight();
					return;  // ← stop here, don't fall through to win/escape checks
				}

				if ( LineDistance <= WinDistance )
				{
					Log.Info( "Caught!" );
					PlayerModel.Parameters.Set( "reel_speed", 0 );
					PlayerModel.Parameters.Set( "b_pulling", false );
					EndFight();
				}
				else if ( LineDistance >= MaxLineDistance )
				{
					Log.Info( "Fish escaped!" );
					PlayerModel.Parameters.Set( "reel_speed", 0 );
					PlayerModel.Parameters.Set( "b_pulling", false );
					EndFight();
				}
				break;
		}
	}

	private void UpdateReel()
	{
		var look = Input.AnalogLook;
		var input = new Vector2( look.yaw, look.pitch );

		if ( input.Length < ReelDeadzone )
		{
			_hasPreviousAngle = false;
			ReelAngularVelocity = 0f;
			return;
		}

		float currentAngle = MathF.Atan2( input.y, input.x );

		if ( !_hasPreviousAngle )
		{
			_previousAngle = currentAngle;
			_hasPreviousAngle = true;
			ReelAngularVelocity = 0f;
			return;
		}


		float delta = currentAngle - _previousAngle;

		//manage jumps in input - if delta is bigger than half a circle in either direction, it's wrong — flip it the other way around.
		if ( delta > MathF.PI ) delta -= MathF.PI * 2f;
		else if ( delta < -MathF.PI ) delta += MathF.PI * 2f;

		float instantaneous = delta / Time.Delta;
		ReelAngularVelocity = MathX.Lerp( ReelAngularVelocity, instantaneous, Time.Delta * 10f );

		if ( MathF.Abs( ReelAngularVelocity ) > 1f )   // tune threshold
		{
			AccumulatedRotation += MathF.Abs( delta );

		}

		_previousAngle = currentAngle;

		//Log.Info( $"reel ω = {ReelAngularVelocity:F2} rad/s, total = {AccumulatedRotation  / (MathF.PI * 2f):F2} turns" );
	}

	private void EndFight()
	{
		HookedFish?.GameObject.Destroy();
		HookedFish = null;
		LineDistance = 0f;
		Tension = 0f;   
		_state = FishingState.Idle;
	}
}
