using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_Graph : MonoBehaviour
	{
		#region Variables

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

		#endregion

		#region Properties

		public int Width { get { return m_Width; } }
		public int Height { get { return m_Height; } }

		public LD_Node[,] Nodes { get; set; }
		public List<LD_Node> Walls { get; set; } = new List<LD_Node> ();

		#endregion

		#region Initialization

		public void Init (int[,] mapData)
		{
			m_MapData = mapData;
			m_Width = mapData.GetLength (0);
			m_Height = mapData.GetLength (1);

			Nodes = new LD_Node[m_Width, m_Height];

			// Create nodes and walls
			for (int y = 0; y < m_Height; y++)
			{
				for (int x = 0; x < m_Width; x++)
				{
					LD_NodeType type = (LD_NodeType)mapData[x, y];
					LD_Node newNode = new LD_Node (x, y, type);

					Nodes[x, y] = newNode;
					newNode.Position = new Vector3 (x, 0, y);

					if (type == LD_NodeType.Blocked)
					{
						Walls.Add (newNode);
					}
				}
			}

			// Link neighbors
			for (int y = 0; y < m_Height; y++)
			{
				for (int x = 0; x < m_Width; x++)
				{
					if (Nodes[x, y].NodeType != LD_NodeType.Blocked)
					{
						Nodes[x, y].Neighbors = GetNeighbors (x, y);
					}
				}
			}
		}

		#endregion

		#region Graph Methods

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
					&& nodeArray[newX, newY].NodeType != LD_NodeType.Blocked)
				{
					neighborNodes.Add (nodeArray[newX, newY]);
				}
			}

			return neighborNodes;
		}

		private List<LD_Node> GetNeighbors (int x, int y)
		{
			return GetNeighbors (x, y, Nodes, m_Directions);
		}

		#endregion
	}
}