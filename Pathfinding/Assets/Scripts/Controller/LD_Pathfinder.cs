using System;
using System.Collections.Generic;
using System.Linq;
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
		[SerializeField] private Color m_ArrowColor = Color.yellow;
		[SerializeField] private Color m_HighlightColor = Color.red;

		private LD_Node m_StartNode;
		private LD_Node m_EndNode;
		private LD_Graph m_Graph;
		private LD_GraphView m_GraphView;

		private Queue<LD_Node> m_FrontierNodes;
		private List<LD_Node> m_ExploredNodes;
		private List<LD_Node> m_PathNodes;

		private bool m_IsComplete;
		private int m_IterationCount;


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

			ShowColors (m_GraphView, start, end);

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

			m_IsComplete = false;
			m_IterationCount = 0;
		}

		private void ShowColors (LD_GraphView graphView, LD_Node start, LD_Node end)
		{
			if (graphView == null || start == null || end == null)
			{
				throw new System.ArgumentNullException ();
			}

			if (m_FrontierNodes != null)
			{
				graphView.SetColorsOf (m_FrontierNodes.ToList (), m_FrontierColor);
			}

			if (m_ExploredNodes != null)
			{
				graphView.SetColorsOf (m_ExploredNodes.ToList (), m_ExploredColor);
			}

			if (m_PathNodes != null && m_PathNodes.Count > 0)
			{
				m_GraphView.SetColorsOf (m_PathNodes, m_PathColor);
			}

			LD_NodeView startView = graphView.NodeViews[start.XIndex, start.YIndex];
			if (startView != null)
			{
				startView.SetColor (m_StartColor);
			}

			LD_NodeView endView = graphView.NodeViews[end.XIndex, end.YIndex];
			if (endView != null)
			{
				endView.SetColor (m_EndColor);
			}
		}

		private void ShowColors ()
		{
			if (m_GraphView == null || m_StartNode == null || m_EndNode == null)
			{
				Debug.LogWarning ("LD_Pathfinder::ShowColors::ArgumentNullException.");
				throw new System.ArgumentNullException ();
			}

			ShowColors (m_GraphView, m_StartNode, m_EndNode);
		}

		public System.Collections.IEnumerator SearchRoutine (float timeStep = 0.1f)
		{
			yield return null;

			while (!m_IsComplete)
			{
				if (m_FrontierNodes.Count > 0)
				{
					LD_Node currentNode = m_FrontierNodes.Dequeue ();
					++m_IterationCount;
					if (!m_ExploredNodes.Contains (currentNode))
					{
						m_ExploredNodes.Add (currentNode);
					}

					ExpandFrontier (currentNode);
					if (m_FrontierNodes.Contains (m_EndNode))
					{
						m_PathNodes = GetPathNodes (m_EndNode);
					}
					ShowColors ();
					if (m_GraphView != null)
					{
						m_GraphView.ShowArrows (m_FrontierNodes.ToList (), m_ArrowColor);
						if (m_FrontierNodes.Contains(m_EndNode))
						{
							m_GraphView.ShowArrows (m_PathNodes, m_HighlightColor);
						}
					}
					yield return new WaitForSeconds (timeStep);
				}
				else
				{
					m_IsComplete = true;
				}
			}
		}

		private List<LD_Node> GetPathNodes (LD_Node end)
		{
			List<LD_Node> path = new List<LD_Node> ();
			if (end == null) return path;

			path.Add (end);
			LD_Node currentNode = end.Previous;
			while (currentNode != null)
			{
				path.Insert (0, currentNode);
				currentNode = currentNode.Previous;
			}

			return path;
		}

		public void ExpandFrontier (LD_Node node)
		{
			if (node != null)
			{
				for (int i = 0; i < node.Neighbors.Count; i++)
				{
					if (!m_ExploredNodes.Contains (node.Neighbors[i])
						&& !m_FrontierNodes.Contains (node.Neighbors[i]))
					{
						node.Neighbors[i].Previous = node;
						m_FrontierNodes.Enqueue (node.Neighbors[i]);
					}
				}
			}
		}
	}
}