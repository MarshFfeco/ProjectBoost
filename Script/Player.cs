using Godot;

public partial class Player : RigidBody3D
{
	[Export(PropertyHint.Range, "750,3000")]
	private float Thrust = 1000.0f;

	[Export]
	private float TorqueThrust = 100.0f;

	private AudioStreamPlayer3D Engine;
	private AudioStreamPlayer3D Explosion;
	private AudioStreamPlayer Sucess;
	private GpuParticles3D Particle;
	private GpuParticles3D RightParticle;
	private GpuParticles3D LeftParticle;
	private GpuParticles3D SuccessParticle;
	private GpuParticles3D ExplosionParticle;

	private float inputHandle;
	private Vector3 direction;

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, MethodName.ActionPlayerCollider));
		Engine = GetNode<AudioStreamPlayer3D>("EngineAudio");
		Explosion = GetNode<AudioStreamPlayer3D>("ExplosionAudio");
		Sucess = GetNode<AudioStreamPlayer>("SucessAudio");

		Particle = GetNode<GpuParticles3D>("Particles/BoosterParticles");
		RightParticle = GetNode<GpuParticles3D>("Particles/RightBoosterParticles");
		LeftParticle = GetNode<GpuParticles3D>("Particles/LeftBoosterParticles");
		SuccessParticle = GetNode<GpuParticles3D>("Particles/SuccessParticles");
		ExplosionParticle = GetNode<GpuParticles3D>("Particles/ExplosionParticles");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
			GetTree().Quit();

		inputHandle = HandleInput();
		direction = new Vector3(0, 0, inputHandle).Normalized();

		if (direction == Vector3.Forward)
			LeftParticle.Emitting = true;
		else if (direction == Vector3.Back)
			RightParticle.Emitting = true;

		if (Input.IsActionPressed("boost"))
		{
			if (!Engine.Playing)
				Engine.Play();

			ApplyCentralForce(Basis.Y * (float)delta * Thrust);

			Particle.Emitting = true;
		}
		else
		{
			Engine.Stop();
			Particle.Emitting = false;
		}

		if (direction != Vector3.Zero)
			ApplyTorque(direction * (float)delta * TorqueThrust);
		else
		{
			RightParticle.Emitting = false;
			LeftParticle.Emitting = false;
		}
	}

	private void ActionPlayerCollider(Node body)
	{
		if (body.IsInGroup("Floor"))
		{
			SetDeferred("contact_monitor", false);
			CallDeferred(MethodName.IMDie);
		}
		else if (body.IsInGroup("Winner"))
		{
			SetDeferred("contact_monitor", false);
			LandingPad la = body as LandingPad;

			CallDeferred(MethodName.NextLevel, la.Path);
		}
	}

	private void NextLevel(string nextLevel)
	{
		Engine.Playing = false;
		Sucess.Playing = true;
		SetProcess(false);
		Particle.Emitting = false;
		RightParticle.Emitting = false;
		LeftParticle.Emitting = false;
		SuccessParticle.Emitting = true;

		Tween tween = GetTree().CreateTween();
		tween.TweenInterval(2.5f);
		tween.TweenCallback(Callable.From(() => GetTree().ChangeSceneToFile(nextLevel)));
	}

	private void IMDie()
	{
		Engine.Playing = false;
		Explosion.Playing = true;
		SetProcess(false);
		Particle.Emitting = false;
		RightParticle.Emitting = false;
		LeftParticle.Emitting = false;
		ExplosionParticle.Emitting = true;

		Tween tween = GetTree().CreateTween();
		tween.TweenInterval(2.5f);
		tween.TweenCallback(Callable.From(() => GetTree().ReloadCurrentScene()));
	}

	private void ReloadScene()
	{
		GetTree().ReloadCurrentScene();
	}

	private float HandleInput() => Input.GetAxis("rotate_right", "rotate_left");
}
