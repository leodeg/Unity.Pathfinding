using UnityEngine;
using System.Collections;

namespace LD.PathFinding
{
	[RequireComponent(typeof(LD_Graph))]
	internal class LD_GraphView : MonoBehaviour
	{
		public GameObject m_NodeViewPrefab;
		public Color m_OpenColor = Color.white;
		public Color m_BlockedColor = Color.black;

		public void Init (LD_Graph graph)
		{
			if (graph == null)
			{
				Debug.LogWarning ("LD_GraphView:: no graph to initialize!");
				return;
			}

			foreach (LD_Node node in graph.m_Nodes)
			{
				GameObject instance = Instantiate (m_NodeViewPrefab, Vector3.zero, Quaternion.identity);
				LD_NodeView nodeView = instance.GetComponent<LD_NodeView> ();

				if (nodeView != null)
				{
					nodeView.Init (node);

					if (node.m_NodeType == NodeType.Blocked)
					{
						nodeView.ColorNode (m_BlockedColor);
					}
					else
					{
						nodeView.ColorNode (m_OpenColor);
					}
				}
			}
		}
	}
}