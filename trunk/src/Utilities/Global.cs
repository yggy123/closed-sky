﻿
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

		//Folder and file
		public const string FONT_FOLDER = "Fonts/";
        public const string LEVEL_FOLDER = "Levels/";
        public const string BOARD_FOLDER = "Boards/";
		public const string IMAGE_FOLDER = CONTENT_FOLDER + "Images/";
		public const string SHADER_FOLDER = "Shaders/";
		public const string TEXTURE_FOLDER = "Textures/";
		public const string CONTENT_FOLDER = "Content/";
		public const string MODEL_FOLDER = "Models/";
		public const string BGM_FOLDER = CONTENT_FOLDER + "Audio/BGM/";
		public const string LEVEL_EXTENSION = ".sky";

		//Sky component
		public const string SKY_MODEL = MODEL_FOLDER + "Sky";
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
            "ORIGINAL DESIGN\r\n" +
            "ITB IRK Laboratory\r\n" +
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
            "ENGINE PROGRAMMER\r\n" +
            "Dimas Ciputra\r\n" +
            "Tom Shane\r\n" +
            "Raka Mahesa\r\n" +
            "\r\n" +
            "CONTRIBUTOR\r\n" +
            "*lardacil\r\n" +
            "~Raven-Experiment-00\r\n" +
            "John Williams\r\n" +
	        "\r\n";

		//State Config
		public const int CONFIGLIST_X = 560;
		public const int CONFIGLIST_Y = 24;
		public const int CONFIGLIST_WIDTH = 228;
		public const int CONFIGLIST_HEIGHT = 400;

        //Config::Board
        public const string BOARD_EXTENSION = ".kmb";
        public const int BOARD_LISTMENU_LEFT = 515;
        public const int BOARD_LISTMENU_TOP = 55;

        //Config::HeroButton
        public static readonly string[] HEROBUTTON_MENU = new string[3] { "Klotski", "BFS", "DFS" };
        public static readonly string[] HEROBUTTON_INFO = new string[3] {"This is Dummy Text for Klotski"
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
        public static readonly string[] CONFIG_MENU = new string[2] { "Back", "Go Save The King!" };
        public const int CONFIGBUTTON_LEFT = 515;
        public const int CONFIGBUTTON_TOP = BOARD_LISTMENU_TOP + 420;
        public const int CONFIGBUTTON_SPACE = 40;
        public const int CONFIGBUTTON_WIDTH = 228;

		//Error & Exceptions
		public const string ERROR_MSGBOX_TITLE	= "Application has encountered an error!";
		public const string UNKNOWNSTATE_ERROR	= "ERROR: Unidentified state ID is used.";
		public const string FMODVERSION_ERROR	= "ERROR: Newer version of FMOD is required.";
		public const string NOFILE_ERROR		= "ERROR: File not found. ";
	}
}
