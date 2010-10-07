
//Namespaces used
using System;
using Klotski.States;
using TomShane.Neoforce.Controls;

//Class namespace
namespace Klotski.Utilities {
	/// <summary>
	/// Class description.
	/// </summary>
	public static class Global {
		//Managers
		public static Manager		GUIManager { get; set; }
		public static SoundManager	SoundManager { get; set; }
		public static StateManager	StateManager { get; set; }
		public static Logger		Logger { get; set; }

		//Application
		public const int		APP_WIDTH = 800;
		public const int		APP_HEIGHT = 600;
		public const string		APP_NAME = "Klotski";
		public const StateID	APP_STATE = StateID.Title;
		public const object[]	APP_PARAMETERS = null;
		public const float		APPCAM_DEFAULTX = 0.0f;
		public const float		APPCAM_DEFAULTY = 0.0f;
		public const float		APPCAM_DEFAULTZ = 40.0f;
		public const float		APPCAM_DEFAULTROTX = 0.0f;
		public const float		APPCAM_DEFAULTROTY = 0.0f;
		public const float		APPCAM_DEFAULTROTZ = 0.0f;

		//Folder and file
		public const string FONT_FOLDER = "Fonts/";
		public const string BLOCKS_FOLDER = "Blocks/";
		public const string SHADER_FOLDER = "Shaders/";
		public const string TEXTURE_FOLDER = "Textures/";
		public const string CONTENT_FOLDER = "Content/";
		public const string LEVEL_FOLDER = CONTENT_FOLDER + "Levels/";
		public const string IMAGE_FOLDER = CONTENT_FOLDER + "Images/";
		public const string MODEL_FOLDER = CONTENT_FOLDER + "Models/";
		public const string BGM_FOLDER = CONTENT_FOLDER + "Audio/BGM/";
		public const string LEVEL_EXTENSION = ".sky";

		//Sky component
		public const string SKY_MODEL = "Models/Sky";
		public const string SKY_SHADER = SHADER_FOLDER + "Sky";
		public const string SKYTEX_PARAMETER = "SkyTexture";
		public const string SKYVIEW_PARAMETER = "View";
		public const string SKYPROJ_PARAMETER = "Projection";

		//Sound system constants
		public const int		MAX_DRIVERNAME = 256;
		public const int		MAX_SOUNDCHANNEL = 100;
		public const int		EMULATED_BUFFERCOUNT = 10;
		public const uint		EMULATED_BUFFERSIZE = 1024;
		public const int		SIGMATEL_SAMPLERATE = 48000;
		public const string		SIGMATEL_DRIVERNAME = "SigmaTel";

		//Logging
		public const string INITIAL_TIME = "00:00:00";
		public const string LOG_EXTENSION = ".log";

		//Title
		public static readonly string[] TITLE_MENU = { "Save the King", "Create a Storm", "Credit", "Exit" };
		public const int				TITLE_MENU_TOP = 360;
		public const int				TITLE_MENU_LEFT = 24;
		public const int				TITLE_MENU_SPACE = 52;
		public const string				TITLE_MENU_FONT = FONT_FOLDER + "Tahoma";
		public const string				TITLE_BGM = "Title.mp3";

		//Story
		public const float	STORY_Y	= -12.0f;
		public const float	STORY_Z	= 30.0f;
		public const float	STORY_SPEED = 0.6f;
		public const float	STORY_ANGLE = (float)(Math.PI * -0.25f);
		public const float	STORY_TIME = 27.0f;
		public const string	STORY_FONT = "TahomaBitmap";
		public const string	STORY_BGM = "Story.mp3";
	    public const string STORY_CAPTION = "Long time ago, in a kingdom far, far away...";
		public const string STORY_TEXT =
			"It was time for the BlueSky Festival.\r\n" +
			"The time for joy and laughter, with\r\n" +
			"hundreds of balloons filling up the\r\n" +
			"sky, enjoying their gift of freedom.\r\n" +
			"The people were overjoyed, even the\r\n" +
			"king himself rides aboard the royal\r\n" +
			"zeppelin to be with his people.\r\n" +
			"\r\n" +
			"However, it all comes to an abrupt\r\n" +
			"end as a storm silently strikes the\r\n" +
			"kingdom, destroying everything in\r\n" +
			"its path and snatching each balloons\r\n" +
			"from the sky. Only the royal zeppelin\r\n" +
			"could hold, thought it ended up in a\r\n" +
			"tangled mess of air balloons, unable\r\n" +
			"to escape.\r\n" +
			"\r\n" +
			"And it all comes down to a hero to\r\n" +
			"save the king from the closed sky...";

        //Credit
        public const string CREDIT_CAPTION = "";
        public const string CREDIT_TEXT =
            "ASSIGNMENT FROM\r\n" +
            "ITB IRK Laboratory\r\n" +
			"\r\n" +
			"GAME DIRECTOR\r\n" +
			"Raka Mahesa\r\n" +
			"\r\n" +
	        "GAME DESIGNER\r\n" +
	        "Karunia Ramadhan\r\n" +
	        "Raka Mahesa\r\n" +
	        "\r\n" +
	        "GAME PROGRAMMER\r\n" +
	        "Dimas Ciputra\r\n" +
	        "Karunia Ramadhan\r\n" +
            "Raka Mahesa\r\n" +
            "\r\n" +
            "EXTERNAL ENGINE BY\r\n" +
            "Roof Top Pew Wee\r\n" +
            "Tom Shane\r\n" +
			"Firelight Technologies\r\n" +
			"\r\n" +
			"2D Artist\r\n" +
			"Karunia Ramadhan\r\n" +
			"\r\n" +
			"3D Artist\r\n" +
			"Dimas Ciputra\r\n" +
			"\r\n" +
            "CONTRIBUTOR\r\n" +
            "*lardacil\r\n" +
            "~Raven-Experiment-00\r\n" +
			"John Williams\r\n" +
			"\r\n" +
			"SPECIAL THANKS TO\r\n" +
			"George Lucas\r\n" +
			"\r\n" +
			"and\r\n" +
			"\r\n" +
			"YOU\r\n" +
	        "\r\n";

		//State Config
		public const int CONFIGLIST_X = 560;
		public const int CONFIGLIST_Y = 24;
		public const int CONFIGLIST_WIDTH = 228;
		public const int CONFIGLIST_HEIGHT = 400;

        //Config::Board
        public const string BOARD_EXTENSION = ".sky";
        public const int BOARD_LISTMENU_LEFT = 515;
        public const int BOARD_LISTMENU_TOP = 55;

        //Config::HeroButton
        public static readonly string[] HEROBUTTON_MENU = { "Klotski", "BFS", "DFS" };
        public static readonly string[] HEROBUTTON_INFO = {"This is Dummy Text for Klotski"
                                                          ,"This is Dummy Text for BFS"
                                                          ,"This is Dummy Text for DFS"};
        public const string HEROBUTTON_TEXTURE = "Images/HeroButton";
        public const string HEROBUTTONOVERLAY_TEXTURE = "Images/HeroButtonOverlay";
        public const string HEROBUTTON_FONT = FONT_FOLDER + "Tahoma";
        public const int HEROBUTTON_LEFT = 50;
        public const int HEROBUTTON_TOP = 50;
        public const int HEROBUTTON_SPACE = HEROBUTTON_HEIGHT + 24;
        public const int HEROBUTTON_WIDTH = 400;
        public const int HEROBUTTON_HEIGHT = 150;
        public const int HEROBUTTON_IMAGEWIDTH = 115;
        public const int HEROBUTTON_IMAGEHEIGHT = 115;

        //Config::MenuButton
        public static readonly string[] CONFIG_MENU = { "Back", "Go Save The King!" };
        public const int CONFIGBUTTON_LEFT = 515;
        public const int CONFIGBUTTON_TOP = BOARD_LISTMENU_TOP + 420;
        public const int CONFIGBUTTON_SPACE = 40;
        public const int CONFIGBUTTON_WIDTH = 228;

		//State game
		public const float	GAMEGAP_WIDTH		= 10.0f;
		public const float	GAMEGAP_HEIGHT		= 10.0f;
		public const float	GAMETILE_WIDTH		= 40.0f;
		public const float	GAMETILE_HEIGHT		= 40.0f;
		public const float	GAME_VERTICAL		= -10.0f;
	    public const double GAME_FALLLIMIT      = -150.0f;
		public const float	GAME_CAMERAHEIGHT	= 500.0f;
        public const float  GAME_CAMLIMIT       = (float) (Math.PI * 0.4f);
		public const float	GAME_GRAVITY		= 140.0f;
		public const string GAME_BGM = "Game.mp3";

		//Actor
		public const string				ACTOR_MODEL = "Vincent";
		public const float				ACTOR_VELOCITY = 40.0f;
		public const float				ACTOR_JUMPING = 60.0f;
		public const float				ACTORCAM_DISTANCE = 20.0f;
		public const float				ACTORCAM_HEIGHT = 10.0f;
		public static readonly string[] ACTOR_ANIMATIONS = { "Idle", "Walking", "Jumping", "Landing" };

		//GUI
		public const int	LIFE_X = 20;
		public const int	LIFE_Y = 13;
		public const int	LIFEBAR_X = 32;
		public const int	LIFEBAR_Y = 32;
		public const string LIFE_TEXTURE = "Images/Life";
		public const string LIFEBAR_TEXTURE = "Images/LifeBar";
        
        //Ship
	    public const float SHIPCAM_HEIGHT = 10.0f;
	    public const float SHIPCAM_DISTANCE = 50.0f;

		//Pause::MenuButton
		public static readonly string[] PAUSE_MENU = { "Resume", "Restart", "To Title", "Exit" };
		public const int                PAUSEBUTTON_LEFT = 470;
		public const int                PAUSEBUTTON_TOP = 230;
		public const int                PAUSEBUTTON_SPACE = 50;
		public const int                PAUSEBUTTON_WIDTH = 100;
		public const string				PAUSE_HELP =
            "The king is trapped among other balloons\r\n" +
			"that caught up in the storm!\r\n" +
			"\r\n" +
			"OBJECTIVE\r\n" +
			"To save the king, you must get him out of\r\n" +
			"the storm by moving other balloons around\r\n" +
			"so the Royal Zeppelin can safely reach the\r\n" +
			"exit point marked by arrow.\r\n" +
			"\r\n" +
			"CONTROL\r\n" +
			"Move the mouse to move the camera.\r\n" +
			"To switch camera, use the TAB button.\r\n" +   
			"To move around, use the WASD buttons.\r\n" +
			"To jump, you can use the space button.\r\n" +
			"\r\n" +      
			"To take control of a balloon, click the\r\n" +
			"mouse left button while on top of the\r\n" +
			"balloon, and click again to get out.\r\n" +
			"While in control of a balloon, you\r\n" +
			"can move it with WASD buttons.";


	    //Error & Exceptions
		public const string ERROR_MSGBOX_TITLE	= "Application has encountered an error!";
		public const string UNKNOWNSTATE_ERROR	= "ERROR: Unidentified state ID is used.";
		public const string FMODVERSION_ERROR	= "ERROR: Newer version of FMOD is required.";
		public const string NOFILE_ERROR		= "ERROR: File not found. ";
	}
}
