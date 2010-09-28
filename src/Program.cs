
//Namespace used
using Klotski.Utilities;
using System.Windows.Forms;

//Application namespace
namespace Klotski {
    //Static class for entry point
    static class Program {
		/// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
			//Create state manager
			try {
				using (StateManager App = new StateManager(Global.APP_STATE, Global.APP_PARAMETERS)) {
					//Start the game
					Global.StateManager = App;
					Global.StateManager.Run();
				}
			} catch (System.Exception ex) {
				//Show error
				MessageBox.Show(ex.Message, Global.ERROR_MSGBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

				//Add it to the log
				if (Global.Logger != null) Global.Logger.AddLine(ex.Message);
			} finally {
				//Print log file
				if (Global.Logger != null) Global.Logger.Print();
			}
        }
    }
}

