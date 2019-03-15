using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_Pathfinder : MonoBehaviour
	{
		public enum SearchMode
		{
			BreadFirstSearch = 0,
			Dijkstra = 1
		}

		[SerializeField] private Color m_StartColor = Color.green;
		[SerializeField] private Color m_EndColor = Color.red;
		[SerializeField] private Color m_FrontierColor = Color.magenta;
		[SerializeField] private Color m_ExploredColor = Color.gray;
		[SerializeField] private Color m_PathColor = Color.cyan;
		[SerializeField] private Color m_ArrowColor = Color.yellow;
		[SerializeField] private Color m_HighlightColor = Color.red;
		[SerializeField] private SearchMode m_SearchMode;

		public bool m_ShowIterations = true;
		public bool m_ShowColors = true;
		public bool m_ShowArrows = true;
		public bool m_StopSearchWhenFindGoalNode = true;

		private LD_Node m_StartNode;
		private LD_Node m_EndNode;
		private LD_Graph m_Graph;
		private LD_GraphView m_GraphView;

		private LD_PriorityQueue<LD_Node> m_FrontierNodes;
		private List<LD_Node> m_ExploredNodes;
		private List<LD_Node> m_PathNodes;

		private bool m_IsComplete;
		private int m_IterationCount;


		public void Initialize (LD_Graph graph, LD_GraphView graphView, LD_Node start, LD_Node end)
		{
			if (start == null || end == null || graph == null || graphView == null)
			{
				Debug.LogError ("LD_Pathfinder::Init::ArgumentNullException()");
				throw new System.ArgumentNullException ();
			}

			if (start.NodeType == LD_NodeType.Blocked || end.NodeType == LD_NodeType.Blocked)
			{
				Debug.LogError ("LD_Pathfinder::Init::The first node or the end node is on blocked cell. The node must be on unlocked cell.");
				throw new System.InvalidOperationException ("The node must be on unlocked cell.");
			}

			m_Graph = graph;
			m_GraphView = graphView;
			m_StartNode = start;
			m_EndNode = end;

			ShowColors (m_GraphView, start, end);

			m_FrontierNodes = new LD_PriorityQueue<LD_Node> ();
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
			m_StartNode.DistanceTraveled = 0;
		}
		public IEnumerator StartSearch (float timeStep = 0f)
		{
			float timeStart = Time.time;

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

					if (m_SearchMode == SearchMode.BreadFirstSearch)
					{
						PathBreadFirstSearch (currentNode);
					}
					else if (m_SearchMode == SearchMode.Dijkstra)
					{
						PathDijkstraSearch (currentNode);
					}

					if (m_FrontierNodes.Contains (m_EndNode))
					{
						m_PathNodes = FindPath (m_EndNode);
						if (m_StopSearchWhenFindGoalNode)
						{
							m_IsComplete = true;
						}
					}

					if (m_ShowIterations)
					{
						ShowSearchDiagnostic ();
					}

					yield return new WaitForSeconds (timeStep);
				}
				else
				{
					m_IsComplete = true;
				}
			}
		}

		private void PathBreadFirstSearch (LD_Node node)
		{
			if (node != null)
			{
				for (int i = 0; i < node.GetNeighborsCount(); i++)
				{
					if (!m_ExploredNodes.Contains (node.GetNeighbor(i))
						&& !m_FrontierNodes.Contains (node.GetNeighbor(i)))
					{
						float distanceToNeighbor = m_Graph.GetDistance (node, node.GetNeighbor (i));
						float distanceTraveled = distanceToNeighbor + node.DistanceTraveled;
						node.GetNeighbor (i).DistanceTraveled = distanceTraveled;

						node.GetNeighbor(i).Previous = node;
						node.GetNeighbor (i).Priority = m_ExploredNodes.Count;
						m_FrontierNodes.Enqueue (node.GetNeighbor(i));
					}
				}
			}
		}
		private void PathDijkstraSearch (LD_Node node)
		{
			if (node != null)
			{
				for (int i = 0; i < node.GetNeighborsCount(); i++)
				{
					if (!m_ExploredNodes.Contains (node.GetNeighbor(i)))
					{
						float distanceToNeighbor = m_Graph.GetDistance (node, node.GetNeighbor (i));
						float distanceTraveled = distanceToNeighbor + node.DistanceTraveled;

						if (float.IsPositiveInfinity (node.GetNeighbor (i).DistanceTraveled)
							|| distanceTraveled < node.GetNeighbor (i).DistanceTraveled)
						{
							node.GetNeighbor (i).Previous = node;
							node.GetNeighbor (i).DistanceTraveled = distanceTraveled;
						}

						if (!m_FrontierNodes.Contains (node.GetNeighbor (i)))
						{
							node.GetNeighbor (i).Priority = (int)node.GetNeighbor (i).DistanceTraveled;
							m_FrontierNodes.Enqueue (node.GetNeighbor(i));
						}
					}
				}
			}
		}

		private void ShowColors ()
		{
			if (m_GraphView == null || m_StartNode == null || m_EndNode == null)
			{
				Debug.LogWarning ("LD_Pathfinder::ShowColors::ArgumentNullException()");
				throw new System.ArgumentNullException ();
			}

			ShowColors (m_GraphView, m_StartNode, m_EndNode);
		}
		private void ShowColors (LD_GraphView graphView, LD_Node start, LD_Node end)
		{
			if (graphView == null || start == null || end == null)
			{
				throw new System.ArgumentNullException ();
			}

			if (m_FrontierNodes != null)
			{
				graphView.SetColors (m_FrontierNodes.ToList (), m_FrontierColor);
			}

			if (m_ExploredNodes != null)
			{
				graphView.SetColors (m_ExploredNodes.ToList (), m_ExploredColor);
			}

			if (m_PathNodes != null && m_PathNodes.Count > 0)
			{
				m_GraphView.SetColors (m_PathNodes, m_PathColor);
			}

			LD_NodeView startView = graphView.GetNodeView(start.XIndex, start.YIndex);
			if (startView != null)
			{
				startView.SetColor (m_StartColor);
			}

			LD_NodeView endView = graphView.GetNodeView(end.XIndex, end.YIndex);
			if (endView != null)
			{
				endView.SetColor (m_EndColor);
			}
		}
		private void ShowSearchDiagnostic ()
		{
			if (m_ShowColors)
			{
				ShowColors ();
			}

			if (m_ShowArrows)
			{
				if (m_GraphView != null)
				{
					m_GraphView.ShowArrows (m_FrontierNodes.ToList (), m_ArrowColor);
					if (m_FrontierNodes.Contains (m_EndNode))
					{
						m_GraphView.ShowArrows (m_PathNodes, m_HighlightColor);
					}
				}
			}
		}
		private List<LD_Node> FindPath (LD_Node end)
		{
			List<LD_Node> path = new List<LD_Node> ();

			if (end == null)
			{
				return path;
			}

			path.Add (end);
			LD_Node currentNode = end.Previous;

			while (currentNode != null)
			{
				path.Insert (0, currentNode);
				currentNode = currentNode.Previous;
			}

			return path;
		}
	}
}