using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Code.Building.Data
{
	[Serializable]
	public enum TileType
	{
		None = -1,
		Grass = 0,
		Sand = 10,
		Water = 20,
		Deep_Waters = 21,
		Stone = 30,
		Tree = 40
	}
}
