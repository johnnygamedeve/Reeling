using Sandbox;
using System;
using System.Collections.Generic;

public sealed class FishingLine : Component
{
	[Property] public FishingPlayer Player { get; set; }
	[Property] public GameObject RodTip { get; set; }
	[Property] public GameObject Hook { get; set; }
	[Property] public int SegmentCount { get; set; } = 12;
	[Property] public float Gravity { get; set; } = 1400f;
	[Property] public int Iterations { get; set; } = 20;
	[Property] public float MinLineLength { get; set; } = 20f;

	// FishingPlayer flips this — unpinned during a cast so the hook flies free
	public bool HookPinned { get; set; } = true;

	private LineRenderer _line;
	private Vector3[] _current;       // where each rope point is this frame
	private Vector3[] _previous;      // where each rope point was last frame
	private float _segmentLength;     // target distance between neighbouring points
	private List<Vector3> _renderPoints;

	// Runs once when the component starts. Sets up the rope: allocates the point
	// arrays and strings them in a straight line between the rod tip and the hook.
	protected override void OnStart()
	{
		_line = GetComponent<LineRenderer>();

		if ( RodTip == null || Hook == null )
		{
			Log.Warning( "FishingLine: RodTip or Hook not assigned." );
			return;
		}

		int pointCount = SegmentCount + 1;
		_current = new Vector3[pointCount];
		_previous = new Vector3[pointCount];
		_renderPoints = new List<Vector3>( pointCount );

		// Spread the points evenly between the two ends as a starting pose.
		Vector3 start = RodTip.WorldPosition;
		Vector3 end = Hook.WorldPosition;
		for ( int i = 0; i < pointCount; i++ )
		{
			float t = (float)i / SegmentCount;
			_current[i] = Vector3.Lerp( start, end, t );
			_previous[i] = _current[i];   // current == previous means zero starting velocity
		}
	}

	// Runs every frame. This is the simulation loop: size the rope, integrate,
	// constrain it back together, nail the ends, then hand the points to the renderer.
	protected override void OnUpdate()
	{
		if ( Player == null || RodTip == null || Hook == null || _current == null )
			return;

		// The rope's length isn't fixed — it tracks how much line is out (LineDistance).
		// MathF.Max keeps it from collapsing to a dot when LineDistance is 0 (idle).
		float length = MathF.Max( Player.LineDistance, MinLineLength );
		_segmentLength = length / SegmentCount;

		Integrate();

		// Each constraint pass nudges points and slightly breaks the others, so we
		// repeat the whole pass several times to settle the rope. More = stiffer.
		for ( int i = 0; i < Iterations; i++ )
			ApplyConstraints();

		PinEndpoints();
		PushToRenderer();
	}

	// STEP 1 — Integrate: let every point coast and fall.
	// Verlet has no velocity variable; velocity is implied by (current - previous).
	// Because it's derived, any later step that shoves a point also changes its
	// velocity automatically — that's what makes constraints "just work".
	private void Integrate()
	{
		Vector3 gravityStep = Vector3.Down * Gravity * Time.Delta * Time.Delta;

		for ( int i = 0; i < _current.Length; i++ )
		{
			Vector3 velocity = _current[i] - _previous[i];   // implied velocity
			_previous[i] = _current[i];                      // remember where we were
			_current[i] += velocity + gravityStep;           // keep moving + gravity pull
		}
	}

	// STEP 2 — Constrain: pull every neighbouring pair back to _segmentLength apart.
	// Without this the points just rain down under gravity. Each point in a pair
	// moves half the correction, so they meet in the middle.
	private void ApplyConstraints()
	{
		for ( int i = 0; i < _current.Length - 1; i++ )
		{
			Vector3 delta = _current[i + 1] - _current[i];
			float distance = delta.Length;

			if ( distance < 0.0001f )
				continue;   // points stacked exactly — skip to avoid divide-by-zero

			// difference > 0 means too close (push apart), < 0 means too far (pull together)
			float difference = (_segmentLength - distance) / distance;
			Vector3 correction = delta * 0.5f * difference;

			_current[i]     -= correction;
			_current[i + 1] += correction;
		}
	}

	// STEP 3 — Pin the endpoints: override the simulation for the two ends.
	// The rod tip is always nailed to the rod. The hook is nailed only when pinned;
	// while unpinned (during a cast) it's a free point the rope drags around.
	private void PinEndpoints()
	{
		_current[0] = RodTip.WorldPosition;

		if ( HookPinned )
			_current[_current.Length - 1] = Hook.WorldPosition;
	}

	// STEP 4 — Hand the computed point positions to the LineRenderer to draw.
	// Reuses one list instead of allocating a new one every frame.
	private void PushToRenderer()
	{
		_renderPoints.Clear();
		for ( int i = 0; i < _current.Length; i++ )
			_renderPoints.Add( _current[i] );

		_line.VectorPoints = _renderPoints;
	}
}
