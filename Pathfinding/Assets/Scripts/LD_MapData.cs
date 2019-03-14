using UnityEngine;
using System.Collections;

namespace LD.PathFinding
{
	internal class LD_MapData : MonoBehaviour
	{
		[SerializeField] private int m_Width = 10;
		[SerializeField] private int m_Height = 10;

		public int[,] MakeMap ()
		{
			int[,] map = new int[m_Width, m_Height];

			for (int y = 0; y < m_Height; y++)
			{
				for (int x = 0; x < m_Width; x++)
				{
					map[x, y] = 0;
				}
			}

			map[1, 0] = 1;
			map[1, 1] = 1;
			map[1, 2] = 1;
			map[3, 2] = 1;
			map[3, 3] = 1;
			map[3, 4] = 1;
			map[4, 2] = 1;
			map[5, 1] = 1;
			map[5, 2] = 1;
			map[6, 2] = 1;
			map[6, 3] = 1;
			map[8, 0] = 1;
			map[8, 1] = 1;
			map[8, 2] = 1;
			map[8, 4] = 1;

			return map;
		}
	}
}