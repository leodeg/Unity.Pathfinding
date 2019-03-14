﻿using System.Collections;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_NodeView : MonoBehaviour
	{
		[SerializeField] private GameObject m_Tile;
		[Range (0, 0.5f)]
		[SerializeField] private float m_BorderSize = 0.15f;

		public void Init (LD_Node node)
		{
			if (m_Tile != null)
			{
				gameObject.name = "Node (" + node.m_XIndex + "_" + node.m_YIndex + ")";
				gameObject.transform.position = node.m_Position;
				m_Tile.transform.localScale = new Vector3 (m_Tile.transform.localScale.x - m_BorderSize, 1f, m_Tile.transform.localScale.x - m_BorderSize);
			}
		}

		private void ColorNode (Color color, GameObject gameObj)
		{
			if (gameObj != null)
			{
				Renderer render = gameObj.GetComponent<Renderer> ();

				if (render != null)
				{
					render.material.color = color;
				}
			}
		}

		public void ColorNode (Color color)
		{
			ColorNode (color, m_Tile);
		}
	}
}