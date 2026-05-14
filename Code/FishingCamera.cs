using Sandbox;

public sealed class FishingCamera : Component
{
	[Property] public FishingPlayer Player { get; set; }
	[Property] public GameObject IdleAnchor { get; set; }
	[Property] public GameObject FightingAnchor { get; set; }

	[Property, Range( 0f, 20f )]
	public float TransitionSpeed { get; set; } = 3f;

	private Vector3 _currentOffset;

	protected override void OnUpdate()
	{
		if ( Player == null || IdleAnchor == null || FightingAnchor == null )
			return;

		var target = Player.IsFighting ? FightingAnchor : IdleAnchor;

		WorldPosition = Vector3.Lerp( WorldPosition, target.WorldPosition, Time.Delta * TransitionSpeed );
		WorldRotation = Rotation.Slerp( WorldRotation, target.WorldRotation, Time.Delta * TransitionSpeed );
	}
}
