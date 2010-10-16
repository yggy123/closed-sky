using Klotski.States;
using FlatRedBall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TomShane.Neoforce.Controls;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

//Application namespace
namespace Klotski.Utilities {
    /// <summary>
    /// Manages various game states.
    /// </summary>
    public class StateManager : Game {
    	private readonly StateID	m_StartState;
		private readonly object[]	m_StartParameters;

        //Members
        private SpriteBatch						m_SpriteBatch;
        private readonly GraphicsDeviceManager  m_Device;

		//State list
		private int							m_Depth;
    	private readonly LinkedList<State>	m_StateList;

    	//Class constants;
		#region Constants
		private const string    GUI_SKIN = "Default";
		private const string    CONTENT_ROOT = "Content";
		private const bool      SHOW_CURSOR = true;
		private const bool      FIXED_STEP = false;
		private const int		GAME_FPS = 60; 
		#endregion

        /// <summary>
        /// State manager class constructor
        /// </summary>
		public StateManager(StateID id, object[] parameters) {
			//Save starting state data
        	m_StartState		= id;
        	m_StartParameters	= parameters;

            //Set device
            m_SpriteBatch   = null;
            m_Device        = new GraphicsDeviceManager(this);

			//Configure engine
			#region Engine configuration
            Content.RootDirectory				= CONTENT_ROOT;
            IsMouseVisible						= SHOW_CURSOR;
            IsFixedTimeStep						= FIXED_STEP;
            m_Device.IsFullScreen               = Global.APP_FULLSCREEN;
            m_Device.PreferMultiSampling        = Global.APP_ANTIALIAS;
			m_Device.PreferredBackBufferWidth	= Global.APP_WIDTH;
			m_Device.PreferredBackBufferHeight	= Global.APP_HEIGHT;

			#endregion

			//Create GUI Manager
			#region Global GUI manager instantiation
			Global.GUIManager = new Manager(this, m_Device, GUI_SKIN, false);
			Global.GUIManager.RenderTargetUsage = RenderTargetUsage.DiscardContents;
			#endregion
			
			//Create other global engines
			Global.SoundManager = new SoundManager();
			Global.Logger		= new Logger(Global.APP_NAME);

			//Create state list
			m_Depth		= 0;
			m_StateList = new LinkedList<State>();
			m_StateList.Clear();

			//Logging
			Global.Logger.AddLine("Engine started.");
        }

        /// <summary>
        /// Initialize the manager.
        /// </summary>
        protected override void Initialize() {
            //Initialize engine
			#region Engine initialization
			m_SpriteBatch = new SpriteBatch(GraphicsDevice);
			FlatRedBallServices.InitializeFlatRedBall(this, m_Device);

			//Initialize GUI manager
			Global.GUIManager.Initialize();
			Global.GUIManager.RenderTarget = new RenderTarget2D(
				GraphicsDevice,
				m_Device.PreferredBackBufferWidth,
				m_Device.PreferredBackBufferHeight,
				1,
				SurfaceFormat.Color,
				Global.GUIManager.RenderTargetUsage);
			Global.GUIManager.TargetFrames = GAME_FPS;

			//Initialize sound manager
			Global.SoundManager.Initialize();

			//Logging
			Global.Logger.AddLine("Engine initialized.");
			#endregion

			//Goes to starting state
			Global.StateManager.GoTo(m_StartState, m_StartParameters);

            //Initialize game class.
            base.Initialize();
        }

        public State GetPreviousState(State state) {
            //Check for null
            if (state == null)          return null;
            if (m_StateList == null)    return null;

            //Variable
            LinkedListNode<State> Source  = null;
            LinkedListNode<State> Current = m_StateList.First;

            //While state still exist
            while (Current != null) {
                //Find source
                if (Current.Value == state) Source = Current;
                Current = Current.Next;
            }

            //Return null if there's no previous state
            if (Source.Previous == null) return null;

            //Otherwise, return previous value
            return Source.Previous.Value;
        }

		/// <summary>
		/// Moves to another state, adding a new state if state has not existed.
		/// </summary>
		/// <param name="id">State identifier</param>
		/// <param name="parameters">Arguments needed by the state constructor.</param>
		public void GoTo(StateID id, object[] parameters) { GoTo(id, parameters, false); }

		/// <summary>
		/// Moves to another state.
		/// </summary>
		/// <param name="id">State identifier</param>
		/// <param name="parameters">Arguments needed by the state constructor.</param>
		/// <param name="swap">Swaps current state with the new one or no.</param>
		public void GoTo(StateID id, object[] parameters, bool swap) {
			//Search for existing state
			State Destination = null;
			foreach (State state in m_StateList) if (state.GetIdentifier() == id) Destination = state;

			//If exist, return there
			if (Destination != null) ReturnTo(Destination);
			else {
				//Create new state
				State NewState = StateFactory.Instance().CreateState(id, parameters);

				//If not swap, add it on the last
				if (!swap) AddState(NewState);
				else {
					//Add it before
					m_Depth += 1;
					m_StateList.AddBefore(m_StateList.Last, NewState);
					NewState.Initialize();

					//Logging
					Global.Logger.AddLine(NewState + " has been added to the state list.");
				}
			}
		}

		/// <summary>
		/// Quit the game by removing all existing state.
		/// </summary>
		public void Quit() {
			//Set depth as many as the number of state
			m_Depth = m_StateList.Count;
		}

		/// <summary>
		/// Adds a new state at the end of the state list.
		/// </summary>
		/// <param name="state">The new state that will be added</param>
		private void AddState(State state) {
			//Do nothing if state is null
			if (state == null) return;

			//Add and initialize state
			m_StateList.AddLast(state);
			m_StateList.Last.Value.Initialize();
			EnterState();

			//Logging
			Global.Logger.AddLine(state + " has been added to the state list.");

			//If top state isn't popup, turn off lower states
			#region Manage popup states);
			if (!m_StateList.Last.Value.IsPopUp()) {
				//Create variable
				LinkedListNode<State> Current = m_StateList.Last;

				do {
					//Turn off previous state
					Current = Current.Previous;
					if (Current != null) Current.Value.SetVisibility(false);
					//While current state is popup
				} while (Current != null && Current.Value.IsPopUp());
			}
			#endregion
		}

		/// <summary>
		/// Returns to an existing state.
		/// </summary>
		/// <param name="state">The state to return to.</param>
		private void ReturnTo(State state) {
			//Do nothing if there's no such state
			if (!m_StateList.Contains(state)) return;

			//Keep marking state until found
			LinkedListNode<State> Current = m_StateList.Last;
			while (Current != null && Current.Value != state) {
				//Increase depth and go to next
				Current = Current.Previous;
				m_Depth++;
			}
		}

		/// <summary>
		/// Things to do when entering a state.
		/// </summary>
		private void EnterState() {
			//Ensure state exist
			if (m_StateList.Count <= 0) return;
			if (m_StateList.Last == null) return;

			//Set state visibility
			m_StateList.Last.Value.OnEnter();
			IsMouseVisible = m_StateList.Last.Value.IsCursorVisible();
			m_StateList.Last.Value.SetVisibility(true);

			//If state is popup, make the rest visible
			#region Manage popup states
			if (m_StateList.Last.Value.IsPopUp()) {
				//Create variable
				LinkedListNode<State> Current = m_StateList.Last;

				do {
					//Turn on previous state
					Current = Current.Previous;
					if (Current != null) Current.Value.SetVisibility(true);
					//While current state is popup
				} while (Current != null && Current.Value.IsPopUp());
			}
			#endregion
		}

		/// <summary>
		/// Removes top state.
		/// </summary>
    	private void RemoveState() {
			//Do nothing if list is empty
			if (m_StateList == null)	return;
			if (m_StateList.Count <= 0)	return;

			//Get state name
			string StateName = m_StateList.Last.Value.ToString();

			//Exit last state
    		m_StateList.Last.Value.OnExit();
			m_StateList.RemoveLast();

			//Logging
			Global.Logger.AddLine(StateName + " has been removed from the state list.");

			//Enter new state
			EnterState();
    	}

        /// <summary>
        /// Updates state manager and the states inside.
        /// </summary>
        /// <param name="time">Amount of time passed between calls.</param>
        protected override void Update(GameTime time) {
            //Updates the engines
			FlatRedBallServices.Update(time);
			Global.SoundManager.Update(time);
            Global.GUIManager.Update(time);
			Global.Logger.SetTime(time.TotalGameTime.ToString());

			//Updates state
			#region Process state list
			//Trim states
			for (int i = 0; i < m_Depth; i++) RemoveState();
			m_Depth = 0;

			//Is list empty?
			bool Empty = m_StateList.Count <= 0;

			//Does last state exist and not null?
			bool TopExist = false;
			if (!Empty) TopExist = (m_StateList.Last.Value != null);

			//No active state found
			bool ActiveFound = false;

			//While not empty, last state exist, and active state has not been found
			while (!Empty && TopExist && !ActiveFound) {
				//Remove last state if not active
				if (!m_StateList.Last.Value.IsActive()) RemoveState();
				//Otherwise, found active state
				else ActiveFound = true;

				//Check if state is now empty
				Empty = m_StateList.Count <= 0;
				TopExist = Empty ? false : (m_StateList.Last != null);
			}

			//Quit if list is empty or last state doesn't exist
			if (Empty || !TopExist) Exit();
			else m_StateList.Last.Value.Update(time); 
			#endregion

            //Updates the original class);
            base.Update(time);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="time">Amount of time passed between calls.</param>
		protected override void Draw(GameTime time) {
			//Draws the game
			Global.GUIManager.Draw(time);
            FlatRedBallServices.Draw();
			base.Draw(time);

			//Draw GUI texture
			m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
			m_SpriteBatch.Draw(Global.GUIManager.RenderTarget.GetTexture(), Vector2.Zero, Color.White);
			m_SpriteBatch.End();
        }
    }
}
