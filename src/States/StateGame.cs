
//Namespaces used
using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Lighting;
using Klotski.Utilities;
using Klotski.Components;
using Klotski.States.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Mouse = FlatRedBall.Input.Mouse;

//Class namespace
namespace Klotski.States {
	/// <summary>
	/// The state that handles main gameplay.
	/// </summary>
	public class StateGame : State {
		//Enumeration
		public enum Player {
			Klotski,	//Human player
			DFS,		//AI using DFS method
			BFS,		//AI using BFS method
			Storm		//Map creation.
		}

		//Sky component
		private Sky		m_Sky;
		private Layer	m_SkyLayer;

		//Cameras
		private Camera m_ActiveCamera;
		private Camera m_BirdsView;

		//Playable components
		private Actor				m_Actor;
		private List<Ship>			m_Ships;
		private Playable			m_Controlled;
		private readonly GameData	m_Data;

		//Logics
		private Player	m_Player;
		private Ship[,] m_Board;
		private bool	m_ReadyToUpdate;

 
		/// <summary>
		/// State Game class constructor.
		/// </summary>
		public StateGame(Player player, GameData data) : base(StateID.Game) {
			//Initialize variable
			m_Data			= data;
			m_Player		= player;
			m_ReadyToUpdate	= false;

			//Empties variables
			m_Sky			= null;
			m_Actor			= null;
			m_Ships			= null;
			m_Board			= null;
			m_SkyLayer		= null;
			m_BirdsView		= null;
			m_ActiveCamera	= null;
			m_Controlled	= null;
		}

		/// <summary>
		/// Initialize the game.
		/// </summary>
		/// <note>DO NOT CHANGE INITIALIZATION ORDER</note>
		public override void Initialize() {
			//Initialize environment
			CreateSky();
			CreateLighting();

			//Load ships
			m_Ships = m_Data.LoadShipList(m_Layer);
			foreach (Ship ship in m_Ships) ship.Initialize();

			//Create the board
			CreateBoard();
			UpdateBoard();

			//Create camera
			CreateBirdsView();

			//Create and initialize actor
			m_Actor			= new Actor(m_Layer);
			m_Controlled	= m_Actor;
			m_ActiveCamera	= m_Controlled.GetCamera();
			m_Actor.Initialize(10, -10, 10);
		}

		/// <summary>
		/// What happened upon entering the state.
		/// </summary>
		public override void OnEnter() { }

		/// <summary>
		/// What happens when the state got removed.
		/// </summary>
		public override void OnExit() {
			base.OnExit();

			//Reset camera
			SpriteManager.Camera.X = Global.APPCAM_DEFAULTX;
			SpriteManager.Camera.Y = Global.APPCAM_DEFAULTY;
			SpriteManager.Camera.Z = Global.APPCAM_DEFAULTZ;
			SpriteManager.Camera.RotationX = Global.APPCAM_DEFAULTROTX;
			SpriteManager.Camera.RotationY = Global.APPCAM_DEFAULTROTY;
			SpriteManager.Camera.RotationZ = Global.APPCAM_DEFAULTROTZ;

			//Remove sky
			SpriteManager.RemoveDrawableBatch(m_Sky);
			SpriteManager.RemoveLayer(m_SkyLayer);
		}

		//Initialization procedures
		#region Initialize Stuff
		/// <summary>
		/// Create array of board from the list of ships.
		/// </summary>
		private void CreateBoard() {
			//Get board size
			int MaxRow		= 1;
			int MaxColumn	= 1;
			foreach (Ship ship in m_Ships) {
				MaxRow = Math.Max(MaxRow, ship.GetRow() + ship.GetHeight());
				MaxColumn = Math.Max(MaxColumn, ship.GetColumn() + ship.GetWidth());
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

		/// <summary>
		/// Creates a skysphere
		/// </summary>
		private void CreateSky() {
			//Add a sky layer under main layer
			Layer Temp	= SpriteManager.AddLayer();
			m_SkyLayer = m_Layer;
			m_Layer = Temp;

			//Create sky
			m_Sky = new Sky(Global.TEXTURE_FOLDER + "Sky");
			SpriteManager.AddDrawableBatch(m_Sky);
			SpriteManager.AddToLayer(m_Sky, m_SkyLayer);
		}

		/// <summary>
		/// Configures global lighting.
		/// </summary>
		private void CreateLighting() {
			//Turn on lighting
			Renderer.LightingEnabled = true;

			//Set lighting used
			Renderer.Lights.SetDefaultLighting(LightCollection.DefaultLightingSetup.Daylight);
			Renderer.Lights.DirectionalLights[0].Direction = new Vector3(0.2f, -0.6f, -0.8f);
		}

		/// <summary>
		/// Create bird's view camera
		/// </summary>
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

			//Logging
			if (Global.Logger != null) Global.Logger.AddLine("Bird's view camera created.");

		}
		#endregion

		/// <summary>
		/// Set state visibility.
		/// </summary>
		/// <param name="visibility">State's new visibility.</param>
		public override void SetVisibility(bool visibility) {
			base.SetVisibility(visibility);

			//Set sky visibility
			m_SkyLayer.Visible = visibility;
		}

		/// <summary>
		/// Switch the camera between controlled and birdview.
		/// </summary>
		private void SwitchCamera() {
			//Switch between birdview and controlled camera
			if (m_ActiveCamera == m_BirdsView)	m_ActiveCamera = m_Controlled.GetCamera();
			else								m_ActiveCamera = m_BirdsView;
		}

		/// <summary>
		/// Find any ship below the actor.
		/// </summary>
		/// <returns>Ship below the actor, if there's none, return null.</returns>
		private Ship GetShipBelowActor() {
			//Set default value
			Ship Below = null;

			//Calculates position
			float X = (m_Actor.GetBoundingBox().Min.X + m_Actor.GetBoundingBox().Max.X) / 2.0f;
			float Y =  m_Actor.GetBoundingBox().Min.Y;
			float Z = (m_Actor.GetBoundingBox().Min.Z + m_Actor.GetBoundingBox().Max.Z) / 2.0f;
			Vector3 Position = new Vector3(X, Y - 1.0f, Z);

			//Find the ship in ship list
			foreach (Ship ship in m_Ships)
				if (ship.GetBoundingBox().Contains(Position) == ContainmentType.Contains) Below = ship;

			//Returns the ship below
			return Below;
		}

		/// <summary>
		/// Updates the game each frame.
		/// </summary>
		/// <param name="time">Game time's data</param>
		public override void Update(GameTime time) {
			//Check actor collision
			foreach (Ship ship in m_Ships) 
				if (ship.GetBoundingBox().Intersects(m_Actor.GetBoundingBox())) m_Actor.OnCollision(ship);

			//Check for input
			#region Input checking
			if (InputManager.Keyboard.KeyPushed(Keys.Tab))		SwitchCamera();
			if (InputManager.Keyboard.KeyPushed(Keys.Escape))	Global.StateManager.GoTo(StateID.Pause, null);

			//If mouse is clicked
			#region Change control
			if (InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton)) {
				//If block is not moving and no in bird's view
				if (!m_ReadyToUpdate && m_ActiveCamera != m_BirdsView) {
					//If currently controlling a ship
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
			}
			#endregion
			#endregion

			//Updating
			#region Entity updating
			//If camera is not bird view
            if (m_ActiveCamera != m_BirdsView) {
                //Set active camera
                m_ActiveCamera = m_Controlled.GetCamera();

                //Update controlled entity only
                m_Controlled.UpdateControl(time);
                m_Controlled.Update(time);
			}
			#endregion
			UpdateCamera();
			#region Board updating
			//If currently controlling a ship
			if (m_Controlled is Ship) {
				//If moving, prepare for board update
				if ((m_Controlled as Ship).IsMoving()) m_ReadyToUpdate = true;
				else if (m_ReadyToUpdate) {
					//Update board
					CreateBoard();
					UpdateBoard();

					//No longer need to update board
					m_ReadyToUpdate = false;
				}
			}
			#endregion
		}
		
		/// <summary>
		/// Updates the board and check next movement.
		/// </summary>
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

		/// <summary>
		/// Updates drawing camera to follow active camera.
		/// </summary>
		private void UpdateCamera() {
			//Copy position and orentation
			SpriteManager.Camera.Position		= m_ActiveCamera.Position;
			SpriteManager.Camera.RotationMatrix = m_ActiveCamera.RotationMatrix;
		}
	}
}
