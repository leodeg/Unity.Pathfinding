using UnityEngine;
using System.Collections.Generic;
using System;

namespace GridMaster
{
    public class GridBase : MonoBehaviour
    {
        #region Variables

        [Header ("Grid Properties")]
        public int gridSizeX;
        public int gridSizeY;
        public int gridSizeZ;
        public int offsetX;
        public int offsetY;
        public int offsetZ;

        [Header ("Grid")]
        public Node[,,] grid;

        [Header ("References")]
        public GameObject gridFloorPrefab;

        [Header ("Node Properties")]
        public Vector3 startNodePosition;
        public Vector3 endNodePosition;

        [Header ("Colors")]
        public Color pathColor = Color.cyan;
        public Color startColor = Color.green;
        public Color endColor = Color.red;

        [Header ("Node Properties")]
        public int agentsAmount;

        [Header ("Debug")]
        public bool visualizePath;

        #endregion

        #region Properties

        private static readonly GridBase singleton = new GridBase ();

        public static GridBase Singleton
        {
            get
            {
                return singleton;
            }
        }

        #endregion

        #region Unity Methods

        private void Start ()
        {
            CreateGrid ();
        }

        private void Update ()
        {
            if (visualizePath)
            {
                visualizePath = false;
                UpdatePath ();
            }
        }

        #endregion

        #region Path Methods

        public void UpdatePath ()
        {
            Debug.Log ("START UPDATE PATH");
            Node startNode = GetNode (startNodePosition);
            Node endNode = GetNode (endNodePosition);

            Pathfinding.Pathfinder pathfinder = new Pathfinding.Pathfinder (startNode, endNode, ShowPath);
            List<Node> path = pathfinder.FindPath (startNode, endNode);

            //for (int i = 0; i < agentsAmount; i++)
            //{
            //    Pathfinding.PathfindingMaster.Singleton.RequestPathfind (startNode, endNode, ShowPath);
            //}

            Debug.Log ("Path Count: [" + path.Count + "]");
            if (path != null && path.Count > 0)
            {
                ShowPath (path);
            }

            startNode.prefabObject.GetComponent<Renderer> ().material.color = startColor;
            endNode.prefabObject.GetComponent<Renderer> ().material.color = endColor;
        }

        public void ShowPath (IEnumerable<Node> path)
        {
            Debug.Log ("START SHOWING PATH");
            foreach (Node node in path)
            {
                node.prefabObject.GetComponent<Renderer> ().material.color = pathColor;
            }
            Debug.Log ("STOP SHOWING PATH");
        }

        #endregion

        #region Grid Methods

        public void CreateGrid ()
        {
            if (gridFloorPrefab == null)
            {
                Debug.LogError ("GridBase::ERROR::Floor prefab is null!");
                return;
            }

            if (gridSizeY <= 0)
            {
                gridSizeY = 1;
            }

            grid = new Node[gridSizeX, gridSizeY, gridSizeZ];

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    for (int z = 0; z < gridSizeZ; z++)
                    {
                        CreateGridCell (x, y, z);
                    }
                }
            }
        }

        private void CreateGridCell (int posX, int posY, int posZ)
        {
            Vector3 currentPosition = new Vector3 (posX * offsetX, posY * offsetY, posZ * offsetZ);

            GameObject prefabClone = Instantiate (gridFloorPrefab, currentPosition, Quaternion.identity);
            prefabClone.transform.name = "Cell_" + posX.ToString () + "_" + posY.ToString () + "_" + posZ.ToString ();
            prefabClone.transform.parent = transform;

            Node node = new Node ();
            node.x = posX;
            node.y = posY;
            node.z = posZ;
            node.prefabObject = prefabClone;

            RaycastHit[] hits = Physics.BoxCastAll (currentPosition, new Vector3 (1, 0, 1), Vector3.forward);

            for (int i = 0; i < hits.Length; i++)
            {
                node.isWalkable = false;
            }

            grid[posX, posY, posZ] = node;
        }

        #endregion

        #region Node Methods

        public Node GetNode (int x, int y, int z)
        {
            if (PositionInGridRange (x, y, z))
                return grid[x, y, z];
            return null;
        }

        public Node GetNode (Vector3 position)
        {
            int x = Mathf.RoundToInt (position.x);
            int y = Mathf.RoundToInt (position.y);
            int z = Mathf.RoundToInt (position.z);

            return GetNode (x, y, z);
        }

        private bool PositionInGridRange (int x, int y, int z)
        {
            return x >= 0 && x < gridSizeX &&
                   y >= 0 && y < gridSizeY &&
                   z >= 0 && z < gridSizeZ;
        }

        #endregion
    }
}