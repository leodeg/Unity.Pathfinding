using System.Collections;
using UnityEngine;

namespace LD.PathFinding
{
	internal class LD_NodeView : MonoBehaviour
	{
		[Header ("References")]
		[SerializeField] private GameObject m_Tile;
		[SerializeField] private GameObject m_Arrow;
		[SerializeField] private LD_Node m_Node;

		[Header ("Properties")]
		[SerializeField]
		[Range (0, 0.5f)] private float m_BorderSize = 0.15f;

		public void Init (LD_Node node)
		{
			if (m_Tile != null)
			{
				gameObject.name = "Node (" + node.XIndex + "_" + node.YIndex + ")";
				gameObject.transform.position = node.Position;
				m_Tile.transform.localScale = new Vector3 (m_Tile.transform.localScale.x - m_BorderSize, 1f, m_Tile.transform.localScale.x - m_BorderSize);

				m_Node = node;
				EnableObject (m_Arrow, false);
			}
		}

		private void SetColor (Color color, GameObject gameObj)
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

		public void SetColor (Color color)
		{
			SetColor (color, m_Tile);
		}

		private void EnableObject (GameObject obj, bool state)
		{
			if (obj != null)
			{
				obj.SetActive (state);
			}
		}

		public void ShowArrow (Color color)
		{
			if (m_Node != null && m_Arrow != null && m_Node.Previous != null)
			{
				EnableObject (m_Arrow, true);

				Vector3 directionToPreviousNode = (m_Node.Previous.Position - m_Node.Position).normalized;
				m_Arrow.transform.rotation = Quaternion.LookRotation (directionToPreviousNode);

				Renderer arrowRenderer = m_Arrow.GetComponent<Renderer> ();
				if (arrowRenderer != null)
				{
					arrowRenderer.material.color = color;
				}
			}
		}
	}
}