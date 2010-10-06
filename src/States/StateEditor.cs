//Namespaces used
using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.IO;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Lighting;
using Klotski.Controls;
using Klotski.Utilities;
using Klotski.Components;
using Klotski.States.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using System.Collections.Generic;

//Application namespace
namespace Klotski.States
{
    /// <summary>
    /// To create game board
    /// </summary>
    public class StateEditor : State {
        //Logics
        private GameData    m_Data;
        private Ship[,]     m_Board;
        private int         BoardRow, BoardColumn;

        //Sky component
        private Sky m_Sky;
        private Layer m_SkyLayer;

        //GUI
        private Button[] m_Buttons;

        public StateEditor(int Row, int Column) : base(StateID.Editor) {
            //Draw Cursor
            m_VisibleCursor = true;

            //Set Values
            BoardRow    = Row;
            BoardColumn = Column;

            //Nulling Stuffs
            m_Data      = null;
            m_Buttons   = null;
            m_Sky       = null;
            m_SkyLayer  = null;
            m_Board     = null;
        }

        public override void Initialize() {
            //Initialize environment
            CreateSky();
            CreateLighting();

            //Create Board
            CreateEmptyBoard(BoardRow, BoardColumn);

            //Create camera
            CreateBirdsView();
        }

        public override void OnEnter(){
        }

        public override void Update(GameTime time){
            UpdateCamera(time);
            if (InputManager.Mouse.ButtonPushed(FlatRedBall.Input.Mouse.MouseButtons.LeftButton)) {
                PutShipToBoard();
            }
            else if (InputManager.Mouse.ButtonPushed(FlatRedBall.Input.Mouse.MouseButtons.RightButton)) {
                DeleteShipInBoard();
            }
        }

        #region Initialize Stuffs
        /// <summary>
        /// Creates a skysphere
        /// </summary>
        private void CreateSky()
        {
            //Add a sky layer under main layer
            Layer Temp = SpriteManager.AddLayer();
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
        private void CreateLighting()
        {
            //Turn on lighting
            FlatRedBall.Graphics.Renderer.LightingEnabled = true;

            //Set lighting used
            FlatRedBall.Graphics.Renderer.Lights.SetDefaultLighting(LightCollection.DefaultLightingSetup.Daylight);
            FlatRedBall.Graphics.Renderer.Lights.DirectionalLights[0].Direction = new Vector3(0.2f, -0.6f, -0.8f);
        }

        /// <summary>
        /// Create array of board from the list of ships.
        /// </summary>
        private void CreateEmptyBoard(int row, int column)
        {
            //Create board
            m_Board = new Ship[column, row];
            for (int x = 0; x < column; x++) for (int y = 0; y < row; y++) m_Board[x, y] = null;
        }

        /// <summary>
        /// Create bird's view camera
        /// </summary>
        private void CreateBirdsView()
        {
            //Calculate center of the board
            int Column = m_Board.GetLength(0);
            int Row = m_Board.GetLength(1);
            float CenterX = Column * (Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2)) / 2.0f;
            float CenterZ = Row * (Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2)) / 2.0f;

            //Set position
            SpriteManager.Camera.X = CenterX;
            SpriteManager.Camera.Y = Global.GAME_CAMERAHEIGHT;
            SpriteManager.Camera.Z = CenterZ;

            //Set rotation
            SpriteManager.Camera.RotationX = (float)(Math.PI * 1.5f);
            SpriteManager.Camera.RotationY = 0.0f;
            SpriteManager.Camera.RotationZ = 0.0f;

            //Logging
            if (Global.Logger != null) Global.Logger.AddLine("Bird's view camera created.");
        }
        #endregion

        #region Update Function

        private void UpdateCamera(GameTime time) {
            //Move Camera according input
            if (InputManager.Keyboard.KeyDown(Keys.W)) SpriteManager.Camera.Z--;
            else if (InputManager.Keyboard.KeyDown(Keys.S)) SpriteManager.Camera.Z++; 
            else if (InputManager.Keyboard.KeyDown(Keys.D)) SpriteManager.Camera.X++;
            else if (InputManager.Keyboard.KeyDown(Keys.A)) SpriteManager.Camera.X--;
        }

        #endregion

        private Vector2 CalculatePositionFromRay() {
            //Assumption -> Stuff.Y = 0.0f
            Vector2 ReturnPosition = new Vector2(0.0f, 0.0f);
            Ray MouseRay = FlatRedBall.Input.InputManager.Mouse.GetMouseRay(SpriteManager.Camera);
            
            //Normalize Direction Vector
            Vector3 NormalDir = MouseRay.Direction;
            NormalDir.Normalize();

            //Calculate Distance
            float Distance = MouseRay.Position.Y / (-NormalDir.Y);

            //Calculate Position
            ReturnPosition.X = MouseRay.Position.X + (Distance * NormalDir.X);
            ReturnPosition.Y = MouseRay.Position.Z + (Distance * NormalDir.Z);

            return ReturnPosition;
        }

        private void PutShipToBoard() {
            Vector2 BoardPosition = CalculatePositionFromRay();
            //Calculate Tile Position
            int TileColumn      = (int)BoardPosition.X / (int)(Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2));
            int TileRow         = (int)BoardPosition.Y / (int)(Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2));

            if ((TileRow >= 0) && (TileRow < BoardRow)) {
                if ((TileColumn >= 0) && (TileColumn < BoardColumn)) {
                    Ship s = new Ship(m_Layer, TileRow, TileColumn, 1, 1);
                    s.Initialize();
                    m_Board[TileColumn, TileRow] = s;
                }
            }
        }

        private void DeleteShipInBoard() {
            Vector2 BoardPosition = CalculatePositionFromRay();
            //Calculate Tile Position
            int TileColumn = (int)BoardPosition.X / (int)(Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2));
            int TileRow = (int)BoardPosition.Y / (int)(Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2));
            //Hurr delete needed.
            if ((TileRow >= 0) && (TileRow < BoardRow)) {
                if ((TileColumn >= 0) && (TileColumn < BoardColumn)) {
                    if (m_Board[TileColumn, TileRow] != null) m_Board[TileColumn, TileRow] = null;
                }
            }
        }
    }
}
