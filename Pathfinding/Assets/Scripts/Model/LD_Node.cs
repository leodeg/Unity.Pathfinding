using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_Node
	{
		public int YIndex { get; set; }
		public int XIndex { get; set; }

		public Vector3 Position { get; set; }
		internal LD_Node Previous { get; set ; }
		internal LD_NodeType NodeType { get; set; }
		internal List<LD_Node> Neighbors { get; set; }

		public LD_Node (int xIndex, int yIndex, LD_NodeType type)
		{
			XIndex = xIndex;
			YIndex = yIndex;
			NodeType = type;
		}

		public void Reset ()
		{
			Previous = null;
		}
	}
}