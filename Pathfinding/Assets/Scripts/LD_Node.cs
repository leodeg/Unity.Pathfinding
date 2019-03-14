using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	internal enum NodeType
	{
		Open = 0,
		Blocked = 1
	}

	internal class LD_Node
	{
		public int m_XIndex = -1;
		public int m_YIndex = -1;
		public Vector3 m_Position;

		public NodeType m_NodeType = NodeType.Open;
		public List<LD_Node> m_Neighbors = new List<LD_Node> ();
		public LD_Node m_Previous = null;


		public LD_Node (int xIndex, int yIndex, NodeType type)
		{
			m_XIndex = xIndex;
			m_YIndex = yIndex;
			m_NodeType = type;
		}

		public void Reset ()
		{
			m_Previous = null;
		}
	}
}