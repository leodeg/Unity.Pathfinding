using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD.PathFinding
{
	internal class LD_MapData : MonoBehaviour
	{
		[Header ("Base Properties")]
		[SerializeField] private int m_Width = 10;
		[SerializeField] private int m_Height = 10;

		[Header ("Text Map Asset")]
		[SerializeField] private TextAsset m_TextMapAsset;
		[SerializeField] private string m_ResourcesPath = "MapData";
		[SerializeField] private string m_FileName = "Maze_01";

		[Header ("Image Map Asset")]
		[SerializeField] private Texture2D m_TextureMapAsset;

		[Header ("Colors")]
		[SerializeField] private Color32 m_OpenColor = Color.white;
		[SerializeField] private Color32 m_BlockedColor = Color.black;
		[SerializeField] private Color32 m_LightColor = Color.blue;
		[SerializeField] private Color32 m_MediumColor = Color.yellow;
		[SerializeField] private Color32 m_HeavyColor = Color.red;

		private static Dictionary<Color32, LD_NodeType> m_TerrainLookupTable;


		private void Awake ()
		{
			SetupLookupTable ();
		}
		private void Start ()
		{

			string levelName = SceneManager.GetActiveScene ().name;
			string path = m_ResourcesPath + "/" + levelName;

			if (m_TextureMapAsset == null)
			{
				m_TextureMapAsset = Resources.Load (path) as Texture2D;
				if (m_TextureMapAsset == null)
				{
					Debug.LogWarning ("LD_MapData::Cannot find texture asset (" + path + ")");
				}
			}

			if (m_TextMapAsset == null)
			{
				m_TextMapAsset = Resources.Load (path) as TextAsset;
				if (m_TextureMapAsset == null)
				{
					Debug.LogWarning ("LD_MapData::Cannot find text asset (" + path + ")");
				}
			}
		}

		public int[,] CreateMap ()
		{
			if (m_TextMapAsset == null && m_TextureMapAsset == null)
			{
				Debug.LogError ("LD_MapData::CreateMap:: map assets is not assign.");
				return null;
			}

			List<string> lines = new List<string> ();

			if (m_TextureMapAsset != null)
			{
				lines = GetMapFromTexture (m_TextureMapAsset);
			}
			else
			{
				lines = GetMapFromFile ();
			}

			SetDimensions (lines);

			int[,] map = new int[m_Width, m_Height];

			for (int y = 0; y < m_Height; y++)
			{
				for (int x = 0; x < m_Width; x++)
				{
					if (lines[y].Length > x)
					{
						map[x, y] = (int)char.GetNumericValue (lines[y][x]);
					}
				}
			}

			return map;
		}
		private void SetupLookupTable ()
		{
			m_TerrainLookupTable = new Dictionary<Color32, LD_NodeType> ();
			m_TerrainLookupTable.Add (m_OpenColor, LD_NodeType.Open);
			m_TerrainLookupTable.Add (m_BlockedColor, LD_NodeType.Blocked);
			m_TerrainLookupTable.Add (m_LightColor, LD_NodeType.LightTerrain);
			m_TerrainLookupTable.Add (m_MediumColor, LD_NodeType.MediumTerrain);
			m_TerrainLookupTable.Add (m_HeavyColor, LD_NodeType.HeavyTerrain);
		}
		public void SetDimensions (List<string> textLines)
		{
			m_Height = textLines.Count;
			foreach (string line in textLines)
			{
				if (line.Length > m_Width)
				{
					m_Width = line.Length;
				}
			}
		}

		public List<string> GetMapFromFile (TextAsset textMaze)
		{
			if (textMaze == null)
			{
				Debug.LogWarning ("LD_MapData::GetTextFromFile::TextMaze::Invalid TextMaze asset.");
				return null;
			}

			List<string> lines = new List<string> ();
			string textData = m_TextMapAsset.text;
			string[] delimiters = { "\r\n", "\n" };
			lines.AddRange (textData.Split (delimiters, System.StringSplitOptions.None));

			return lines;
		}
		public List<string> GetMapFromFile ()
		{
			return GetMapFromFile (m_TextMapAsset);
		}
		public List<string> GetMapFromTexture (Texture2D texture)
		{
			if (m_TextureMapAsset == null)
			{
				Debug.LogError ("LD_MapData::GetMapFormTexture::ArgumentNullException().");
				throw new System.ArgumentNullException ();
			}

			List<string> lines = new List<string> ();

			for (int y = 0; y < texture.height; y++)
			{
				string newLine = string.Empty;
				for (int x = 0; x < texture.width; x++)
				{
					Color pixelColor = texture.GetPixel (x, y);
					if (m_TerrainLookupTable.ContainsKey (pixelColor))
					{
						LD_NodeType nodeType = m_TerrainLookupTable[pixelColor];
						int nodeTypeEnum = (int)nodeType;
						newLine += nodeTypeEnum;
					}
					else
					{
						newLine += '0';
					}
				}
				lines.Add (newLine);
			}
			return lines;
		}
		public static Color GetColorFromNodeType (LD_NodeType nodeType)
		{
			if (m_TerrainLookupTable.ContainsValue(nodeType))
			{
				Color colorKey = m_TerrainLookupTable.FirstOrDefault (x => x.Value == nodeType).Key;
				return colorKey;
			}
			return Color.white;
		}
	}
}