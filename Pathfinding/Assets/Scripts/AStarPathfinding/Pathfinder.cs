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
        List<Node> foundPath;

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
                completeCallback (foundPath);
            }
        }

        #region Find Path Methods

        public void FindPath ()
        {
            foundPath = FindPath (startPosition, endPosition);
            jobDone = true;
        }


        private List<Node> FindPath (Node start, Node end)
        {
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
                        openSet[i].heightCost < currentNode.heightCost))
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

                foreach (Node neighbour in GetNeighbours (currentNode, true))
                {
                    if (!closedSet.Contains (neighbour))
                    {
                        float costToNeighbour = currentNode.gridCost + GetDistance (currentNode, neighbour);

                        if (costToNeighbour < neighbour.gridCost ||
                            !openSet.Contains (neighbour))
                        {
                            neighbour.gridCost = costToNeighbour;
                            neighbour.heightCost = GetDistance (neighbour, end);
                            neighbour.parentNode = currentNode;

                            if (!openSet.Contains (neighbour))
                            {
                                openSet.Add (neighbour);
                            }
                        }
                    }
                }
            }

            return path;
        }

        private List<Node> RetracePath (Node startNode, Node endNode)
        {
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

        private List<Node> GetNeighbours (Node node, bool getVertivalNeighbours = false)
        {
            List<Node> list = new List<Node> ();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        int yIndex = y;
                        if (!getVertivalNeighbours) yIndex = 0;

                        // If not current node
                        if (x != 0 && y != 0 && z != 0)
                        {
                            Node searchPosition = new Node ();
                            searchPosition.x = node.x + x;
                            searchPosition.y = node.y + y;
                            searchPosition.z = node.z + z;

                            Node newNode = GetNeighbourNode (searchPosition, true, node);

                            if (newNode != null)
                                list.Add (newNode);
                        }
                    }
                }
            }

            return list;
        }

        private Node GetNeighbourNode (Node offsetPos, bool searchTopDown, Node currentNodePos)
        {
            Node returnValue = null;
            Node node = GetNode (offsetPos.x, offsetPos.y, offsetPos.z);

            // Search Top-Down
            if (node != null && node.isWalkable)
            {
                returnValue = node;
            }
            else if (searchTopDown)
            {
                offsetPos.y -= 1; // Look at bottom block
                Node bottomBlock = GetNode (offsetPos.x, offsetPos.y, offsetPos.z);

                if (bottomBlock != null && bottomBlock.isWalkable)
                {
                    returnValue = bottomBlock;
                }
                else
                {
                    offsetPos.y += 2; // Look at top block
                    Node topBlock = GetNode (offsetPos.x, offsetPos.y, offsetPos.z);
                    if (topBlock != null && topBlock.isWalkable)
                    {
                        returnValue = topBlock;
                    }
                }
            }

            // Search diagonal
            int originalX = offsetPos.x - currentNodePos.x;
            int originalZ = offsetPos.z - currentNodePos.z;

            if (Mathf.Abs (originalX) == 1 &&
                Mathf.Abs (originalZ) == 1)
            {
                Node firstNeighbour = GetNode (currentNodePos.x + originalX, currentNodePos.y, currentNodePos.z);
                if (firstNeighbour == null || !firstNeighbour.isWalkable)
                {
                    returnValue = null;
                }

                Node secondNeighbour = GetNode (currentNodePos.x, currentNodePos.y, currentNodePos.z + originalZ);
                if (secondNeighbour == null || !secondNeighbour.isWalkable)
                {
                    returnValue = null;
                }
            }

            return returnValue;
        }

        private Node GetNode (int x, int y, int z)
        {
            Node node = null;
            lock (grid)
                node = grid.GetNode (x, y, z);
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