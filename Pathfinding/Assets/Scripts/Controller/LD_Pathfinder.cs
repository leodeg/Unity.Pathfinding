using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_Pathfinder : MonoBehaviour
	{
		[SerializeField] private Color m_StartColor = Color.green;
		[SerializeField] private Color m_EndColor = Color.red;
		[SerializeField] private Color m_FrontierColor = Color.magenta;
		[SerializeField] private Color m_ExploredColor = Color.gray;
		[SerializeField] private Color m_PathColor = Color.cyan;
		private LD_Node m_StartNode;
		private LD_Node m_EndNode;
		private LD_Graph m_Graph;
		private LD_GraphView m_GraphView;
		private Queue<LD_Node> m_FrontierNodes;
		private List<LD_Node> m_ExploredNodes;
		private List<LD_Node> m_PathNodes;

		public void Init (LD_Graph graph, LD_GraphView graphView, LD_Node start, LD_Node end)
		{
			if (start == null || end == null || graph == null || graphView == null)
			{
				Debug.LogError ("LD_Pathfinder::Init::ArgumentNullException.");
				throw new System.ArgumentNullException ();
			}

			if (start.NodeType == LD_NodeType.Blocked || end.NodeType == LD_NodeType.Blocked)
			{
				Debug.LogError ("LD_Pathfinder::Init::Start and End nodes must be unlocked.");
				throw new System.InvalidOperationException ("Start and End nodes must be unlocked.");
			}

			m_Graph = graph;
			m_GraphView = graphView;
			m_StartNode = start;
			m_EndNode = end;

			LD_NodeView startView = m_GraphView.NodeViews[start.XIndex, start.YIndex];
			if (startView != null)
			{
				startView.SetColor (m_StartColor);
			}

			LD_NodeView endView = m_GraphView.NodeViews[end.XIndex, end.YIndex];
			if (endView != null)
			{
				endView.SetColor (m_EndColor);
			}

			m_FrontierNodes = new Queue<LD_Node> ();
			m_FrontierNodes.Enqueue (m_StartNode);
			m_ExploredNodes = new List<LD_Node> ();
			m_PathNodes = new List<LD_Node> ();

			for (int x = 0; x < m_Graph.Width; x++)
			{
				for (int y = 0; y < m_Graph.Height; y++)
				{
					m_Graph.Nodes[x, y].Reset ();
				}
			}
		}
	}
}