
//Namespace used
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Model;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Ship class, a block in klotski puzzle.
	/// </summary>
	public class Ship {
        //Members
        protected Layer m_Layer;
        protected PositionedModel m_Model;

		/// <summary>
		/// Ship class constructor.
		/// </summary>
		/// <param name="layer">FRB layer for the ship model.</param>
        public Ship(Layer layer) {
            //
            m_Layer = layer;
        }

        public void Initialize() {
            //Create and add model to layer
            m_Model = ModelManager.AddModel("Content/Models/Blocks/Balloon12", FlatRedBallServices.GlobalContentManager, false);
            ModelManager.AddToLayer(m_Model, m_Layer);

			m_Model.X = 40.0f;
			m_Model.Z = -40.0f;
			m_Model.Y = -10.0f;
        }

		public void Initialize(float X, float Y, int Width, int Height) {
			//Create and add model to layer
			m_Model = ModelManager.AddModel("Content/Models/Blocks/Balloon" + Width + Height, FlatRedBallServices.GlobalContentManager, false);
			ModelManager.AddToLayer(m_Model, m_Layer);

			m_Model.X = X;
			m_Model.Z = Y;
			m_Model.Y = -10.0f;
		}
    }
}
