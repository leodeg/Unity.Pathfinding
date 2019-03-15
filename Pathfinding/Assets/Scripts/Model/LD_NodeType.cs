using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	internal enum LD_NodeType
	{
		Open = 0,
		Blocked = 1,
		LightTerrain = 2,
		MediumTerrain = 3,
		HeavyTerrain = 4
	}
}