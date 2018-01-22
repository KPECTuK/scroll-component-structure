using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Extensions
{
	public struct VectorGeneric2
	{
		public Vector2 Origin { get; set; }
		public Vector2 Target { get; set; }
		public Vector2 Direction { get { return Target - Origin; } }
		public Vector2 DirectionNormalized { get { return (Target - Origin).normalized; } }

		public bool Intersect(VectorGeneric2 vector, out Vector2 result)
		{
			var intersection = Vector2.zero;
			var source = new Straight { Origin = vector.Origin, Direction = vector.Target - vector.Origin };
			if(!source.Intersect(this, out result))
				return false;
			return Vector3.Dot(
				Vector3.Cross(Origin - intersection, Vector3.forward).normalized,
				Vector3.Cross(Target - intersection, Vector3.forward).normalized) > 0;
		}
	}
}