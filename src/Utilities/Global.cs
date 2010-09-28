
//Namespaces used
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
        public const string BOARD_FOLDER = "Boards/";
		public const string CONTENT_FOLDER = "Content/";
		public const string IMAGE_FOLDER = CONTENT_FOLDER + "Images/";
		public const string BGM_FOLDER = CONTENT_FOLDER + "Audio/BGM/";


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
		public static readonly string[] TITLE_MENU = new string[4] { "Save the King", "Trap the King", "Credit", "Exit" };
		public const int				TITLE_MENU_TOP = 360;
		public const int				TITLE_MENU_LEFT = 24;
		public const int				TITLE_MENU_SPACE = 52;
		public const string				TITLE_MENU_FONT = FONT_FOLDER + "Tahoma";

        //Config::Board
        public const string BOARD_EXTENSION = ".kmb";
        public const int    BOARD_LISTMENU_LEFT = 600;
        public const int    BOARD_LISTMENU_TOP  = 160;


		//Error & Exceptions
		public const string ERROR_MSGBOX_TITLE	= "Application has encountered an error!";
		public const string UNKNOWNSTATE_ERROR	= "ERROR: Unidentified state ID is used.";
		public const string FMODVERSION_ERROR	= "ERROR: Newer version of FMOD is required.";
		public const string NOFILE_ERROR		= "ERROR: File not found. ";
	}
}
