
//Namespaces used
using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Lighting;
using Klotski.Utilities;
using Klotski.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Mouse = FlatRedBall.Input.Mouse;

//Class namespace
namespace Klotski.States {
	/// <summary>
	/// Class description.
	/// </summary>
	public class StateGame : State {
		//Members
		private Sky			m_Sky;
		private Actor		m_Actor;
		private List<Ship>	m_Ships;
		private Layer		m_SkyLayer;

		private Playable m_Controlled;

		//
		private Ship[,] m_Board;

		//Cameras
		private Camera m_ActiveCamera;
		private Camera m_BirdsView;

		private bool m_ReadyToUpdate;
 
		/// <summary>
		/// Class constructor.
		/// </summary>
		public StateGame() : base(StateID.Game) {
			//Empties variables
			m_Sky = null;
			m_Actor = null;
			m_Ships = new List<Ship>();
			m_BirdsView = null;
			m_ActiveCamera = null;
			m_Board = null;
			m_Controlled = null;
			m_ReadyToUpdate = false;
		}

		/// <summary>
		/// Initialize the game
		/// </summary>
		public override void Initialize() {
			//Set lighting
			Renderer.LightingEnabled = true;
			Renderer.Lights.SetDefaultLighting(LightCollection.DefaultLightingSetup.Evening);
			Renderer.Lights.DirectionalLights[0].Direction = new Vector3(0.2f, -0.6f, -0.8f);

			//
			Layer Temp	= SpriteManager.AddLayer();
			m_SkyLayer	= m_Layer;
			m_Layer		= Temp;

			//Create sky
			m_Sky = new Sky(Global.TEXTURE_FOLDER + "Sky");
			SpriteManager.AddDrawableBatch(m_Sky);
			SpriteManager.AddToLayer(m_Sky, m_SkyLayer);

			//Create ships
			Ship S = new Ship(m_Layer, 0, 0, 1, 1);
			m_Ships.Add(S);
			S = new Ship(m_Layer, 1, 0, 1, 2);
			m_Ships.Add(S);
			S = new Ship(m_Layer, 1, 1, 2, 2);
			m_Ships.Add(S);
			S = new Ship(m_Layer, 0, 1, 2, 1);
			m_Ships.Add(S);
			S = new Ship(m_Layer, 1, 3, 1, 1);
			m_Ships.Add(S);
			S = new Ship(m_Layer, 3, 0, 2, 1);
			m_Ships.Add(S);

			//Initialize them
			foreach (Ship ship in m_Ships) ship.Initialize();

			//Create the board
			CreateBoard();
		    UpdateBoard();
			CreateBirdsView();

			m_Actor = new Actor(m_Layer);
			m_Actor.Initialize(10, -10, 10);
			m_Controlled = m_Actor;
			m_ActiveCamera = m_Controlled.GetCamera();
		}

		private void CreateLighting() {
			//
		}

		private void CreateBirdsView() {
			//Instantiate a camera
			m_BirdsView = new Camera(FlatRedBallServices.GlobalContentManager);

			//Calculate center of the board
			int		Column	= m_Board.GetLength(0);
			int		Row		= m_Board.GetLength(1);
			float	CenterX = Column * (Global.GAMETILE_WIDTH  + (Global.GAMEGAP_WIDTH * 2))  / 2.0f;
			float	CenterZ = Row	 * (Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2)) / 2.0f;

			//Set position
			m_BirdsView.X = CenterX;
			m_BirdsView.Y = Global.GAME_CAMERAHEIGHT;
			m_BirdsView.Z = CenterZ;
			
			//Set rotation
			m_BirdsView.RotationX = (float)(Math.PI * 1.5f);
			m_BirdsView.RotationY = 0.0f;
			m_BirdsView.RotationZ = 0.0f;

		}

		private void SwitchCamera() {
			//Switch between birdview and controlled camera
			if (m_ActiveCamera == m_BirdsView)	m_ActiveCamera = m_Controlled.GetCamera();
			else								m_ActiveCamera = m_BirdsView;
		}

		/// <summary>
		/// Updates drawing camera to follow active camera.
		/// </summary>
		private void UpdateCamera() {
            //

			//Copy position and orentation
			SpriteManager.Camera.Position		= m_ActiveCamera.Position;
			SpriteManager.Camera.RotationMatrix = m_ActiveCamera.RotationMatrix;
		}

		public override void OnEnter() {}

		/// <summary>
		/// Create array of board from the list of ships.
		/// </summary>
		private void CreateBoard() {
			//Get board size
			int MaxRow		= 1;
			int MaxColumn	= 1;
			foreach (Ship ship in m_Ships) {
				MaxRow		= Math.Max(MaxRow, ship.GetRow() + ship.GetHeight());
				MaxColumn	= Math.Max(MaxColumn, ship.GetColumn() + ship.GetWidth());
			}

			//Create board
			m_Board = new Ship[MaxColumn, MaxRow];
			for (int x = 0; x < MaxColumn; x++) for (int y = 0; y < MaxRow; y++) m_Board[x, y] = null;

			//For each ship
			foreach (Ship ship in m_Ships) {
				//Get variable
				bool Empty	= true;
				int X		= ship.GetColumn();
				int Y		= ship.GetRow();

				//Ensure ship's space is empty
				for (int x = X; x < X + ship.GetWidth(); x++) for (int y = Y; y < Y + ship.GetHeight(); y++)
					if (m_Board[x, y] != null) Empty = false;

				//Place the ship if empty
				if (Empty) {
					for (int x = X; x < X + ship.GetWidth(); x++) for (int y = Y; y < Y + ship.GetHeight(); y++)
						m_Board[x, y] = ship;
				}
			}
		}

        private void UpdateBoard() {
			//Skip if board doesn't exist
			if (m_Board == null) return;

            //For each ship
            foreach (Ship ship in m_Ships) {
                //Reset movement
                ship.ResetMovement();

				//Check each direction
				#region Movement availability checking
				//Is ship on top row?
                bool Available = (ship.GetRow() + ship.GetHeight()) < m_Board.GetLength(1);

                //Check against other ship if not on top row
                if (Available) {
					for (int x = ship.GetColumn(); x < ship.GetColumn() + ship.GetWidth(); x++)
						if (m_Board[x, ship.GetRow() + ship.GetHeight()] != null) Available = false;
                	if (Available) { ship.AddMovement(Ship.Direction.PositiveY); }
				}
				
				//Is ship on bottom row?
				Available = ship.GetRow() > 0;

				//Check against other ship if not on bottom row
				if (Available) {
					for (int x = ship.GetColumn(); x < ship.GetColumn() + ship.GetWidth(); x++)
						if (m_Board[x, ship.GetRow() - 1] != null) Available = false;
					if (Available) { ship.AddMovement(Ship.Direction.NegativeY); }
				}

				//Is ship on left most row?
				Available = (ship.GetColumn() + ship.GetWidth()) < m_Board.GetLength(0);

				//Check against other ship if not on bottom row
				if (Available) {
					for (int y = ship.GetRow(); y < ship.GetRow() + ship.GetHeight(); y++)
						if (m_Board[ship.GetColumn() + ship.GetWidth(), y] != null) Available = false;
					if (Available) { ship.AddMovement(Ship.Direction.PositiveX); }
				}

				//Is ship on right most row?
				Available = ship.GetColumn() > 0;

				//Check against other ship if not on bottom row
				if (Available) {
					for (int y = ship.GetRow(); y < ship.GetRow() + ship.GetHeight(); y++)
						if (m_Board[ship.GetColumn() - 1, y] != null) Available = false;
					if (Available) { ship.AddMovement(Ship.Direction.NegativeX); }
				}
				#endregion
			}
        }

		private Ship GetShipBelowActor() {
			//Set default value
			Ship Below = null;

			//Calculates actor position
			float X = (m_Actor.GetBoundingBox().Min.X + m_Actor.GetBoundingBox().Max.X) / 2.0f;
			float Y = m_Actor.GetBoundingBox().Min.Y;
			float Z = (m_Actor.GetBoundingBox().Min.Z + m_Actor.GetBoundingBox().Max.Z) / 2.0f;
			Vector3 Position = new Vector3(X, Y, Z);
			Position.Z -= 1.0f;
				
			foreach (Ship ship in m_Ships)
				if (ship.GetBoundingBox().Contains(Position) == ContainmentType.Contains) Below = ship;

			return Below;
		}

		public override void Update(GameTime time) {
			if (InputManager.Keyboard.KeyPushed(Keys.Escape)) Global.StateManager.GoTo(StateID.Pause, null);
			if (InputManager.Keyboard.KeyPushed(Keys.LeftShift)) SwitchCamera();

			//
			foreach (Ship ship in m_Ships) {
				//If intersecting
				if (ship.GetBoundingBox().Intersects(m_Actor.GetBoundingBox())) m_Actor.OnCollision(ship);
			}

			//If controlled ship is moving
			if (m_Controlled is Ship) {
				if ((m_Controlled as Ship).IsMoving()) m_ReadyToUpdate = true;
				else if (m_ReadyToUpdate) {
					//Update board
					CreateBoard();
					UpdateBoard();

					//
					m_ReadyToUpdate = false;
				}
			}

			//If mouse is clicked
			if (InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton) && !m_ReadyToUpdate) {
				//Check
				if (m_Controlled != m_Actor) {
					//Place actor on top of the ship
					m_Actor.SetPosition(
						(m_Controlled.GetBoundingBox().Max.X + m_Controlled.GetBoundingBox().Min.X) / 2.0f,
						m_Controlled.GetBoundingBox().Max.Y,
						(m_Controlled.GetBoundingBox().Max.Z + m_Controlled.GetBoundingBox().Min.Z) / 2.0f);

					//Return control to actor
					m_Actor.SetVisibility(true);
					m_Controlled = m_Actor;
				} else {
					//Get the ship below actor
					Ship Below = GetShipBelowActor();

					//If a ship exist
					if (Below != null) {
						//Set as controlled
						m_Controlled = Below;

						//Turn off actor
						m_Actor.SetVisibility(false);
					}
				}
			}

            //If camera is not bird view
            if (m_ActiveCamera != m_BirdsView) {
                //Set active camera
                m_ActiveCamera = m_Controlled.GetCamera();

                //Update controlled
                m_Controlled.UpdateControl(time);
                m_Controlled.Update(time);
            }
			UpdateCamera();
		}

		public override void SetVisibility(bool visibility) {
			base.SetVisibility(visibility);

			//Set sky visibility
			m_SkyLayer.Visible = visibility;
		}

        public override void OnExit() {
            base.OnExit();

            //Reset camera
            SpriteManager.Camera.X			= 0;
            SpriteManager.Camera.Y			= 0;
            SpriteManager.Camera.Z			= 40;
            SpriteManager.Camera.RotationX  = 0;
            SpriteManager.Camera.RotationY  = 0;
            SpriteManager.Camera.RotationZ  = 0;

			//Remove sky
			SpriteManager.RemoveDrawableBatch(m_Sky);
			SpriteManager.RemoveLayer(m_SkyLayer);
        }
	}
}
