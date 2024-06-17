using Godot;
using System;

public partial class MovingHazad : AnimatableBody3D
{
	[Export]
	private Vector3 Destination;

	[Export]
	private float Duration = 1.0f;


	public override void _Ready()
	{
		Tween tween = CreateTween();
		tween.SetLoops();
		tween.SetTrans(Tween.TransitionType.Spring);
		tween.TweenProperty(this, "global_position", GlobalPosition + Destination, Duration);
		tween.TweenProperty(this, "global_position", GlobalPosition, Duration);

	}
}
