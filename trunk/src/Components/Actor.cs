
//Namespaces used
using System;
using Klotski.Utilities;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FlatRedBall.Graphics.Lighting;

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

        //Data
		private int			m_Life;
		private PointLight	m_Light;
        private Vector3		m_SavedVelocity;
        private Vector3		m_SavedAcceleration;

	    private Ship m_LastShip;

		/// <summary>
		/// Actor class constructor.
		/// </summary>
		/// <param name="layer">Actor's layer</param>
		public Actor(Layer layer) : base(layer) {
			//Empties variables
            m_LastShip          = null;
			m_RotationX         = 0.0f;
			m_RotationY         = 0.0f;
			m_Movement	        = Vector3.Zero;
            m_SavedVelocity     = Vector3.Zero;
            m_SavedAcceleration = Vector3.Zero;

			//Initialize values
            m_Life      = 0;
			m_Jumping	= false;
			m_Forward	= new Vector3(0.0f, 0.0f, 1.0f);
			m_Sideway	= new Vector3(-1.0f, 0.0f, 0.0f);

			//Create light
			m_Light					= new PointLight();
			m_Light.Range			= 40.0f;
			m_Light.DiffuseColor	= new Vector3(1.0f, 1.0f, 1.0f);
			m_Light.SpecularColor	= new Vector3(0.3f, 0.3f, 0.3f);
		}

	    /// <summary>
	    /// Initialize actor
	    /// </summary>
	    /// <param name="ship">The ship the character is on.</param>
	    /// <param name="life"></param>
	    public void Initialize(Ship ship, int life) {
            //Set character's life
		    m_Life      = life;
		    m_LastShip  = ship;

            //Get character's position
	    	Vector3 Position  = ship.GetCenterTop();
	    	Position.Y		 += 25.0f;

			//Calls parent initialization
			Initialize(Global.ACTOR_MODEL, true, Position.X, Position.Y, Position.Z);

			//Set model animation
			m_Model.CurrentAnimation = Global.ACTOR_ANIMATIONS[1];
			m_Camera.X = 0;
			m_Camera.Z = 0;

			//Set camera orientation
			m_Camera.RotationX = 0;
			m_Camera.RotationY = (float)Math.PI;
			m_Camera.RotationZ = 0;

			//Attach light
	    	m_Light.Position	= m_Model.Position;
	    	m_Light.Position.Y += 5.0f;
	    	m_Light.Position.Z += 5.0f;
			m_Light.AttachTo(m_Model, true);
	    }

		public PointLight GetLight() {
			return m_Light;
		}
        
        /// <summary>
        /// Save actor's current data.
        /// </summary>
        public void SaveState() {
            //Save data
            m_SavedVelocity     = m_Model.Velocity;
            m_SavedAcceleration = m_Model.Acceleration;

            //Reset
            m_Model.Velocity        = Vector3.Zero;
            m_Model.Acceleration    = Vector3.Zero;
            m_Model.Animate         = false;
        }

        /// <summary>
        /// Restore actor's saved data
        /// </summary>
        public void Restore() {
            //Restore velocity and acceleration
            m_Model.Velocity        = m_SavedVelocity;
            m_Model.Acceleration    = m_SavedAcceleration;
            m_Model.Animate         = true;
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

            //Apply gravity
            m_Model.YAcceleration = -Global.GAME_GRAVITY;

			//Moves and rotate character
			m_Model.Position += m_Movement * Global.ACTOR_VELOCITY * Difference;
			if (m_Movement.LengthSquared() > 0) m_Model.RotationY = (float)Math.Atan2(m_Movement.X, m_Movement.Z);

            //If under certain limit
            if (m_Model.Position.Y <= Global.GAME_FALLLIMIT) {
                //Reset stuff
                m_Jumping = false;
                m_Model.YVelocity = 0.0f;
                m_Model.CurrentAnimation = "Walking";

                //Reduce life
                m_Life--;

                //Return to last ship visited
                m_Model.Position	= m_LastShip.GetCenterTop();
            	m_Model.Position.Y += 5.0f;
            }

			//Updates direction
			m_Forward = Vector3.Transform(m_Forward, Matrix.CreateRotationY(m_RotationX));
			m_Sideway = Vector3.Transform(m_Sideway, Matrix.CreateRotationY(m_RotationX));

			//Updates camera
			m_Camera.Position = m_Model.Position - 
				(m_Forward * Global.ACTORCAM_DISTANCE) + 
				new Vector3(0, Global.ACTORCAM_HEIGHT, 0);
			m_Camera.RotationX += m_RotationY;
			m_Camera.RotationY += m_RotationX;

            //Limits camera
            if (m_Camera.RotationX > Math.PI && m_Camera.RotationX < Math.PI * 1.5f) m_Camera.RotationX = (float) (Math.PI * 1.5f);
            else if (m_Camera.RotationX > Global.GAME_CAMLIMIT && m_Camera.RotationX <= Math.PI) m_Camera.RotationX =  Global.GAME_CAMLIMIT;

			//Jump with space);
			if (InputManager.Keyboard.KeyDown(Keys.Space) && !m_Jumping) {
                //Set animation
				m_Model.CurrentAnimation = "Jumping";

                //Start jumping
				m_Model.YVelocity   = Global.ACTOR_JUMPING;
				m_Jumping           = true;
			}
		}

		public void OnCollision(Ship ship) {
            //Determines should actor keep falling
		    bool Falling = ship.GetBoundingBox().Max.Y > (GetBoundingBox().Min.Y + GetBoundingBox().Max.Y)/2;

            //If no longer falling
            if (!Falling) {
				//Stop jumping
				m_Jumping                   = false;
				m_Model.YVelocity           = 0.0f;
				m_Model.CurrentAnimation    = "Walking";

                //Set last ship visited
			    m_LastShip = ship;

				//Place it on the ship
				m_Model.Y = ship.GetBoundingBox().Max.Y;
            }

		}

	    public void SetPosition(float f, float f1, float f2) {
	        m_Model.Position		= new Vector3(f, f1, f2);
			m_Model.Velocity		= Vector3.Zero;
			m_Model.Acceleration	= Vector3.Zero;
	    }

	    public int GetLife() {
            return m_Life;
	    }

		public void SetLife(int life) {
			m_Life = life;
		}
	}
}
