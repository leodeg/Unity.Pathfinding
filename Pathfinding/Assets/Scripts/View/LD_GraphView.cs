using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	[RequireComponent (typeof (LD_Graph))]
	internal class LD_GraphView : MonoBehaviour
	{
		[SerializeField] private GameObject nodeViewPrefab;
		[SerializeField] private LD_NodeView[,] nodeViews;
		[SerializeField] private Color openColor = Color.white;
		[SerializeField] private Color blockedColor = Color.black;

		public GameObject NodeViewPrefab { get => nodeViewPrefab; set => nodeViewPrefab = value; }
		internal LD_NodeView[,] NodeViews { get => nodeViews; set => nodeViews = value; }
		public Color OpenColor { get => openColor; set => openColor = value; }
		public Color BlockedColor { get => blockedColor; set => blockedColor = value; }

		public void Init (LD_Graph graph)
		{
			if (graph == null)
			{
				Debug.LogWarning ("LD_GraphView:: no graph to initialize!");
				return;
			}

			NodeViews = new LD_NodeView[graph.Width, graph.Height];

			foreach (LD_Node node in graph.Nodes)
			{
				GameObject instance = Instantiate (NodeViewPrefab, Vector3.zero, Quaternion.identity);
				LD_NodeView nodeView = instance.GetComponent<LD_NodeView> ();

				if (nodeView != null)
				{
					nodeView.Init (node);
					NodeViews[node.XIndex, node.YIndex] = nodeView;

					if (node.NodeType == LD_NodeType.Blocked)
					{
						nodeView.SetColor (BlockedColor);
					}
					else
					{
						nodeView.SetColor (OpenColor);
					}
				}
			}
		}

		public void SetupColors (List<LD_Node> nodes, Color color)
		{
			foreach (LD_Node node in nodes)
			{
				if (node != null)
				{
					LD_NodeView nodeView = NodeViews[node.XIndex, node.YIndex];

					if (nodeView != null)
					{
						nodeView.SetColor (color);
					}
				}
			}
		}
	}
}