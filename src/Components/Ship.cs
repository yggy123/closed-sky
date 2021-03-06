﻿
//Namespace used
using System;
using System.Collections.Generic;
using Klotski.Utilities;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//Class namespace
namespace Klotski.Components {
    //Enumeration
	public enum Direction {
		NegativeY = -2,
        NegativeX = -1,
		None	  =  0,
        PositiveX =  1,
        PositiveY =  2
    }

	/// <summary>
	/// Ship class, a block in klotski puzzle.
	/// </summary>
	public class Ship : Playable {
		//Data
		protected int m_Row;
		protected int m_Column;
		protected int m_Width;
		protected int m_Height;

		//Movement
		protected int				m_Move;
		protected float				m_MoveTime;
		protected Vector3           m_Forward;
		protected Direction			m_Movement;
	    protected List<Direction>	m_Available;

		protected static float m_Speed;

		//AI
		protected Stack<Ship>		m_ShipStack;
		protected Stack<Direction>	m_MoveStack;
		public bool Returning;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Ship(Layer layer, int row, int column, int width, int height) : base(layer) {
			//Set variable
			m_Row		= row;
			m_Column	= column;
			m_Width		= width;
			m_Height	= height;

            //Initialize variable
			m_MoveTime	= 0.0f;
			Returning	= false;
            m_Forward   = Vector3.Zero;
            m_Movement	= Direction.None;
            m_Available = new List<Direction>();

			//Prepare stack
			m_ShipStack	= new Stack<Ship>();
			m_MoveStack = new Stack<Direction>();
		}

        public void Initialize() {
			//
        	m_Move = 0;
        	m_Speed = 1.0f;

			//Get model name
        	string model = Global.BLOCKS_FOLDER + "Balloon" + m_Width + m_Height;

			//Calculate position via row and column
			float X = Global.GAMEGAP_WIDTH  + (((Global.GAMEGAP_WIDTH * 2)  + Global.GAMETILE_WIDTH)  * m_Column);
			float Y = Global.GAMEGAP_HEIGHT + (((Global.GAMEGAP_HEIGHT * 2) + Global.GAMETILE_HEIGHT) * m_Row);

			//Initialize
			Initialize(model, false, X, Global.GAME_VERTICAL, Y);

			//Calculate forward vector
            m_Forward	= (m_Width > m_Height) ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(0.0f, 0.0f, 1.0f);

            //Configures camera
            m_Camera.RotationX  = 0.0f;
            m_Camera.RotationY = (float) ((m_Width > m_Height) ? (Math.PI * 1.5f) : Math.PI);
			m_Camera.RotationZ = 0.0f;

			//Clear stack
			m_ShipStack.Clear();
			m_MoveStack.Clear();

			//Logging
			if (Global.Logger != null) Global.Logger.AddLine("Ship created.");
        }

		protected override void CalculateAABB() {
			//Create default AABB
			m_AABB = new BoundingBox(new Vector3(0, -20, 0), new Vector3(40, 4, 40));

			if (m_Width == 2)	m_AABB.Max.X += 60;
			if (m_Height == 2)	m_AABB.Max.Z += 60;
		}

		public bool IsMoving() {
			return (m_Movement != Direction.None);
		}

		/// <summary>
		/// Ships row accessor.
		/// </summary>
		/// <returns>m_Row</returns>
		public int GetRow() {
			return m_Row;
		}

		/// <summary>
		/// Ships column accessor.
		/// </summary>
		/// <returns>m_Column</returns>
		public int GetColumn() {
			return m_Column;
		}

		/// <summary>
		/// Ship's width acessor.
		/// </summary>
		/// <returns>m_Width</returns>
		public int GetWidth() {
			return m_Width;
		}
		
		/// <summary>
		/// Ship's height accessor
		/// </summary>
		/// <returns>m_Height</returns>
		public int GetHeight() {
			return m_Height;
		}

		public int GetMoveNumber() {
			return m_Move;
		}

        public Vector3 GetCenterTop() {
            return new Vector3(
                ((m_AABB.Min.X + m_AABB.Max.X) / 2.0f)  + m_Model.Position.X,
                  m_AABB.Max.Y                          + m_Model.Position.Y,
                ((m_AABB.Min.Z + m_AABB.Max.Z) / 2.0f)  + m_Model.Position.Z);
        }

		public static void IncreaseSpeed() {
			if (m_Speed < 5.0f) m_Speed += 1.0f;
		}
		public static void DecreaseSpeed() {
			if (m_Speed > 1.0f) m_Speed -= 1.0f;
		}

        public List<Direction> GetAvailableMovement()
        {
            return m_Available;
        }

        public void ResetMovement()
        {
            m_Available.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void AddMovement(Direction direction) {
			//Do nothing if there's no direction
			if (direction == Direction.None) return;

            //Check whether direction has existed or no
            bool Contain = false;
            foreach (Direction dir in m_Available) if (dir.Equals(direction)) Contain = true;

            //Add direction if doesn't exist yet
            if (!Contain) m_Available.Add(direction);
        }

		public override void UpdateControl(GameTime time) {
			//Don't receive input if still moving
			if (IsMoving()) return;

			//Initialize direction
			Direction Move = Direction.None;

			//Set movement based on input
			if (InputManager.Keyboard.KeyDown(Keys.W)) Move = m_Forward.X > 0 ? Direction.PositiveX : Direction.PositiveY;
			else if (InputManager.Keyboard.KeyDown(Keys.S)) Move = m_Forward.X > 0 ? Direction.NegativeX : Direction.NegativeY;
			else if (InputManager.Keyboard.KeyDown(Keys.D)) Move = m_Forward.X > 0 ? Direction.PositiveY : Direction.NegativeX;
			else if (InputManager.Keyboard.KeyDown(Keys.A)) Move = m_Forward.X > 0 ? Direction.NegativeY : Direction.PositiveX;

			//Validate movement
			if (m_Available.Contains(Move)) m_Movement = Move;
		}

		public override void Update(GameTime time) {
			//If moving
			if (m_Movement != Direction.None) {
				//Calculate time difference
				float Difference  = time.ElapsedGameTime.Milliseconds / 1000.0f * m_Speed;
				m_MoveTime		 += Difference;

				//Set movement direction on movement
				switch (m_Movement) {
				case Direction.PositiveX:
					m_Model.Position.X += (Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2)) * Difference;
					break;
				case Direction.NegativeX:
					m_Model.Position.X -= (Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2)) * Difference;
					break;
				case Direction.PositiveY:
					m_Model.Position.Z += (Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2)) * Difference;
					break;
				case Direction.NegativeY:
					m_Model.Position.Z -= (Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2)) * Difference;
					break;
				}

				//If has reached limit
				if (m_MoveTime >= 1.0f) {
					//Change row and collumn
					switch (m_Movement) {
					case Direction.PositiveX: { m_Column++; break; }
					case Direction.NegativeX: { m_Column--; break; }
					case Direction.PositiveY: { m_Row++;	break; }
					case Direction.NegativeY: { m_Row--;	break; }
					}

					//Place ship on the right place
					m_Model.X = Global.GAMEGAP_WIDTH  + (((Global.GAMEGAP_WIDTH * 2)  + Global.GAMETILE_WIDTH)  * m_Column);
					m_Model.Z = Global.GAMEGAP_HEIGHT + (((Global.GAMEGAP_HEIGHT * 2) + Global.GAMETILE_HEIGHT) * m_Row);

					//Reset
					m_Movement = Direction.None;
					m_MoveTime = 0;

					m_Move++;
				}
			}

			//Position camera behind the model
            m_Camera.Position = m_Model.Position - 
                (m_Forward * Global.SHIPCAM_DISTANCE) +
                new Vector3(
                    (m_Width > m_Height) ? 0.0f : (m_AABB.Max.X - m_AABB.Min.X) / 2.0f,
                    Global.SHIPCAM_HEIGHT,
                    (m_Width > m_Height) ? (m_AABB.Max.Z - m_AABB.Min.Z) / 2.0f : 0.0f);
		}
	
		/// <summary>
		/// Is this ship is the royal zeppelin?
		/// </summary>
		/// <returns>IOf</returns>
		public bool IsKing() {
			return (m_Width == 2 && m_Height == 2);
		}

		public void Wins() {
			//Set velocity
			m_Model.Velocity = new Vector3(0.0f, 0.0f, -1.0f) * (Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2));
			m_Model.RotationY = (float) (Math.PI * 2);
		}

		public void AnimateWinning() {
		}

		///
        public void Move(Direction direction) {
            //Set direction as current movement
            m_Movement = direction;

			//Add to the stack
			m_MoveStack.Push(direction);
        }

		public Direction GetBackDirection() {
			if (m_MoveStack != null) if (m_MoveStack.Count > 0) return (Direction) (-(int) m_MoveStack.Peek());
			return Direction.None;
		}

		public void Return() {
			//If stack has existing movement, use it to return
			if (m_MoveStack != null) if (m_MoveStack.Count > 0) {
				m_Movement = (Direction)(-(int)m_MoveStack.Pop());
				Returning = true;
			}
		}
	}
}
