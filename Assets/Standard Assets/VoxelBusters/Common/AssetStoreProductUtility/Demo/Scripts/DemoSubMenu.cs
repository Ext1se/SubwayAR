using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.UASUtils.Demo
{
	public class DemoSubMenu : DemoGUIWindow 
	{
		#region Properties
	
		private List<string>			m_results			= new List<string>();
		
		//Misc
		private GUIScrollView 			m_resultsScrollView;
	
		#endregion
		
		#region Unity Methods
	
		protected override void Start()
		{
			base.Start();
	
			//For drawing results in scrollview
			//m_resultsScrollView  =  gameObject.AddComponent<GUIScrollView>();
		}

		#endregion
	
		#region Methods
	
		protected virtual void DrawPopButton (string _popTitle = "Back")
		{	
			if (GUILayout.Button(_popTitle))
			{
				gameObject.SetActive(false);
			}	
		}
	
		protected override void OnGUIWindow ()
		{
			base.OnGUIWindow ();
			GUILayout.Box(name);
		}
		
		#endregion
	
		#region For Displaying and Tracking Results
	
		protected void AppendResult(string _result)
		{
			m_results.Add(_result);
		}
		
		protected void AddNewResult(string _result)
		{
			m_results.Clear();
			m_results.Add(_result);
		}
		
		protected void DrawResults()
		{
			
		}
	
		#endregion
	}
}