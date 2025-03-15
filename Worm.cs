using Godot;
using System;

public partial class Worm : CharacterBody2D {
    [Export] public float Gravity = 500.0f;

    [Export] public float Speeds = 100.0f;
    [Export] public float JumpHeight = 250.0f;


    public override void _Process(double deltaT) {
        float dt = (float)deltaT;

        Vector2 vel = Velocity;
        vel.X = 0;
        if (Input.IsActionPressed("left")) {
            vel.X -= Speeds;
        }
        if (Input.IsActionPressed("right")) {
            vel.X += Speeds;
        }

        if (!IsOnFloor()) {
            vel.Y += Gravity*dt;
            if (vel.Y*Gravity < 0 && Input.IsActionJustReleased("jump")) {
                vel.Y *= 1/3.0f;
            }
        } else {
            vel.Y = 0;
            if (Input.IsActionJustPressed("jump")) {
                vel.Y = -Mathf.Sign(Gravity)*Mathf.Sqrt(2*Gravity*JumpHeight);
            }
        }

        if (vel.X < 0) {
            // flip the sprite turnways
        } else if (vel.X > 0) {
            // flipt it normdmal
        }

        Velocity = vel;
        MoveAndSlide();
    }
}
