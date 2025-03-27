using Godot;

public partial class Worm : CharacterBody2D
{
    private float _aimAngle;

    private Sprite2D _body;
    private Sprite2D _gun;
    private Sprite2D _reticle;
    private float _reticleDist;

    [Export] public float MoveSpeed = 100.0f;
    [Export] public float AimSpeed = 250.0f; // in degrees/s
    [Export] public float Gravity = 500.0f;
    [Export] public float JumpHeight = 250.0f;

    public override void _Ready()
    {
        _body = FindChild("Body") as Sprite2D;
        _gun = FindChild("Gun") as Sprite2D;
        _reticle = FindChild("Reticle") as Sprite2D;
        _reticleDist = _reticle.Position.Length();
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
        if (Input.IsActionPressed("left"))
        {
            vel.X -= MoveSpeed;
        }

        if (Input.IsActionPressed("right"))
        {
            vel.X += MoveSpeed;
        }

        // Jump 
        if (!IsOnFloor())
        {
            vel.Y += Gravity * dt;
            if (vel.Y * Gravity < 0 && Input.IsActionJustReleased("jump"))
            {
                vel.Y *= 1 / 3.0f;
            }
        }
        else
        {
            vel.Y = 0;
            if (Input.IsActionJustPressed("jump"))
            {
                vel.Y = -Mathf.Sign(Gravity) * Mathf.Sqrt(2 * Gravity * JumpHeight);
            }
        }

        // Flip sprites
        var isFlipped = _body.FlipH;
        // explicitly not doing `isFlipped = vel.X < 0`, to maintain flip when not moving
        if (vel.X < 0) isFlipped = true;
        else if (vel.X > 0) isFlipped = false;

        _body.FlipH = isFlipped;
        _gun.FlipH = isFlipped;

        // Handle V input
        var dAim = Input.GetAxis("up", "down");
        _aimAngle += dt * dAim * AimSpeed;
        _aimAngle = Mathf.Clamp(_aimAngle, -85, 45);
        var rPos = _reticleDist * Vector2.FromAngle(_aimAngle * Mathf.Pi / 180);
        if (isFlipped) rPos.X *= -1;

        _reticle.Position = rPos;

        // Ship it!!!!
        Velocity = vel;
        MoveAndSlide();
    }
}