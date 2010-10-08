//ReSharper disable LoopCanBeConvertedToQuery

//Namespaces used
using Klotski.Utilities;
using Klotski.Components;
using FlatRedBall.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

//Application namespace
namespace Klotski.States.Game {
	/// <summary>
	/// Class for storing game data.
	/// </summary>
    [Serializable]
	public class GameData {
		//Ship list
		public List<int> m_ShipsRow;
		public List<int> m_ShipsColumn;
		public List<int> m_ShipsHeight;
		public List<int> m_ShipsWidth;

		//Data
		public int Goal;

		/// <summary>
		/// Game data default class constructor.
		/// </summary>
		public GameData() {
			//Initialize data
			m_ShipsRow		= new List<int>();
			m_ShipsColumn	= new List<int>();
			m_ShipsWidth	= new List<int>();
			m_ShipsHeight	= new List<int>();
			Goal			= 1;
		}

        public static void SaveGameData(GameData data, string file) {
            FileStream SaveFile = File.Open(Global.LEVEL_FOLDER + file + Global.LEVEL_EXTENSION, FileMode.Create);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));

            xmlSerializer.Serialize(SaveFile, data);
            SaveFile.Close();

            //Logging
            Global.Logger.AddLine("Level data has been saved to " + Global.LEVEL_FOLDER + file + Global.LEVEL_EXTENSION);
        }

        public static GameData LoadGameData(string file) {
            GameData ReturnData = null;
            if (File.Exists(Global.LEVEL_FOLDER + file + Global.LEVEL_EXTENSION)) {
                FileStream SaveFile = File.Open(Global.LEVEL_FOLDER + file + Global.LEVEL_EXTENSION, FileMode.Open);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));

                ReturnData  = (GameData)xmlSerializer.Deserialize(SaveFile);
                SaveFile.Close();

                //Logging
                if (Global.Logger != null)
                    Global.Logger.AddLine("Level data has been extracted from " + Global.LEVEL_FOLDER + file + Global.LEVEL_EXTENSION);
            }
            return ReturnData;
        }

		/// <summary>
		/// Stores ship data.
		/// </summary>
		/// <param name="ship">The ship which data will be stored</param>
		public void AddShip(Ship ship) {
			//Add each data individually
			AddShip(ship.GetRow(), ship.GetColumn(), ship.GetWidth(), ship.GetHeight());
		}

		/// <summary>
		/// Stores ship data.
		/// </summary>
		/// <param name="row">Ship's row</param>
		/// <param name="column">Ship's column</param>
		/// <param name="width">Ship's width</param>
		/// <param name="height">Ship's height</param>
		public void AddShip(int row, int column, int width, int height) {
			//Add ship data to the list
			m_ShipsRow.Add(row);
			m_ShipsColumn.Add(column);
			m_ShipsWidth.Add(width);
			m_ShipsHeight.Add(height);
		}

		/// <summary>
		/// Load ships from data.
		/// </summary>
		/// <param name="layer"></param>
		/// <returns></returns>
		public List<Ship> LoadShipList(Layer layer) {
			//Create list
			List<Ship> ShipList = new List<Ship>();

			//Create ships
			for (int i = 0; i < m_ShipsRow.Count; i++)
				ShipList.Add(new Ship(layer, m_ShipsRow[i], m_ShipsColumn[i], m_ShipsWidth[i], m_ShipsHeight[i]));

			//Return list
			return ShipList;
		}
	}
}
