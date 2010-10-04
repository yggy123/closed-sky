
//Namespaces used
using System;
using Klotski.Utilities;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Game main character, Klotski.
	/// </summary>
	public class Actor : Playable {
		//Control
		private bool	m_Jumping;
		private float	m_RotationX;
		private float	m_RotationY;
		private Vector3	m_Forward;
		private Vector3	m_Sideway;
		private Vector3 m_Movement;

		/// <summary>
		/// Actor class constructor.
		/// </summary>
		/// <param name="layer">ACtor's layer</param>
		public Actor(Layer layer) : base(layer) {
			//Empties variables
			m_RotationX = 0.0f;
			m_RotationY = 0.0f;
			m_Movement	= Vector3.Zero;

			//Initialize values
			m_Jumping	= false;
			m_Forward	= new Vector3(0.0f, 0.0f, 1.0f);
			m_Sideway	= new Vector3(-1.0f, 0.0f, 0.0f);
		}

		/// <summary>
		/// Initialize actor
		/// </summary>
		/// <param name="x">Actor's X position</param>
		/// <param name="y">Actor's Y position</param>
		/// <param name="z">Actor's Z position</param>
		public void Initialize(float x, float y, float z) {
			//Calls parent initialization
			Initialize(Global.ACTOR_MODEL, true, x, y, z);

			//Set model animation
			m_Model.CurrentAnimation = Global.ACTOR_ANIMATIONS[1];
			m_Camera.X = 0;
			m_Camera.Z = 0;

			//Set camera orientation
			m_Camera.RotationX = 0;
			m_Camera.RotationY = (float)Math.PI;
			m_Camera.RotationZ = 0;
		}

		public void SetPosition(float x, float y, float z) {
			m_Model.Position = new Vector3(x, y, z);
		}

		/// <summary>
		/// Check player input and updates accordingly.
		/// </summary>
		/// <param name="time">Time data</param>
		public override void UpdateControl(GameTime time) {
			//Reset movement
			m_Movement = Vector3.Zero;

			//Check camera control
			m_RotationX = -InputManager.Mouse.XChange / 100.0f;
			m_RotationY = -InputManager.Mouse.YChange / 100.0f;

			//Set movement based on player input
			if (InputManager.Keyboard.KeyDown(Keys.W)) m_Movement += m_Forward;
			if (InputManager.Keyboard.KeyDown(Keys.S)) m_Movement -= m_Forward;
			if (InputManager.Keyboard.KeyDown(Keys.D)) m_Movement += m_Sideway;
			if (InputManager.Keyboard.KeyDown(Keys.A)) m_Movement -= m_Sideway;

			//Normalize directions if needed
			if (m_Movement.LengthSquared() > 0) m_Movement = Vector3.Normalize(m_Movement);
		}

		/// <summary>
		/// Updates the actor.
		/// </summary>
		/// <param name="time">Time data</param>
		public override void Update(GameTime time) {
			//Get time difference
			float Difference = time.ElapsedGameTime.Milliseconds / 1000.0f;

			//TODO: Fix character rotation

			//Moves character
			m_Model.Position += m_Movement * Global.ACTOR_VELOCITY * Difference;
			if (m_Movement.LengthSquared() > 0) m_Model.RotationY = (float)Math.Asin(m_Movement.X);

			//TODO: Apply gravity

			//Updates direction
			m_Forward = Vector3.Transform(m_Forward, Matrix.CreateRotationY(m_RotationX));
			m_Sideway = Vector3.Transform(m_Sideway, Matrix.CreateRotationY(m_RotationX));

			//TODO: Limits camera

			//Updates camera
			//Vector3 Behind = -Vector3.Normalize(m_Forward + new Vector3(0, 0.1));
			m_Camera.Position = m_Model.Position - 
				(m_Forward * Global.ACTORCAM_DISTANCE) + 
				new Vector3(0, Global.ACTORCAM_HEIGHT, 0);
			m_Camera.RotationX += m_RotationY;
			m_Camera.RotationY += m_RotationX;

			//Jump with space
			if (InputManager.Keyboard.KeyDown(Keys.Space) && !m_Jumping) {
				m_Model.CurrentAnimation = "Jumping";

				m_Model.YVelocity = Global.ACTOR_JUMPING;
				m_Model.YAcceleration = -Global.GAME_GRAVITY;
				m_Jumping = true;
			}
		}

		public void OnCollision(Ship ship) {
			//If jumping
			if (m_Jumping) {
				//Stop jumping
				m_Model.YVelocity = 0.0f;
				m_Model.YAcceleration = 0.0f;
				m_Jumping = false;
				m_Model.CurrentAnimation = "Walking";

				//Place it
				m_Model.Y = ship.GetBoundingBox().Max.Y;
			}
		}
	}
}
