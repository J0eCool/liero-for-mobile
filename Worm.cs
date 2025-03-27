using Godot;

public partial class Worm : CharacterBody2D
{
    private float _aimAngle;
    private float _reticleDist;

    [Export] public Sprite2D BodySprite;
    [Export] public Sprite2D GunSprite;
    [Export] public Sprite2D ReticleSprite;

    [Export] public float MoveSpeed = 100.0f;
    [Export] public float AimSpeed = 250.0f; // in degrees/s
    [Export] public float Gravity = 500.0f;
    [Export] public float JumpHeight = 250.0f;

    public override void _Ready()
    {
        _reticleDist = ReticleSprite.Position.Length();
    }

    public override void _Process(double deltaT)
    {
        // Time init
        var dt = (float)deltaT;

        // Movement processing
        // Local copy of velocity - set base.Velocity at end
        var vel = Velocity;
        vel.X = 0;

        // Handle H input
        vel.X = MoveSpeed * Input.GetAxis("left", "right");

        // Jump 
        if (!IsOnFloor())
        {
            vel.Y += Gravity * dt;
            // Reduce jump speed when releasing jump while rising
            // allows for control over jump height by differentiating press vs press-and-hold
            if (vel.Y * Gravity < 0 && Input.IsActionJustReleased("jump"))
            {
                vel.Y *= 1 / 3.0f;
            }
        }
        else // is on floor
        {
            vel.Y = 0;
            if (Input.IsActionJustPressed("jump"))
            {
                vel.Y = -Mathf.Sign(Gravity) * Mathf.Sqrt(2 * Gravity * JumpHeight);
            }
        }

        // Flip sprites
        var isFlipped = BodySprite.FlipH;
        // explicitly not doing `isFlipped = vel.X < 0`, to maintain flip when not moving
        if (vel.X < 0) isFlipped = true;
        else if (vel.X > 0) isFlipped = false;
        BodySprite.FlipH = isFlipped;
        GunSprite.FlipH = isFlipped;

        // Handle V input
        var dAim = Input.GetAxis("up", "down");
        _aimAngle += dt * dAim * AimSpeed;
        _aimAngle = Mathf.Clamp(_aimAngle, -85, 45);
        var rPos = _reticleDist * Vector2.FromAngle(_aimAngle * Mathf.Pi / 180);
        if (isFlipped) rPos.X *= -1;
        ReticleSprite.Position = rPos;

        // Ship it!!!!
        Velocity = vel;
        MoveAndSlide();
    }
}