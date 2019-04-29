using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridMaster
{
    public class Node
    {
        public enum NodeType { groud, air }

        public int x;
        public int y;
        public int z;

        public float heightCost;
        public float gridCost;
        public float TotalCost { get { return heightCost + gridCost; } }

        public GameObject worldObject;
        public Node parentNode;
        public bool isWalkable = true;

        public NodeType nodeType;
    }
}