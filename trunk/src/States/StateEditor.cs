//Namespaces used
using System;
using System.Collections.Generic;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Lighting;
using Klotski.Utilities;
using Klotski.Components;
using Klotski.States.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework.Graphics;
using FlatRedBall.Graphics.Model;

//Application namespace
namespace Klotski.States
{
    /// <summary>
    /// To create game board
    /// </summary>
    public class StateEditor : State {
        //Logics
        private GameData    m_Data;
        private List<Ship>  m_Ships;
        private Ship[,]     m_Board;
        private int         BoardRow, BoardColumn;
        private int         SelectedHeight, SelectedWidth;

        //Sky component
        private Sky     m_Sky;
        private Layer   m_SkyLayer;

        //Model
        private PositionedModel m_Arrow;

        //GUI
        private Button[]        m_Buttons;
        private Button[]        m_MenuButtons;
        private SideBar         m_SideBar;
        private SideBarPanel    m_SideBarPanel;
        private Window          m_Window;
        private Label           m_Help;

        public StateEditor(int Row, int Column) : base(StateID.Editor) {
            //Draw Cursor
            m_VisibleCursor = true;

            //Set Values
            BoardRow        = Row;
            BoardColumn     = Column;
            SelectedHeight  = 1;
            SelectedWidth   = 1;

            //Nulling Stuffs
            m_Data          = null;
            m_Buttons       = null;
            m_MenuButtons   = null;
            m_Sky           = null;
            m_SkyLayer      = null;
            m_Arrow         = null;
            m_Ships         = null;
            m_Board         = null;
            m_SideBar       = null;
            m_SideBarPanel  = null;
            m_Window        = null;
            m_Help          = null;
        }

        public override void Initialize() {
            //Initialize environment
            CreateSky();
            CreateLighting();

            //Create and set Game Data values
            m_Data = new GameData();
            m_Data.Goal = (BoardColumn / 2) - 1;

            //Create Ship List
            m_Ships = new List<Ship>();

            //Create Board
            CreateEmptyBoard(BoardRow, BoardColumn);

            //Create camera
            CreateBirdsView();

            //Put King's ship according to goal
            PutKingShip();

            //Create goal arrow
            CreateArrow();

            //Create GUI
            CreateGUI();

            //Create Lines
            CreateLines();

            //Start BGM
            Global.SoundManager.PlayBGM(Global.EDITOR_BGM);
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

            if (InputManager.Keyboard.KeyPushed(Keys.Tab))
            {
                m_Window.Visible = !m_Window.Visible;
                m_Help.Visible = !m_Help.Visible;
            }
            else if (InputManager.Keyboard.KeyPushed(Keys.Escape)) m_Active = false;
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

        /// <summary>
        /// Create goal arrow.
        /// </summary>
        private void CreateArrow()
        {
            //Create arrow model
            m_Arrow = ModelManager.AddModel(Global.MODEL_FOLDER + "Arrow", FlatRedBallServices.GlobalContentManager, true);
            m_Arrow.CurrentAnimation = "Default";
            ModelManager.AddToLayer(m_Arrow, m_Layer);

            //Move arrow to goal
            m_Arrow.X = (Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2)) * (m_Data.Goal + 1.0f);
            m_Arrow.Y = -50.0f;
            m_Arrow.Z = 0.0f;
        }

        private void PutKingShip()
        {
            Ship ship = new Ship(m_Layer, 0, m_Data.Goal, 2, 2);
            ship.Initialize();
            m_Ships.Add(ship);

            //Put into board
            for (int x = m_Data.Goal; x < m_Data.Goal + ship.GetWidth(); x++) for (int y = 0; y < 0 + ship.GetHeight(); y++)
                    m_Board[x, y] = ship;
        }

        private void CreateGUI() {
            //Create Sidebar
            m_SideBar = new SideBar(Global.GUIManager);
            m_SideBar.Top = Global.EDITORSIDEBAR_TOP;
            m_SideBar.Left = Global.EDITORSIDEBAR_LEFT;
            m_SideBar.Width = Global.EDITORSIDEBAR_WIDTH;
            m_SideBar.Height = Global.EDITORSIDEBAR_HEIGHT;
            m_Panel.Add(m_SideBar);
            Global.GUIManager.Add(m_SideBar);

            //Create Sidebar Panel
            m_SideBarPanel = new SideBarPanel(Global.GUIManager);
            m_SideBarPanel.Top = Global.EDITORSIDEBARPANEL_TOP;
            m_SideBarPanel.Left = Global.EDITORSIDEBARPANEL_LEFT;
            m_SideBarPanel.Width = Global.EDITORSIDEBARPANEL_WIDTH;
            m_SideBarPanel.Height = Global.EDITORSIDEBARPANEL_HEIGHT;
            m_Panel.Add(m_SideBarPanel);
            Global.GUIManager.Add(m_SideBarPanel);

            //Create Editor Buttons
            m_Buttons = new Button[Global.EDITORBUTTON_MENU.Length];
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                //Create buttons
                m_Buttons[i] = new Button(Global.GUIManager);
                m_Buttons[i].Text = Global.EDITORBUTTON_MENU[i];
                m_Buttons[i].Top = Global.EDITORBUTTON_TOP + (i * Global.EDITORBUTTON_SPACE);
                m_Buttons[i].Left = Global.EDITORBUTTON_LEFT;
                m_Buttons[i].Width = Global.EDITORBUTTON_WIDTH;
                m_Buttons[i].Init();

                //Add the buttons
                m_Panel.Add(m_Buttons[i]);
                Global.GUIManager.Add(m_Buttons[i]);

                //Set event handler
                m_Buttons[i].Click += EditorChoose;
            }

            //Create Menu Buttons
            m_MenuButtons = new Button[Global.EDITOR_MENU.Length];
            for (int i = 0; i < m_MenuButtons.Length; i++)
            {
                //Create buttons
                m_MenuButtons[i] = new Button(Global.GUIManager);
                m_MenuButtons[i].Text = Global.EDITOR_MENU[i];
                m_MenuButtons[i].Top = Global.EDITORMENU_TOP + (i * Global.EDITORMENU_SPACE);
                m_MenuButtons[i].Left = Global.EDITORMENU_LEFT;
                m_MenuButtons[i].Width = Global.EDITORMENU_WIDTH;
                m_MenuButtons[i].Init();

                //Add the buttons
                m_Panel.Add(m_MenuButtons[i]);
                Global.GUIManager.Add(m_MenuButtons[i]);

                //Set event handler
                m_MenuButtons[i].Click += MenuChoose;
            }

            //Create Help Window
            m_Window = new Window(Global.GUIManager);
            m_Window.Init();
            m_Window.CloseButtonVisible = false;
            m_Window.Movable = false;
            m_Window.StayOnBack = true;
            m_Window.Resizable = false;
            m_Window.Visible = false;

            m_Window.Text = "Editor Help";
            m_Window.Top = Global.EDITORWINDOW_TOP;
            m_Window.Left = Global.EDITORWINDOW_LEFT;
            m_Window.Width = Global.EDITORWINDOW_WIDTH;
            m_Window.Height = Global.EDITORWINDOW_HEIGHT;
            m_Panel.Add(m_Window);
            Global.GUIManager.Add(m_Window);

            //Create label in Help window
            m_Help = new Label(Global.GUIManager);
            m_Help.Init();
            m_Help.Parent = m_Window;
            m_Help.Text = Global.EDITOR_HELP;
            m_Help.Top = Global.EDITORHELP_TOP;
            m_Help.Left = Global.EDITORHELP_LEFT;
            m_Help.Width = Global.EDITORHELP_WIDTH;
            m_Help.Height = Global.EDITORHELP_HEIGHT;


            //Add it to the
            m_Panel.Add(m_Help);
            Global.GUIManager.Add(m_Help);
        }

        private void CreateLines() {
            //Create vertical lines
            for (int i = 0; i < BoardColumn + 1; ++i)
            {
                //Assign variable
                double X = i * (Global.GAMETILE_WIDTH + (2 * Global.GAMEGAP_WIDTH));
                Line L = ShapeManager.AddLine();
                //Draw line
                L.RelativePoint1 = new Point3D(X, Global.EDITORLINE_LIMIT, 0);
                L.RelativePoint2 = new Point3D(X, Global.EDITORLINE_LIMIT, BoardRow * (Global.GAMETILE_HEIGHT + (2 * Global.GAMEGAP_HEIGHT)));
                L.Color = Color.Black;
                ShapeManager.AddToLayer(L, m_Layer);
            }

            //Create horizontal lines
            for (int i = 0; i < BoardRow + 1; ++i)
            {
                //Assign variable
                double Z = i * (Global.GAMETILE_HEIGHT + (2 * Global.GAMEGAP_HEIGHT));
                Line L = ShapeManager.AddLine();
                //Draw line
                L.RelativePoint1 = new Point3D(0, Global.EDITORLINE_LIMIT, Z);
                L.RelativePoint2 = new Point3D(BoardColumn * (Global.GAMETILE_WIDTH + (2 * Global.GAMEGAP_WIDTH)), Global.EDITORLINE_LIMIT, Z);
                L.Color = Color.Black;
                ShapeManager.AddToLayer(L, m_Layer);
            }
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

            //Check if cursor position are on board
            if ((TileRow >= 0) && (TileRow < BoardRow)) {
                if ((TileColumn >= 0) && (TileColumn < BoardColumn)) {
                    //Get variable
                    bool Available = false;
                    bool Empty = true;

                    //Quick check board and size
                    if ((TileRow + SelectedHeight <= BoardRow) && (TileColumn + SelectedWidth <= BoardColumn)) Available = true;

                    //Ensure ship's space is empty
                    if (Available) {
                        for (int x = TileColumn; x < TileColumn + SelectedWidth; x++) for (int y = TileRow; y < TileRow + SelectedHeight; y++)
                                if (m_Board[x, y] != null) Empty = false;
                    }

                    //Place the ship if empty
                    if (Available && Empty){
                        Ship ship = new Ship(m_Layer, TileRow, TileColumn, SelectedWidth, SelectedHeight);
                        m_Ships.Add(ship);
                        ship.Initialize();

                        //Put into board
                        for (int x = TileColumn; x < TileColumn + ship.GetWidth(); x++) for (int y = TileRow; y < TileRow + ship.GetHeight(); y++)
                                m_Board[x, y] = ship;
                    }
                }
            }
        }

        private void DeleteShipInBoard() {
            Vector2 BoardPosition = CalculatePositionFromRay();

            //Calculate Tile Position
            int TileColumn = (int)BoardPosition.X / (int)(Global.GAMETILE_WIDTH + (Global.GAMEGAP_WIDTH * 2));
            int TileRow = (int)BoardPosition.Y / (int)(Global.GAMETILE_HEIGHT + (Global.GAMEGAP_HEIGHT * 2));

            if ((TileRow >= 0) && (TileRow < BoardRow)) {
                if ((TileColumn >= 0) && (TileColumn < BoardColumn)) {
                    if ((m_Board[TileColumn,TileRow] != null) && (!m_Board[TileColumn,TileRow].IsKing())){
                        Ship s = m_Board[TileColumn, TileRow];

                        //Delete Ship from list and board.
                        m_Ships.Remove(s);
                        ResetBoard();

                        //Delete ship entirely
                        s.Remove();
                    }
                }
            }
        }

        private void ResetBoard() {
            for (int x = 0; x < BoardColumn; x++) for (int y = 0; y < BoardRow; y++) m_Board[x, y] = null;
            //Put ship back
            foreach (Ship ship in m_Ships) {
                for (int x = ship.GetColumn(); x < ship.GetColumn() + ship.GetWidth(); x++) for (int y = ship.GetRow(); y < ship.GetRow() + ship.GetHeight(); y++)
                        m_Board[x, y] = ship;
            }
        }

        private void FillGameData()
        {
            foreach (Ship s in m_Ships) m_Data.AddShip(s);
        }

        private void EditorChoose(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (sender == m_Buttons[0]) { SelectedHeight = 1; SelectedWidth = 1; }
            if (sender == m_Buttons[1]) { SelectedHeight = 1; SelectedWidth = 2; }
            if (sender == m_Buttons[2]) { SelectedHeight = 2; SelectedWidth = 1; }
        }

        private void MenuChoose(object sender, TomShane.Neoforce.Controls.EventArgs e) {
            //If back, destroy state
            if (sender == m_MenuButtons[0]) m_Active = false;

            //if trap, create game data and go trap the king
            if (sender == m_MenuButtons[1]) {
                FillGameData();

                //Create parameter
                Object[] Parameters = new object[2];
                Parameters[0] = Player.Storm;
                Parameters[1] = m_Data;

                //Go to play state
                Global.StateManager.GoTo(StateID.Game, Parameters, true);
            }
        }

        /// <summary>
        /// What happens when the state got removed.
        /// </summary>
        public override void OnExit()
        {
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
    }
}
