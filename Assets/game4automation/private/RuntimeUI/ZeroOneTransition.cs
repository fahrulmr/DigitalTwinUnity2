using System;
using UnityEngine;

namespace game4automation
{
	[Serializable]
	public struct ZeroOneTransition
	{
		public float Transition;

		public ZeroOneTransition( float t )
		{
			Transition = t;
		}

		/// <summary>
		/// Return the eased transition. Identical to Mathf.SmoothStep( ).
		/// </summary>
		public float Eased { get{ return -2f * Transition * Transition * Transition + 3 * Transition * Transition; } }

		public float Squared { get{ return Transition * Transition; } }
		public float Sqrt { get{ return (float) Math.Sqrt( Transition ); } }



		/// <summary>
		/// Advance the transition by the specified value.
		/// If active is true the transition will be advanced towards 1. Otherwise to 0. 
		/// </summary>
		public bool Advance( bool active, float diff )
		{
			if( active ? Transition >= 1f : Transition <= 0f )
				return false;

			Advance( active ? diff : -diff );

			return true;
		}

		/// <summary>
		/// Advance the transition by the specified value.
		/// </summary>
		public float Advance( float diff )
		{
			Transition = Mathf.Clamp01( Transition + diff );

			return Transition;
		}

		public float SmoothStep( float from, float to )
		{
			var t = Eased;

			return to * t + (1f - t) * from;
		}
	}
}
