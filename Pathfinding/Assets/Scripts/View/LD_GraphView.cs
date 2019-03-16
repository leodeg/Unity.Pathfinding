using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	[RequireComponent (typeof (LD_Graph))]
	internal class LD_GraphView : MonoBehaviour
	{
		[SerializeField] private GameObject m_NodeViewPrefab;
		[SerializeField] private LD_NodeView[,] m_NodeViews;

		public void Initialize (LD_Graph graph)
		{
			if (graph == null)
			{
				Debug.LogWarning ("LD_GraphView:: no graph to initialize!");
				return;
			}

			m_NodeViews = new LD_NodeView[graph.Width, graph.Height];

			foreach (LD_Node node in graph.Nodes)
			{
				GameObject instance = Instantiate (m_NodeViewPrefab, Vector3.zero, Quaternion.identity);
				LD_NodeView nodeView = instance.GetComponent<LD_NodeView> ();

				if (nodeView != null)
				{
					nodeView.Init (node);
					m_NodeViews[node.XIndex, node.YIndex] = nodeView;

					Color originalColor = LD_MapData.GetColorFromNodeType (node.GetNodeType());
					nodeView.SetColor (originalColor);
				}
			}
		}

		public LD_NodeView GetNodeView (int x, int y)
		{
			return m_NodeViews[x, y];
		}

		public void SetColors (List<LD_Node> nodes, Color color, bool lerpColor = false, float lerpValue = 0.5f)
		{
			foreach (LD_Node node in nodes)
			{
				if (node != null)
				{
					LD_NodeView nodeView = m_NodeViews[node.XIndex, node.YIndex];
					Color targetColor = color;

					if (lerpColor)
					{
						Color originalColor = LD_MapData.GetColorFromNodeType (node.GetNodeType());
						targetColor = Color.Lerp (originalColor, targetColor, lerpValue);
					}

					if (nodeView != null)
					{
						nodeView.SetColor (targetColor);
					}
				}
			}
		}
		public void ShowArrows (LD_Node node, Color color)
		{
			if (node != null)
			{
				LD_NodeView nodeView = m_NodeViews[node.XIndex, node.YIndex];
				if (nodeView != null)
				{
					nodeView.ShowArrow (color);
				}
			}
		}
		public void ShowArrows (List<LD_Node> nodes, Color color)
		{
			foreach (LD_Node node in nodes)
			{
				ShowArrows (node, color);
			}
		}
	}
}