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
		[SerializeField] private float m_SearchSpeed = 0.1f;

		private void Start ()
		{
			if (m_MapData == null && m_Graph == null)
			{
				Debug.LogError ("LD_GameController::Start:: Map data or Graph data is null.");
				return;
			}

			if (m_Pathfinder == null)
			{
				Debug.LogError ("LD_GameController::Start:: Pathfinder is not assign.");
				return;
			}

			if (m_StartX == m_EndX && m_StartY == m_EndY)
			{
				Debug.LogError ("LD_GameController::Start:: Start and end nodes is on the same position.");
				return;
			}

			if (m_Graph.IsInTheMapRange (m_StartX, m_StartY)
				|| m_Graph.IsInTheMapRange (m_EndX, m_EndY))
			{
				Debug.LogWarning ("LD_GameController::Start:: Start or end node is out of map range.");
				return;
			}

			int[,] mapData = m_MapData.CreateMap ();
			m_Graph.Initialize (mapData);

			LD_GraphView graphView = m_Graph.gameObject.GetComponent<LD_GraphView> ();
			if (graphView != null)
			{
				graphView.Initialize (m_Graph);

				LD_Node startNode = m_Graph.GetNode (m_StartX, m_StartY);
				LD_Node endNode = m_Graph.GetNode (m_EndX, m_EndY);

				m_Pathfinder.Initialize (m_Graph, graphView, startNode, endNode);
				StartCoroutine (m_Pathfinder.StartSearch (m_SearchSpeed));
			}
			else
			{
				Debug.LogWarning ("LD_GameController::Start:: Cannot find GraphView class.");
			}
		}
	}
}