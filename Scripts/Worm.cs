using System;
using Godot;

public partial class Worm : CharacterBody2D
{
    // Constants
    private const int AimAngleLowerBound = 60; // OpenLieroX/src/common/CWormHuman.cpp:522 
    private const int AimAngleUpperBound = -90;
    private const float AimFrictionCoefficient = 300.0f;

    private const float AimAccelConst = 5.0f; // Not sure if exact match for OG liero
    private const float AimMaxSpeed = 200.0f; // Slightly slower than OG Liero, could use some tweaking

    // Movement/positioning state vars
    private bool _facingRight = true;
    private float _aimAngleSpeed = 0.0f;
    private float _aimAngle = 0.0f;
    private float _reticleDist = 0.0f;

    [Export] public Sprite2D BodySprite;
    [Export] public Sprite2D GunSprite;
    [Export] public Sprite2D ReticleSprite;

    [Export] public float MoveSpeed = 100.0f;

    [Export] public float AimAccel = 100.0f; // in degrees/s

    // [Export] public float AimSpeed = 250.0f; // in degrees/s
    [Export] public float Gravity = 500.0f;
    [Export] public float JumpHeight = 250.0f;

    [Export] public PackedScene BulletScene;
    [Export] public float ShootSpeed = 1500.0f;

    [Export] public bool IsServerPlayer = false;

    public override void _Ready()
    {
        if (!IsPlayerControlled()) return;

        _reticleDist = ReticleSprite.Position.Length();
        BodySprite.Modulate = PlayerSettings.Instance.WormColor;
    }

    public override void _Process(double deltaT)
    {
        if (!IsPlayerControlled()) return;

        // Time init
        var dt = (float)deltaT; // Frame time in seconds

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
        // explicitly not doing `isFlipped = vel.X < 0`, to maintain flip when not moving
        if (vel.X < 0) _facingRight = false;
        else if (vel.X > 0) _facingRight = true;
        BodySprite.FlipH = !_facingRight;
        GunSprite.FlipH = !_facingRight;

        // Handle V input (aiming up or down)
        CalculateAimAngleSpeed(dt);

        _aimAngle += _aimAngleSpeed * dt;

        if (AimAngleClamped())
        {
            // If position is at a limit, speed goes to 0
            _aimAngleSpeed = 0;
        }

        ReticleSprite.Position = _reticleDist * AimingDir();

        // Ship it!!!!
        Velocity = vel;
        MoveAndSlide();

        if (Input.IsActionJustPressed("shoot"))
        {
            var bullet = BulletScene.Instantiate<RigidBody2D>();
            bullet.Position = Position;
            bullet.LinearVelocity = ShootSpeed * AimingDir();
            GetTree().CurrentScene.AddChild(bullet);
        }

        if (Input.IsActionJustPressed("menu"))
        {
            // GetTree().Root.RemoveChild(GetTree().CurrentScene);
            GetTree().ChangeSceneToFile("res://Scenes/title_screen.tscn");
        }
    }

    private void CalculateAimAngleSpeed(float dt)
    {
        // Aim direction constant multiplier
        var dAim = Input.GetAxis("up", "down"); // -1 : up, 1 : down, 0 : nothing

        if (dAim != 0.0)
        {
            // Up or down is pressed/held
            _aimAngleSpeed += AimAccel * dAim * dt * AimAccelConst;
            _aimAngleSpeed = Mathf.Clamp(_aimAngleSpeed, -AimMaxSpeed, AimMaxSpeed);
        }
        else if (_aimAngleSpeed != 0)
        {
            // Decelerate to 0 if no input
            _aimAngleSpeed = ReduceByConstantAmount(_aimAngleSpeed, AimFrictionCoefficient * dt);

            // Set to 0 if sufficiently low
            if (Math.Abs(_aimAngleSpeed) < 5.0f && _aimAngleSpeed != 0.0f)
            {
                _aimAngleSpeed = 0.0f;
            }
        }
    }

    float ReduceByConstantAmount(float input, float amount)
    {
        if (input > 0.0)
        {
            var reduction = input - (amount);
            input = Math.Max(0, reduction);
        }
        else if (input < 0.0)
        {
            var reduction = input + (amount);
            input = Math.Min(0, reduction);
        }

        return input;
    }

    private Vector2 AimingDir()
    {
        var ret = Vector2.FromAngle(_aimAngle * Mathf.Pi / 180);
        if (!_facingRight) ret.X *= -1;
        return ret;
    }

    // Tries to clamp the _aimAngle between its bounds, returns true iff original value was OOB 
    private bool AimAngleClamped()
    {
        float originalAngle = _aimAngle;
        _aimAngle = Mathf.Clamp(_aimAngle, AimAngleUpperBound, AimAngleLowerBound);
        if (originalAngle > AimAngleLowerBound || originalAngle < AimAngleUpperBound)
        {
            return true;
        }

        return false;
    }

    // True iff this specific worm is our player character
    private bool IsPlayerControlled()
    {
        // Testing whether we *think* we're the server player; no need to check Multiplayer.IsServer()
        return IsServerPlayer == PlayerSettings.Instance.IsServerPlayer;
    }
}