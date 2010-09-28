//ReSharper disable LoopCanBeConvertedToQuery

//Namespaces used
using System.Collections.Generic;

//Class namespace
namespace Klotski.Utilities {
	/// <summary>
	/// Logger class for creating application log files.
	/// </summary>
	public class Logger {
		//Members
		protected string		m_Name;
		protected string		m_Time;
		protected List<string>	m_Logs;

		/// <summary>
		/// Logger class constructor.
		/// <param name="name">Application name</param>
		/// </summary>
		public Logger(string name) {
			//Intialize variable
			m_Name = name;
			m_Time = Global.INITIAL_TIME;

			//Create log list
			m_Logs = new List<string>();
			m_Logs.Clear();
		}

		/// <summary>
		/// Set logger's current time.
		/// </summary>
		/// <param name="time"></param>
		public void SetTime(string time) {
			m_Time = time;
		}

		/// <summary>
		/// Add a new line to the log file.
		/// </summary>
		/// <param name="line">The new line to be added to the log file.</param>
		public void AddLine(string line) {
			//Add it to the list
			m_Logs.Add('[' + m_Time + "] " + line);
		}

		/// <summary>
		/// Print the logs to a log file
		/// </summary>
		public void Print() {
			//Append all logs
			string Logs = "";
			foreach (string line in m_Logs) Logs += (line + "\r\n");
			
			//Print log
			FlatRedBall.IO.FileManager.SaveText(Logs, m_Name + Global.LOG_EXTENSION);
		}
	}
}
