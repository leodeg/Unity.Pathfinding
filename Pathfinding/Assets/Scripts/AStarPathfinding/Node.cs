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

        public float distanceCost;
        public float neighbourCost;
        public float TotalCost { get { return distanceCost + neighbourCost; } }

        public GameObject prefabObject;
        public Node parentNode;
        public bool isWalkable = true;

        public NodeType nodeType;
    }
}