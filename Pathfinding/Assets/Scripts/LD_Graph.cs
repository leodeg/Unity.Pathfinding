using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_Graph : MonoBehaviour
	{
		public LD_Node[,] m_Nodes;
		public List<LD_Node> m_Walls = new List<LD_Node> ();

		private int[,] m_MapData;
		private int m_Width;
		private int m_Height;

		public static readonly Vector2[] m_Directions =
		{
			new Vector2(0f,1f),
			new Vector2(1f,1f),
			new Vector2(1f,0f),
			new Vector2(1f,-1f),
			new Vector2(0f,-1f),
			new Vector2(-1f,-1f),
			new Vector2(-1f,0f),
			new Vector2(-1f,1f)
		};

		public void Init (int[,] mapData)
		{
			m_MapData = mapData;
			m_Width = mapData.GetLength (0);
			m_Height = mapData.GetLength (1);

			m_Nodes = new LD_Node[m_Width, m_Height];

			// Create nodes and walls
			for (int y = 0; y < m_Height; y++)
			{
				for (int x = 0; x < m_Width; x++)
				{
					NodeType type = (NodeType)mapData[x, y];
					LD_Node newNode = new LD_Node (x, y, type);

					m_Nodes[x, y] = newNode;
					newNode.m_Position = new Vector3 (x, 0, y);

					if (type == NodeType.Blocked)
					{
						m_Walls.Add (newNode);
					}
				}
			}

			// Link neighbors
			for (int y = 0; y < m_Height; y++)
			{
				for (int x = 0; x < m_Width; x++)
				{
					if (m_Nodes[x, y].m_NodeType != NodeType.Blocked)
					{
						m_Nodes[x, y].m_Neighbors = GetNeighbors (x, y);
					}
				}
			}
		}

		public bool IsWithinBounds (int x, int y)
		{
			return ( x >= 0 ) && ( x < m_Width )
				&& ( y >= 0 ) && ( y < m_Height );
		}

		private List<LD_Node> GetNeighbors (int x, int y, LD_Node[,] nodeArray, Vector2[] directions)
		{
			List<LD_Node> neighborNodes = new List<LD_Node> ();

			foreach (Vector2 direction in directions)
			{
				int newX = x + (int)direction.x;
				int newY = y + (int)direction.y;

				if (IsWithinBounds (newX, newY)
					&& nodeArray[newX, newY] != null
					&& nodeArray[newX, newY].m_NodeType != NodeType.Blocked)
				{
					neighborNodes.Add (nodeArray[newX, newY]);
				}
			}

			return neighborNodes;
		}

		private List<LD_Node> GetNeighbors (int x, int y)
		{
			return GetNeighbors (x, y, m_Nodes, m_Directions);
		}
	}
}