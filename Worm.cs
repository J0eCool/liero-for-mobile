using Godot;
using System;
using System.Diagnostics;

public partial class Worm : CharacterBody2D {
	[Export] public float Gravity = 500.0f;

	[Export] public float Speeds = 100.0f;
	[Export] public float JumpHeight = 250.0f;
	
	// Sprite2D sprite = FindChild("Body") as Sprite2D;
	private Sprite2D _body;
	private Sprite2D _gun;

	public override void _Ready() {
		_body = FindChild("Body") as Sprite2D;
		_gun = FindChild("Gun") as Sprite2D;
	}

	public override void _Process(double deltaT) {
		// Time init
		float dt = (float)deltaT;

		// Movement processing
		// Local copy of velocity - set base.Velocity at end 
		Vector2 vel = Velocity;
		vel.X = 0;
		
		// Handle H input 
		if (Input.IsActionPressed("left")) {
			vel.X -= Speeds;
		}
		if (Input.IsActionPressed("right")) {
			vel.X += Speeds;
		}
		
		// Jump 
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

		// Flip sprites
		if (vel.X < 0) {
			// Face left
			_body.FlipH = true;
			_gun.FlipH = true;
		} else if (vel.X > 0) {
			// Face right
			_body.FlipH = false;
			_gun.FlipH = false;
		}

		// Ship it!!!!
		Velocity = vel;
		MoveAndSlide();
	}
}
