using UnityEngine;
using System.Collections;

namespace LD.PathFinding
{
	public class LD_GameController : MonoBehaviour
	{
		[SerializeField] private LD_MapData m_MapData;
		[SerializeField] private LD_Graph m_Graph;

		private void Start ()
		{
			if (m_MapData != null && m_Graph != null)
			{
				int[,] mapInstance = m_MapData.MakeMap ();
				m_Graph.Init (mapInstance);
				LD_GraphView graphView = m_Graph.gameObject.GetComponent<LD_GraphView> ();

				if (graphView != null)
				{
					graphView.Init (m_Graph);
				}
			}
		}
	}
}