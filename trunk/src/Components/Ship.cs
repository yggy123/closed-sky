
//Namespace used
using Klotski.Utilities;
using FlatRedBall.Graphics;
using Microsoft.Xna.Framework;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Ship class, a block in klotski puzzle.
	/// </summary>
	public class Ship : Playable {
		//Data
		protected int m_Row;
		protected int m_Column;
		protected int m_Width;
		protected int m_Height;

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
		}

        public void Initialize() {
			//Get model name
        	string model = Global.BLOCKS_FOLDER + "Balloon" + m_Width + m_Height;

			//Calculate position via row and column
			float X = (m_Column + 0.5f)	* (Global.GAMETILE_WIDTH  + Global.GAMEGAP_WIDTH);
			float Y = (m_Row	+ 0.5f)	* (Global.GAMETILE_HEIGHT + Global.GAMEGAP_HEIGHT);

			//Initialize
			Initialize(model, false, X, Global.GAME_VERTICAL, Y);

			System.Console.WriteLine("Min " + m_AABB.Min + " max " + m_AABB.Max);
			System.Console.WriteLine("Min2 " + GetBoundingBox().Min + " max2 " + GetBoundingBox().Max);
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

		public override void Update(GameTime time) {
			//
		}

		public override void UpdateControl(GameTime time) {
			//
		}
	}
}
