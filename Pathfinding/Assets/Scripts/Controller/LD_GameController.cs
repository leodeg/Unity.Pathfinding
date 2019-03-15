using System;
using System.Collections;
using UnityEngine;

namespace LD.PathFinding
{
	public class LD_GameController : MonoBehaviour
	{
		[Header ("References")]
		[SerializeField] private LD_MapData m_MapData;
		[SerializeField] private LD_Graph m_Graph;
		[SerializeField] private LD_Pathfinder m_Pathfinder;

		[Header ("Start Positions")]
		[SerializeField] private int m_StartX;
		[SerializeField] private int m_StartY;
		[SerializeField] private int m_EndX;
		[SerializeField] private int m_EndY;

		[Header ("Search Properties")]
		[SerializeField] private float m_TimeStep = 0.1f;

		private void Start ()
		{
			if (m_MapData != null && m_Graph != null)
			{
				int[,] mapInstance = m_MapData.CreateMap ();
				m_Graph.Init (mapInstance);
				LD_GraphView graphView = m_Graph.gameObject.GetComponent<LD_GraphView> ();

				if (graphView != null)
				{
					graphView.Init (m_Graph);
				}

				if (m_Graph.IsWithinBounds (m_StartX, m_StartY)
					|| m_Graph.IsWithinBounds (m_EndX, m_EndY))
				{
					if (m_Pathfinder != null)
					{
						LD_Node startNode = m_Graph.Nodes[m_StartX, m_StartY];
						LD_Node endNode = m_Graph.Nodes[m_EndX, m_EndY];
						m_Pathfinder.Init (m_Graph, graphView, startNode, endNode);
						StartCoroutine (m_Pathfinder.SearchRoutine(m_TimeStep));
					}
					else
					{
						Debug.LogWarning ("LD_GameController::Start:: Pathfinder is not assign.");
					}
				}
				else
				{
					Debug.LogWarning ("LD_GameController::Start:: The start or the end node out of map size range.");
				}
			}

		}


	}
}