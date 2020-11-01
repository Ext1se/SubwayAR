using UnityEngine;
using System.Collections;
using VoxelBusters.UASUtils.Demo;

namespace VoxelBusters.NativePlugins.Demo
{
	public class NPDemoBase : DemoSubMenu 
	{
		#region Constants

		private		const	string			kThingsToKnowTitle			= "Things to know";

		#endregion

		#region Fields
		
		private				bool			m_showThingsToKnow;
		private				string			m_featureInterfaceInfoText;
		private				string[]		m_additionalInfoTexts		= new string[0];

		#endregion

		#region Unity Methods

		protected override void Start ()
		{
			base.Start ();

			// Set initial values
			m_showThingsToKnow			= true;

			// Update info texts
			string _featureName			= gameObject.name;
			m_featureInterfaceInfoText	= string.Format("NPBinding.{0} object provides the interface to access {1} functionalities.",
			                                           _featureName.Replace(" ", string.Empty), _featureName);
		}

		#endregion

		#region GUI Methods

		protected override void OnGUIWindow ()
		{
			
		}
		
		protected bool DisplayThingsToKnow ()
		{
			
			
			return false;
		}
		
		protected virtual void DisplayFeatureFunctionalities ()
		{}

		#endregion

		#region Methods

		protected void AddExtraInfoTexts (params string[] _infoTexts)
		{
			if (_infoTexts == null)
				return;

			m_additionalInfoTexts	= _infoTexts;
		}

		protected void SetFeatureInterfaceInfoText (string _newText)
		{
			m_featureInterfaceInfoText	= _newText;
		}

		#endregion
	}
}