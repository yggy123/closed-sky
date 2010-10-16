
//Namespaces used
using System;
using FlatRedBall;
using FlatRedBall.Graphics.Model;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Lighting;
using Klotski.Utilities;
using Klotski.Components;
using Klotski.States.Game;
using Klotski.Controls.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Console = System.Console;
using Mouse = FlatRedBall.Input.Mouse;
using Renderer = FlatRedBall.Graphics.Renderer;

//Class namespace
namespace Klotski.States {
    // ReSharper disable InconsistentNaming
    //Enumeration
    public enum Player {
        Klotski,	//Human player
        DFS,		//AI using DFS method
        BFS,		//AI using BFS method
        Storm		//Map creation.
    }

    // ReSharper restore InconsistentNaming
	/// <summary>
	/// The state that handles main gameplay.
	/// </summary>
    public class StateGame : State {
		//Sky component
		private Sky		m_Sky;
		private Layer	m_SkyLayer;

		//Cameras
		private Camera m_ActiveCamera;
		private Camera m_BirdsView;

		//Game components
		private Actor				m_Actor;
		private Ship				m_King;
		private List<Ship>			m_Ships;
		private Playable			m_Controlled;
		private PositionedModel		m_Arrow;
		private readonly GameData	m_Data;

		//GUI
		private Timer	m_Timer;
		private LifeBar m_LifeBar;
		private Counter m_Counter;

		//Logics
		private Player	m_Player;
		private Ship[,] m_Board;
        private int     m_BoardWidth;
        private int     m_BoardHeight;
		private bool	m_Victory;
		private bool	m_Initialized;
		private bool	m_ReadyToUpdate;
		private float	m_VictoryWait;

		//AI
		private int				m_Depth;
		private int				m_MaxDepth;
		private Stack<Ship>		m_ShipStack;
		private Stack<int>		m_IteratorStack;
		private List<GameData>	m_VisitedList;

		/// <summary>
		/// State Game class constructor.
		/// </summary>
		public StateGame(Player player, GameData data) : base(StateID.Game) {
			//Initialize variable
			m_Data			= data;
			m_Player		= player;
			m_Victory		= false;
			m_Initialized	= false;
			m_ReadyToUpdate	= false;
            m_BoardWidth    = 0;
            m_BoardHeight   = 0;
			m_IteratorStack	= new Stack<int>();
			m_ShipStack		= new Stack<Ship>();
			m_VisitedList	= new List<GameData>();

			//Empties variables
			m_Depth			= 0;
			m_MaxDepth		= 0;
			m_Sky			= null;
			m_Actor			= null;
			m_King			= null;
			m_Ships			= null;
			m_Board			= null;
			m_SkyLayer		= null;
			m_BirdsView		= null;
			m_ActiveCamera	= null;
			m_Controlled	= null;
			m_LifeBar		= null;
			m_Timer			= null;
		}

		/// <summary>
		/// Initialize the game.
		/// </summary>
		/// <note>DO NOT CHANGE INITIALIZATION ORDER</note>
		public override void Initialize() {
			//Initialize environment
			if (!m_Initialized) CreateSky();
			if (!m_Initialized) CreateGUI();
			if (!m_Initialized) CreateArrow();
			if (!m_Initialized) CreateLighting();

			//If has been initialized, remove old ships
            if (m_Initialized) foreach (Ship ship in m_Ships) ship.Remove();

			//Load ships
			m_Ships = m_Data.LoadShipList(m_Layer);
			foreach (Ship ship in m_Ships) ship.Initialize();

			//Create the board
			CreateBoard();
			UpdateBoard();

			//Create camera
            if (!m_Initialized) CreateBirdsView();

			//Create and initialize actor if player
            if (m_Player == Player.Klotski || m_Player == Player.Storm) {
                if (m_Initialized) m_Actor.Remove();
                m_Actor = new Actor(m_Layer);
                m_Controlled = m_Actor;
                m_ActiveCamera = m_Controlled.GetCamera();
                m_Actor.Initialize(m_Ships[0], 3);
				Renderer.Lights.Add(m_Actor.GetLight());
            }
            else m_ActiveCamera = m_BirdsView;

			//Initialize AI
			m_Depth			= 0;
			m_MaxDepth		= 1;
			m_VictoryWait	= 0;
			m_ShipStack.Clear();
			m_IteratorStack.Clear();

			//Add current state
			m_VisitedList.Clear();
			GameData Root = new GameData();
			foreach (Ship ship in m_Ships) Root.AddShip(ship);
			m_VisitedList.Add(Root);

			//Start BGM
			Global.SoundManager.PlayBGM(Global.GAME_BGM);

			//Game is initialized
			m_Initialized = true;
		}

		/// <summary>
		/// What happened upon entering the state.
		/// </summary>
		public override void OnEnter() {
            //Restore actor's state
            if (m_Actor != null) m_Actor.Restore();
        }

		/// <summary>
		/// What happens when the state got removed.
		/// </summary>
		public override void OnExit() {
			base.OnExit();

			//Reset camera
			SpriteManager.Camera.X			= Global.APPCAM_DEFAULTX;
			SpriteManager.Camera.Y			= Global.APPCAM_DEFAULTY;
			SpriteManager.Camera.Z			= Global.APPCAM_DEFAULTZ;
			SpriteManager.Camera.RotationX	= Global.APPCAM_DEFAULTROTX;
			SpriteManager.Camera.RotationY	= Global.APPCAM_DEFAULTROTY;
			SpriteManager.Camera.RotationZ	= Global.APPCAM_DEFAULTROTZ;

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
            //If board size is 0
            if (m_BoardWidth <= 0 && m_BoardHeight <= 0) {
			    //Get board size
			    int MaxRow		= 1;
			    int MaxColumn	= 1;
			    foreach (Ship ship in m_Ships) {
				    MaxRow = Math.Max(MaxRow, ship.GetRow() + ship.GetHeight());
				    MaxColumn = Math.Max(MaxColumn, ship.GetColumn() + ship.GetWidth());
			    }

                //Set board size
                m_BoardWidth    = MaxColumn;
                m_BoardHeight   = MaxRow;
            }

			//Create board
            m_Board = new Ship[m_BoardWidth, m_BoardHeight];
            for (int x = 0; x < m_BoardWidth; x++) for (int y = 0; y < m_BoardHeight; y++) m_Board[x, y] = null;

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
			if (m_Player == Player.Storm)	m_Sky = new Sky(Global.EDITOR_SKY);
			else							m_Sky = new Sky(Global.GAME_SKY);
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
			Renderer.Lights.AmbientLights[0].SetColor(new Color(0.1f, 0.1f, 0.1f));
			Renderer.Lights.DirectionalLights[0].Direction = Vector3.Normalize(new Vector3(0.0f, -0.4f, -0.8f));
			Renderer.Lights.DirectionalLights[0].SetColor(new Color(0.45f, 0.35f, 0.2f));
		}

		/// <summary>
		/// Create goal arrow.
		/// </summary>
		private void CreateArrow() {
			//Create arrow model
			m_Arrow = ModelManager.AddModel(Global.MODEL_FOLDER + "Arrow", FlatRedBallServices.GlobalContentManager, true);
			m_Arrow.CurrentAnimation = "Default";
			ModelManager.AddToLayer(m_Arrow, m_Layer);

			//Move arrow to goal
			m_Arrow.X = (Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2)) * (m_Data.Goal + 1.0f);
			m_Arrow.Y = -50.0f;
			m_Arrow.Z = 0.0f;
		}

		/// <summary>
		/// Create game's graphical interface.
		/// </summary>
		private void CreateGUI() {
            //If player
            if (m_Player == Player.Klotski || m_Player == Player.Storm) {
			    //Create lifebar
			    m_LifeBar		= new LifeBar(Global.GUIManager);
			    m_LifeBar.Left	= Global.LIFEBAR_X;
			    m_LifeBar.Top	= Global.LIFEBAR_Y;
			    m_LifeBar.Init();

			    //Add it
			    m_Panel.Add(m_LifeBar);
			    Global.GUIManager.Add(m_LifeBar);
            }

			//If not editor
			if (m_Player != Player.Storm) {
				//Create timer
				m_Timer			= new Timer(Global.GUIManager);
				m_Timer.Left	= Global.TIMER_X;
				m_Timer.Top		= Global.TIMER_Y;
				m_Timer.Init(Global.TIMER_FONT);

				//Create steps counter
				m_Counter		= new Counter(Global.GUIManager);
				m_Counter.Left	= Global.COUNTER_X;
				m_Counter.Top	= Global.COUNTER_Y;
				m_Counter.Init(Global.COUNTER_FONT, m_Player != Player.Klotski);

				//Add it
				m_Panel.Add(m_Timer);
				m_Panel.Add(m_Counter);
				Global.GUIManager.Add(m_Timer);
				Global.GUIManager.Add(m_Counter);
				
			}
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
			m_BirdsView.RotationY = (float) Math.PI;
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
			//If winning
			if (m_Victory) {
				//Waits 2 seconds
				m_VictoryWait += time.ElapsedGameTime.Milliseconds / 1000.0f;
				if (m_VictoryWait > 2.0f) {
					//Create parameter
					object[] Parameters = new object[4];
					Parameters[1] = m_Timer.GetTime();
					if (m_Player == Player.Klotski) Parameters[0] = 0;
					else							Parameters[0] = 1;

					int Movement = 0;
					foreach (Ship ship in m_Ships) Movement += ship.GetMoveNumber();
					Parameters[2] = Movement;
					Parameters[3] = 0;

					//If not player
					if (m_Player != Player.Klotski) Parameters[3] = m_VisitedList.Count;

					//Go to gameover state
					Global.StateManager.GoTo(StateID.GameOver, Parameters, true);
				}
			}
			//Otherwise, do the usual update
			else {

				//Updates GUI if not editor
				if (m_Player != Player.Storm) {
					int MoveNumber = 0;
					foreach (Ship ship in m_Ships) MoveNumber += ship.GetMoveNumber();
					m_Counter.SetCounter(MoveNumber, m_VisitedList.Count);
					m_Timer.Increase(time.ElapsedGameTime);
				} else m_Actor.SetLife(3);

				//if player controlled);
                if (m_Player == Player.Klotski || m_Player == Player.Storm) {
				    //Check actor collision);
				    foreach (Ship ship in m_Ships) 
					    if (ship.GetBoundingBox().Intersects(m_Actor.GetBoundingBox())) m_Actor.OnCollision(ship);
                } else {
                	//Check speed change
					if (InputManager.Keyboard.KeyPushed(Keys.Q)) Ship.DecreaseSpeed();
					if (InputManager.Keyboard.KeyPushed(Keys.E)) Ship.IncreaseSpeed();
                }
                //Updates based on the player
                switch (m_Player) {
				#region Player controlled
				case Player.Storm:
				case Player.Klotski:
					//Camera switching
					if (InputManager.Keyboard.KeyPushed(Keys.Tab)) SwitchCamera();

					//Controlled character switching
					if (InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton)) {
						//If block is not moving and not in bird's view
						if (!m_ReadyToUpdate && m_ActiveCamera != m_BirdsView) {
							//If currently controlling a ship
							if (m_Controlled != m_Actor) {
								//Place actor on top of the ship
								m_Actor.SetPosition(
									(m_Controlled.GetBoundingBox().Max.X + m_Controlled.GetBoundingBox().Min.X) / 2.0f,
									m_Controlled.GetBoundingBox().Max.Y + 5.0f,
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
					break; 
				#endregion
				#region AI controlled BFS
				case Player.BFS:
					//Create iterator if not exist
					if (m_IteratorStack.Count <= 0) m_IteratorStack.Push((int)Direction.NegativeY);

					//If
					if (m_Controlled == null || !((Ship)m_Controlled).IsMoving()) {
						//Get stack info
						int i = (m_IteratorStack.Peek() - (int)Direction.NegativeY) / 5;
						int j =  m_IteratorStack.Peek() - (i * 5);
						m_IteratorStack.Pop();

						//While there's no moving block and iterator hasn't finished
						while (m_Controlled == null && (i * 5) + j < (m_Ships.Count * 5) + (int)Direction.NegativeY) {
							while (m_Controlled == null && j <= (int)Direction.PositiveY) {
								//If the ship contains a movement
								if (m_Ships[i].GetBackDirection() != (Direction)j) {
									if (m_Ships[i].GetAvailableMovement().Contains((Direction)j)) {
										//Move and Set as active
										m_Controlled = m_Ships[i];
										m_ShipStack.Push(m_Ships[i]);
										m_Ships[i].Move((Direction)j);
										m_Depth++;

										//Save stack
										j++;
										if (j > (int)Direction.PositiveY) {
											j = (int)Direction.NegativeY;
											i++;
										}
										m_IteratorStack.Push((i * 5) + j);
									}
								}

								//Next direction
								j++;
							}

							//Reset j
							j = (int)Direction.NegativeY;

							//Next
							i++;
						}

						//If has iterated through all
						if (m_Controlled == null && i >= m_Ships.Count) {
							//Increase max depth if at root
							if (m_Depth == 0) m_MaxDepth++;
							else {
								//Return via last ship
								m_Controlled = m_ShipStack.Pop();
								((Ship)m_Controlled).Return();
								m_Depth--;
							}
						}
					}

					//If moving ship exist
					if (m_Controlled != null) {
						//Updates
						m_Controlled.Update(time);

						//If not moving
						if (!((Ship)m_Controlled).IsMoving()) {
							//If maximum depth
							if (m_Depth >= m_MaxDepth) {
								//Find current state in list
								bool		Exist = false;
								GameData	CurrentState = new GameData();
								foreach (Ship ship in m_Ships) CurrentState.AddShip(ship);
								foreach (GameData state in m_VisitedList) {
									if (state.Equals(CurrentState)) Exist = true;
								}
								
								//If not exist
								if (!Exist) m_VisitedList.Add(CurrentState);

								((Ship)m_Controlled).Return();
								m_ShipStack.Pop();
								m_Depth--;
							} else {
								//If returning, no longer returning, otherwise, add a stack
								if (((Ship)m_Controlled).Returning) ((Ship)m_Controlled).Returning = false;
								else m_IteratorStack.Push((int)Direction.NegativeY);

								//Find a new ship
								m_Controlled = null;
							}

							//Update board
							CreateBoard();
							UpdateBoard();
						}
					}
					break; 
				#endregion
				#region AI controlled DFS
				case Player.DFS:
					//Create iterator if not exist
					if (m_IteratorStack.Count <= 0) m_IteratorStack.Push((int)Direction.NegativeY);

					if (m_Controlled == null || !((Ship)m_Controlled).IsMoving()) {
						//Get stack info
						int x = (m_IteratorStack.Peek() - (int) Direction.NegativeY)/5;
						int y = m_IteratorStack.Peek() - (x*5);
						m_IteratorStack.Pop();

						//While there's no moving block and iterator hasn't finished
						while (m_Controlled == null && (x*5) + y < (m_Ships.Count*5) + (int) Direction.NegativeY) {
							while (m_Controlled == null && y <= (int) Direction.PositiveY) {
								//If the ship contains a movement
								if (m_Ships[x].GetBackDirection() != (Direction) y) {
									if (m_Ships[x].GetAvailableMovement().Contains((Direction) y)) {
										//Move and Set as active
										m_Controlled = m_Ships[x];
										m_ShipStack.Push(m_Ships[x]);
										m_Ships[x].Move((Direction) y);
										Console.WriteLine(m_Ships[x].GetBackDirection());

										//Save stack
										y++;
										if (y > (int) Direction.PositiveY) {
											y = (int) Direction.NegativeY;
											x++;
										}
										m_IteratorStack.Push((x*5) + y);
									}
								}

								//Next direction
								y++;
							}

							//Reset y
							y = (int) Direction.NegativeY;

							//Next
							x++;
						}

						//If has iterated through all
						if (m_Controlled == null && x >= m_Ships.Count) {
							//Return via last ship
							m_Controlled = m_ShipStack.Pop();
							((Ship)m_Controlled).Return();
						}
					}

                	//If moving ship exist
					if (m_Controlled != null) {
						//Updates
						m_Controlled.Update(time);

						//If not moving
						if (!((Ship)m_Controlled).IsMoving()) {
							//Update board
							CreateBoard();
							UpdateBoard();

							//If returning, no longer returning, otherwise, add a stack
							if (((Ship)m_Controlled).Returning) {
								((Ship)m_Controlled).Returning = false;

								//Find a new ship
								m_Controlled = null;
							}
							else {
								//Find current state in list
								bool		Exist = false;
								GameData	CurrentState = new GameData();
								foreach (Ship ship in m_Ships) CurrentState.AddShip(ship);
								foreach (GameData state in m_VisitedList) {
									if (state.Equals(CurrentState)) Exist = true;
								}
								
								//If exist
								if (Exist) {
									((Ship)m_Controlled).Return();
									m_ShipStack.Pop();
								}
								else {
									m_VisitedList.Add(CurrentState);

									m_IteratorStack.Push((int)Direction.NegativeY);

									//Find a new ship
									m_Controlled = null;
								}
							}
						}
					}

                	break; 
				#endregion
                }

				//Updating
				#region Entity updating
				//If camera is not bird view
				if (m_ActiveCamera != m_BirdsView && (m_Player == Player.Klotski || m_Player == Player.Storm)) {
					//Ensure controlled entity exist
					if (m_Controlled != null) {
						//Set active camera
						m_ActiveCamera = m_Controlled.GetCamera();

						//Update controlled entity only
						m_Controlled.UpdateControl(time);
						m_Controlled.Update(time);
					}

					//Update life bar
					m_LifeBar.SetLife(m_Actor.GetLife());
					m_LifeBar.Invalidate();

                    //If actor dies
					if (m_Actor.GetLife() <= 0) {
						//Create parameter
						object[] Parameters = new object[4];

						Parameters[0] = -1;
						Parameters[1] = new TimeSpan();
						Parameters[2] = 0;
						Parameters[3] = 0;

						//Go to gameover state
						Global.StateManager.GoTo(StateID.GameOver, Parameters, true);
                    	
                    }
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

                //if escape is pressed
                if (InputManager.Keyboard.KeyPushed(Keys.Escape)) {
                    //Save actor's data if exist
                    if (m_Actor != null) m_Actor.SaveState();
                    
                    //Go to pause state
					object[] Parameters = new object[1];
                	Parameters[0] = false;
					if (m_Player == Player.Storm) Parameters[0] = true;
                    Global.StateManager.GoTo(StateID.Pause, Parameters);
                }
			}
		}
		
		/// <summary>
		/// Updates the board and check next movement.
		/// </summary>
		private void UpdateBoard() {
			//Skip if board doesn't exist
			if (m_Board == null) return;

			//Checks winning condition if not editor
            if (m_Player != Player.Storm && m_Board[m_Data.Goal, 0] != null && m_Board[m_Data.Goal + 1, 0] != null)
				if (m_Board[m_Data.Goal, 0].IsKing() && m_Board[m_Data.Goal + 1, 0].IsKing()) {
					//Wins
					m_Victory	= true;
					m_King		= m_Board[m_Data.Goal, 0];
					m_King.Wins();

					//Set camera as birdview
					m_ActiveCamera = m_BirdsView;
					UpdateCamera();

					//No need to check the rest
					return;
				}

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
					if (Available) { ship.AddMovement(Direction.PositiveY); }
				}

				//Is ship on bottom row?
				Available = ship.GetRow() > 0;

				//Check against other ship if not on bottom row
				if (Available) {
					for (int x = ship.GetColumn(); x < ship.GetColumn() + ship.GetWidth(); x++)
						if (m_Board[x, ship.GetRow() - 1] != null) Available = false;
					if (Available) { ship.AddMovement(Direction.NegativeY); }
				}

				//Is ship on left most row?
				Available = (ship.GetColumn() + ship.GetWidth()) < m_Board.GetLength(0);

				//Check against other ship if not on bottom row
				if (Available) {
					for (int y = ship.GetRow(); y < ship.GetRow() + ship.GetHeight(); y++)
						if (m_Board[ship.GetColumn() + ship.GetWidth(), y] != null) Available = false;
					if (Available) { ship.AddMovement(Direction.PositiveX); }
				}

				//Is ship on right most row?
				Available = ship.GetColumn() > 0;

				//Check against other ship if not on bottom row
				if (Available) {
					for (int y = ship.GetRow(); y < ship.GetRow() + ship.GetHeight(); y++)
						if (m_Board[ship.GetColumn() - 1, y] != null) Available = false;
					if (Available) { ship.AddMovement(Direction.NegativeX); }
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

		public void SaveMap() {
			//Create gamedata from current game
			GameData Save = new GameData();
			foreach (Ship ship in m_Ships) Save.AddShip(ship);
			Save.Goal = m_Data.Goal;

			//Save
			GameData.SaveGameData(Save, "Output");
		}
    }
}
