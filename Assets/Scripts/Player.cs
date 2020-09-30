using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Frontend
{
	public class Player {
		private IList<Int32> ballsCollected = new List<Int32>();

		public Player(string name) {
			Name = name;
		}

		public string Name {
			get;
			private set;
		}

		public int Points {
			get { return ballsCollected.Count; }
		}

		public void Collect(int ballNumber) {
			Debug.Log(Name + " collected ball " + ballNumber);
			ballsCollected.Add(ballNumber);
		}
		//override C# object comparison behavior to make the program compare by value
		public override bool Equals(object obj)
		{
			Player p2 = obj as Player;
			if (object.ReferenceEquals(null, p2)) return false;
			return (Name == p2.Name && Points == p2.Points) ;
		}

		public static bool operator ==(Player p1, Player p2)
		{
			if (object.ReferenceEquals(null, p1))
				return object.ReferenceEquals(null, p2);
			else if (object.ReferenceEquals(null, p2))
				return false;
			return p1.Equals(p2);
		}

		public static bool operator !=(Player p1, Player p2)
		{
			if (object.ReferenceEquals(null, p1))
				return !object.ReferenceEquals(null, p2);
			else if (object.ReferenceEquals(null, p2))
				return true;
			return !p1.Equals(p2);
		}

	}
}

