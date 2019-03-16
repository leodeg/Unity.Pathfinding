using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_Node : IComparable<LD_Node>
	{
		private int m_yIndex;
		private int m_xIndex;
		private LD_NodeType m_NodeType;
		private List<LD_Node> m_Neighbors;

		public int YIndex { get { return m_yIndex; } }
		public int XIndex { get { return m_xIndex; } }

		public int Priority { get; set; }
		public Vector3 Position { get; set; }
		public LD_Node Previous { get; set; }
		public float DistanceTraveled { get; set; }

		public LD_Node (int xIndex, int yIndex, LD_NodeType type)
		{
			m_Neighbors = new List<LD_Node> ();
			DistanceTraveled = Mathf.Infinity;
			m_xIndex = xIndex;
			m_yIndex = yIndex;
			m_NodeType = type;
		}

		public LD_NodeType GetNodeType ()
		{
			return m_NodeType;
		}

		public LD_Node GetNeighbor (int index)
		{
			return m_Neighbors[index];
		}

		public int GetNeighborsCount ()
		{
			return m_Neighbors.Count;
		}

		public void SetNeighbor (LD_Node node)
		{
			m_Neighbors.Add (node);
		}

		public void SetNeighbors (List<LD_Node> nodes)
		{
			m_Neighbors.AddRange (nodes);
		}

		public void Reset ()
		{
			Previous = null;
		}

		public int CompareTo (LD_Node other)
		{
			if (this.Priority < other.Priority) return -1;
			else if (this.Priority > other.Priority) return 1;
			else return 0;
		}
	}
}