//Namespaces used
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace Klotski.States
{
    public class StateInitEditor : State
    {
        //Member
        private Window      m_Window;
        private Button[]    m_Buttons;
        private ComboBox[]  m_ComboBoxs;
        private Label       m_Label;

        private int         HeightIndex;
        private int         WidthIndex;

        //Class Constructor
        public StateInitEditor()
            : base(StateID.InitEditor)
        {
            //Draw cursor
            m_VisibleCursor = true;
            m_PopUp = true;

            //Set Values
            HeightIndex = 1;
            WidthIndex = 0;

            //Nulling value
            m_Label = null;
            m_Buttons = null;
        }

        public override void Initialize()
        {
            #region Initialize Window
            //Create Window
            m_Window = new Window(Global.GUIManager);
            m_Window.Init();
            m_Window.CloseButtonVisible = false;
            m_Window.Movable = false;
            m_Window.StayOnBack = true;
            m_Window.Resizable = false;

            m_Window.Text = "Inititalize Editor";
            m_Window.Top = Global.INITEDITORWINDOW_TOP;
            m_Window.Left = Global.INITEDITORWINDOW_LEFT;
            m_Window.Width = Global.INITEDITORWINDOW_WIDTH;
            m_Window.Height = Global.INITEDITORWINDOW_HEIGHT;

            m_Panel.Add(m_Window);
            Global.GUIManager.Add(m_Window);
            #endregion

            #region Initialize combo box for board height and width
            m_ComboBoxs = new ComboBox[2];
            for (int i = 0; i < m_ComboBoxs.Length; i++)
            {
                m_ComboBoxs[i] = new ComboBox(Global.GUIManager);
                m_ComboBoxs[i].Top = Global.INITCOMBO_TOP + (i * Global.INITCOMBO_SPACE);
                m_ComboBoxs[i].Left = Global.INITCOMBO_LEFT;
                m_ComboBoxs[i].Width = Global.INITCOMBO_WIDTH;
                m_ComboBoxs[i].Items.Add(4);
                m_ComboBoxs[i].Items.Add(5);
                m_ComboBoxs[i].Items.Add(6);
                m_ComboBoxs[i].Items.Add(7);
                m_ComboBoxs[i].Items.Add(8);
                m_ComboBoxs[i].Items.Add(9);
                m_ComboBoxs[i].ReadOnly = true;
                m_ComboBoxs[i].Init();

                //Add the buttons
                m_Panel.Add(m_ComboBoxs[i]);
                Global.GUIManager.Add(m_ComboBoxs[i]);
            }
            #endregion


            #region Initialize Buttons
            m_Buttons = new Button[Global.INITEDITOR_MENU.Length];
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                //Create buttons
                m_Buttons[i] = new Button(Global.GUIManager);
                m_Buttons[i].Text = Global.INITEDITOR_MENU[i];
                m_Buttons[i].Top = Global.INITEDITOR_TOP;
                m_Buttons[i].Left = Global.INITEDITOR_LEFT + (i * (Global.INITEDITOR_WIDTH + Global.INITEDITOR_SPACE));
                m_Buttons[i].Width = Global.INITEDITOR_WIDTH;
                m_Buttons[i].Init();

                //Add the buttons
                m_Panel.Add(m_Buttons[i]);
                Global.GUIManager.Add(m_Buttons[i]);

                //Set event handler
                m_Buttons[i].Click += InitChoose;
            }
            #endregion

            //Create label
            m_Label = new Label(Global.GUIManager);
            m_Label.Init();
            m_Label.Text = Global.INITEDITORLABEL_TEXT;
            m_Label.Top = Global.INITEDITORLABEL_TOP;
            m_Label.Left = Global.INITEDITORLABEL_LEFT;
            m_Label.Width = Global.INITEDITORLABEL_WIDTH;
            m_Label.Height = Global.INITEDITORLABEL_HEIGHT;

            //Add Label
            m_Panel.Add(m_Label);
            Global.GUIManager.Add(m_Label);

        }

        private void InitChoose(object sender, EventArgs e)
        {
            //Cancel Button
            if (sender == m_Buttons[0]) m_Active = false;

            //Go to editor
            if (sender == m_Buttons[1])
            {
                //Prepare parameter
                object[] Parameters;

                //Create parameter
                Parameters = new object[2];
                Parameters[0] = m_ComboBoxs[0].Items[HeightIndex];
                Parameters[1] = m_ComboBoxs[1].Items[WidthIndex];

                //Go to story
                Global.StateManager.GoTo(StateID.Editor, Parameters);

                //Make state inactive
                m_Active = false;
            }
        }

        public override void OnEnter()
        {
        }

        public override void Update(GameTime time)
        {
            if (m_ComboBoxs[0].ItemIndex != -1) HeightIndex = m_ComboBoxs[0].ItemIndex;
            if (m_ComboBoxs[1].ItemIndex != -1) WidthIndex  = m_ComboBoxs[1].ItemIndex;
        }
    }
}