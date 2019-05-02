using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GridMaster;
using System;

namespace Pathfinding
{
    public class Pathfinder
    {
        #region Variables

        GridBase grid;
        public Node startPosition;
        public Node endPosition;

        public volatile bool jobDone;
        private PathfindingMaster.JobComplete completeCallback;
        List<Node> completePath;

        #endregion

        public Pathfinder (Node start, Node target, PathfindingMaster.JobComplete callback)
        {
            startPosition = start;
            endPosition = target;
            completeCallback = callback;
            grid = GridBase.Singleton;
        }

        public void NotifyComplete ()
        {
            if (completeCallback != null)
            {
                completeCallback (completePath);
            }
        }

        #region Find Path Methods

        public void FindPath ()
        {
            completePath = FindPath (startPosition, endPosition);
            if (completePath.Count > 0)
            {
                //NotifyComplete ();
                jobDone = true;
            }
            else
            {
                Debug.Log ("Complete Path Count: [" + completePath.Count + "]");
            }
        }

        public List<Node> FindPath (Node start, Node end)
        {
            Debug.Log ("FIND PATH");
            List<Node> path = new List<Node> ();
            List<Node> openSet = new List<Node> ();
            HashSet<Node> closedSet = new HashSet<Node> ();

            openSet.Add (start);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].TotalCost < currentNode.TotalCost ||
                        (openSet[i].TotalCost == currentNode.TotalCost &&
                        openSet[i].distanceCost < currentNode.distanceCost))
                    {
                        if (!currentNode.Equals (openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }

                openSet.Remove (currentNode);
                closedSet.Add (currentNode);

                if (currentNode.Equals (end))
                {
                    path = RetracePath (start, currentNode);
                    break;
                }

                Debug.Log ("FIND NEIGHBOURS");
                List<Node> currentNeighbours = GetNeighbours (currentNode, false);

                if (currentNeighbours == null || currentNeighbours.Count == 0)
                {
                    Debug.Log ("CURRENT NEIGHBOURS IS NULL");
                    Debug.Log ("Current Neighbours Count: [" + currentNeighbours.Count + "]");
                }
                else
                {
                    foreach (Node neighbour in currentNeighbours)
                    {
                        if (!closedSet.Contains (neighbour))
                        {
                            float costToNeighbour = currentNode.neighbourCost + GetDistance (currentNode, neighbour);

                            if (costToNeighbour < neighbour.neighbourCost || !openSet.Contains (neighbour))
                            {
                                neighbour.neighbourCost = costToNeighbour;
                                neighbour.distanceCost = GetDistance (neighbour, end);
                                neighbour.parentNode = currentNode;

                                if (!openSet.Contains (neighbour))
                                {
                                    openSet.Add (neighbour);
                                }
                            }
                        }
                    }
                }

            }

            return path;
        }

        private List<Node> RetracePath (Node startNode, Node endNode)
        {
            Debug.Log ("RETRACE PATH");
            List<Node> path = new List<Node> ();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add (currentNode);
                currentNode = currentNode.parentNode;
            }

            path.Reverse ();
            return path;
        }

        #endregion

        #region Get Node Methods

        private List<Node> GetNeighbours (Node node, bool getTopAndDownNeighbours = false)
        {
            Debug.Log ("GET NEIGHBOURS");
            List<Node> neighbours = new List<Node> ();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        int currentYIndex = y;
                        if (!getTopAndDownNeighbours)
                        {
                            currentYIndex = 0;
                        }

                        // If not current node
                        if (x != 0 && currentYIndex != 0 && z != 0)
                        {
                            Node currentSearchPosition = new Node ();
                            currentSearchPosition.x = node.x + x;
                            currentSearchPosition.y = node.y + currentYIndex;
                            currentSearchPosition.z = node.z + z;

                            Node neighbourNode = GetNeighbourNode (currentSearchPosition, false, node);

                            if (neighbourNode != null)
                                neighbours.Add (neighbourNode);
                            else Debug.Log ("NEIGHBOUR IS NULL");
                        }
                    }
                }
            }

            return neighbours;
        }

        private Node GetNeighbourNode (Node currentPosition, bool searchTopDown, Node baseNode)
        {
            Debug.Log ("GET NEIGHBOUR NODE");
            Node nodeToReturn = null;
            Node node = GetNode (currentPosition.x, currentPosition.y, currentPosition.z);

            // Search Top-Down
            if (node != null && node.isWalkable)
            {
                nodeToReturn = node;
            }
            else if (searchTopDown)
            {
                currentPosition.y -= 1; // Look at bottom block
                Node bottomNode = GetNode (currentPosition.x, currentPosition.y, currentPosition.z);
                if (bottomNode != null && bottomNode.isWalkable)
                    nodeToReturn = bottomNode;
                else
                {
                    currentPosition.y += 2; // Look at top block
                    Node topNode = GetNode (currentPosition.x, currentPosition.y, currentPosition.z);
                    if (topNode != null && topNode.isWalkable)
                        nodeToReturn = topNode;
                }
            }

            // Search diagonal
            int originalX = currentPosition.x - baseNode.x;
            int originalZ = currentPosition.z - baseNode.z;

            if (Mathf.Abs (originalX) == 1 && Mathf.Abs (originalZ) == 1)
            {
                Node xNeighbour = GetNode (baseNode.x + originalX, baseNode.y, baseNode.z);
                if (xNeighbour == null || !xNeighbour.isWalkable)
                    nodeToReturn = null;

                Node zNeighbour = GetNode (baseNode.x, baseNode.y, baseNode.z + originalZ);
                if (zNeighbour == null || !zNeighbour.isWalkable)
                    nodeToReturn = null;
            }

            return nodeToReturn;
        }

        private Node GetNode (int x, int y, int z)
        {
            Node node = null;

            lock (grid)
            {
                node = grid.GetNode (x, y, z);
            }

            return node;
        }

        private int GetDistance (Node a, Node b)
        {
            int distanceX = Mathf.Abs (a.x - b.x);
            int distanceY = Mathf.Abs (a.y - b.y);
            int distanceZ = Mathf.Abs (a.z - b.z);

            if (distanceX > distanceZ)
            {
                return 14 * distanceZ + 10 * (distanceX - distanceZ) + 10 * distanceY;
            }

            return 14 * distanceX + 10 * (distanceZ - distanceX) + 10 * distanceY;
        }

        #endregion
    }
}