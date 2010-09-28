
//Namespaces used
using FlatRedBall;
using FlatRedBall.Graphics;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using FlatRedBall.Graphics.Model;
using System.Collections.Generic;
using TomShane.Neoforce.Controls;

//Application namespace
namespace Klotski.States {
	/// <summary>
	/// Abstract state class that serves as a template for other states.
	/// </summary>
	public abstract class State {
		//Members
		protected StateID	m_ID;
		protected bool		m_PopUp;
		protected bool		m_Active;
		protected bool		m_VisibleCursor;

		//Containers
		protected List<Control>					m_Panel;
		protected FlatRedBall.Graphics.Layer	m_Layer;

		/// <summary>
		/// Class constructor.
		/// <param name="id">State identifier</param>
		/// </summary>
		protected State(StateID id) {
			//Set identifier
			m_ID = id;

			//Set default value
			m_Active		= true;
			m_PopUp			= false;
			m_VisibleCursor = false;

			//Create layers
			m_Layer = SpriteManager.AddLayer();
			m_Panel = new List<Control>();
		}

		/// <summary>
		/// What happens when the state got removed.
		/// </summary>
		public virtual void OnExit() {
			//Removes all GUI
			foreach (Control control in m_Panel) Global.GUIManager.Remove(control);

			//Removes FRB entities
			foreach (Text text in m_Layer.Texts)				TextManager.RemoveText(text);
			foreach (Sprite sprite in m_Layer.Sprites)			SpriteManager.RemovePositionedObject(sprite);
			foreach (PositionedModel model in m_Layer.Models)	SpriteManager.RemovePositionedObject(model);

			//Removes the layer
			SpriteManager.RemoveLayer(m_Layer);
		}

		/// <summary>
		/// Checks whether state is a popup or no.
		/// </summary>
		/// <returns>State's pop up field.</returns>
		public bool IsPopUp() {
			return m_PopUp;
		}

		/// <summary>
		/// Checks whether cursor should be drawn in this state or no.
		/// </summary>
		/// <returns>State's cursor visibility</returns>
		public bool IsCursorVisible() {
			return m_VisibleCursor;
		}

		/// <summary>
		/// State identifier accessor.
		/// </summary>
		/// <returns>State's ID.</returns>
		public StateID GetIdentifier() {
			return m_ID;
		}

		/// <summary>
		/// Set state visibility.
		/// </summary>
		/// <param name="visibility">State's new visibility.</param>
		public virtual void SetVisibility(bool visibility) {
			//Set layer visibility
			m_Layer.Visible = visibility;
			foreach (Control control in m_Panel) control.Visible = visibility;
		}

		public abstract void Initialize();
		public abstract void OnEnter();
		public abstract void Update(GameTime time);

		public bool IsActive() {
			return m_Active;
		}
	}
}
