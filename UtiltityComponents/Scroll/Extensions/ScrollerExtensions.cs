using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Extensions
{
	public static class ScrollerExtensions
	{
		public const float SCROLL_SPEED_LIMIT_F = 12f;

		public static Vector2 ProjectTo(this Vector2 source, Vector2 @base)
		{
			return
				@base.normalized *
				source.magnitude *
				Vector2.Dot(source.normalized, @base.normalized);
		}

		public static Rect Move(this Rect source, Vector2 offset)
		{
			return new Rect(source.position + offset, source.size);
		}

		public static VectorGeneric2[] GetSidesInParentSpace(this RectTransform source)
		{
			var corners = new Vector3[4];
			source.GetLocalCorners(corners);
			return new[]
			{
				new VectorGeneric2 { Origin = (Vector2)corners[0] + source.anchoredPosition, Target = (Vector2)corners[1] + source.anchoredPosition },
				new VectorGeneric2 { Origin = (Vector2)corners[1] + source.anchoredPosition, Target = (Vector2)corners[2] + source.anchoredPosition },
				new VectorGeneric2 { Origin = (Vector2)corners[2] + source.anchoredPosition, Target = (Vector2)corners[3] + source.anchoredPosition },
				new VectorGeneric2 { Origin = (Vector2)corners[3] + source.anchoredPosition, Target = (Vector2)corners[0] + source.anchoredPosition },
			};
		}

		public static VectorGeneric2[] GetSidesInLocalSpace(this RectTransform source)
		{
			var corners = new Vector3[4];
			source.GetLocalCorners(corners);
			return new[]
			{
				new VectorGeneric2 { Origin = corners[0], Target = corners[1] },
				new VectorGeneric2 { Origin = corners[1], Target = corners[2] },
				new VectorGeneric2 { Origin = corners[2], Target = corners[3] },
				new VectorGeneric2 { Origin = corners[3], Target = corners[0] },
			};
		}

		public static Vector2[] GetCornersInLocalSpace(this RectTransform source)
		{
			var corners = new Vector3[4];
			var result = new Vector2[4];
			source.GetLocalCorners(corners);
			for(var index = 0; index < corners.Length; index++)
				result[index] = corners[index];
			return result;
		}

		public static Vector2[] GetCornersInParentSpace(this RectTransform source)
		{
			var corners = new Vector3[4];
			var result = new Vector2[4];
			source.GetLocalCorners(corners);
			for(var index = 0; index < corners.Length; index++)
				result[index] = (Vector2)corners[index] + source.anchoredPosition;
			return result;
		}

		public static bool GetIntersectionInParentSpace(this RectTransform source, Straight cutting, out VectorGeneric2 result)
		{
			var intersections = source.GetSidesInParentSpace().Aggregate(new List<Vector2>(2), (acm, _) =>
			{
				Vector2 intersection;
				if(cutting.Intersect(_, out intersection))
					acm.Add(intersection);
				return acm;
			}).ToArray();
			//var points = source.GetSidesInParentSpace().Select(_ => _.Origin).ToArray();
			//Debug.Log(string.Format("<color=blue>PARENT points: {0}, {1}, {2}, {3},</color>", points[0], points[1], points[2], points[3] ));
			result = intersections.Length == 0
				? new VectorGeneric2()
				: new VectorGeneric2
				{
					Origin = Vector2.Dot(cutting.Direction, intersections[1] - intersections[0]) > 0f ? intersections[0] : intersections[1],
					Target = Vector2.Dot(cutting.Direction, intersections[1] - intersections[0]) > 0f ? intersections[1] : intersections[0]
				};
			return intersections.Length != 0;
		}

		public static bool GetIntersectionInLocalSpace(this RectTransform source, Straight cutting, out VectorGeneric2 result)
		{
			var intersections = source.GetSidesInLocalSpace().Aggregate(new List<Vector2>(2), (acm, _) =>
			{
				Vector2 intersection;
				if(cutting.Intersect(_, out intersection))
					acm.Add(intersection);
				return acm;
			}).ToArray();
			//var points = source.GetSidesInParentSpace().Select(_ => _.Origin).ToArray();
			//Debug.Log(string.Format("<color=blue>LOCAL points: {0}, {1}, {2}, {3},</color>", points[0], points[1], points[2], points[3]));
			result = intersections.Length == 0
				? new VectorGeneric2()
				: new VectorGeneric2
				{
					Origin = Vector2.Dot(cutting.Direction, intersections[1] - intersections[0]) > 0f ? intersections[0] : intersections[1],
					Target = Vector2.Dot(cutting.Direction, intersections[1] - intersections[0]) > 0f ? intersections[1] : intersections[0]
				};
			return intersections.Length != 0;
		}

		public static bool IsInsideParentSpace(this RectTransform source, Vector2 point)
		{
			VectorGeneric2 intersection;
			var vertical = source.GetIntersectionInParentSpace(new Straight { Direction = Vector2.up, Origin = point }, out intersection);
			//Debug.Log(string.Format("<color=blue>PARENT vertical: {0}, p:{3} :: i: {1} - {2}</color>", vertical, intersection.Origin, intersection.Target, point));
			var horizontal = source.GetIntersectionInParentSpace(new Straight { Direction = Vector2.right, Origin = point }, out intersection);
			//Debug.Log(string.Format("<color=blue>PARENT horizontal: {0}, p: {3} :: i: {1} - {2}</color>", horizontal, intersection.Origin, intersection.Target, point));
			return vertical && horizontal;
		}

		public static bool IsInsideLocalSpace(this RectTransform source, Vector2 point)
		{
			VectorGeneric2 intersection;
			var vertical = source.GetIntersectionInLocalSpace(new Straight { Direction = Vector2.up, Origin = point }, out intersection);
			//Debug.Log(string.Format("<color=blue>LOCAL vertical: {0}, p:{3} :: i: {1} - {2}</color>", vertical, intersection.Origin, intersection.Target, point));
			var horizontal = source.GetIntersectionInLocalSpace(new Straight { Direction = Vector2.right, Origin = point }, out intersection);
			//Debug.Log(string.Format("<color=blue>LOCAL horizontal: {0}, p: {3} :: i: {1} - {2}</color>", horizontal, intersection.Origin, intersection.Target, point));
			return vertical && horizontal;
		}

		public static bool IsOverlapsChild(this RectTransform parent, RectTransform child)
		{
			foreach(var childSide in child.GetSidesInParentSpace())
			{
				foreach(var parentSide in parent.GetSidesInLocalSpace())
				{
					Vector2 intersection;
					if(childSide.Intersect(parentSide, out intersection))
						return true;
				}
			}
			return child.GetCornersInParentSpace().Any(parent.IsInsideLocalSpace);
		}

		public static void Cross(Vector3 pointer, Color color, float size = 10f, float duration = 1f)
		{
			Debug.DrawLine(pointer + Vector3.down * size, pointer + Vector3.up * size, color, duration);
			Debug.DrawLine(pointer + Vector3.left * size, pointer + Vector3.right * size, color, duration);
		}

		public static void MultiLine(Vector3[] corners, Color color, bool isClosed = true, float duration = 1f)
		{
			for(var index = 0; index < (isClosed ? corners.Length : corners.Length - 1); index++)
				Debug.DrawLine(corners[index], corners[(index + 1) % corners.Length], color, duration);
		}

		public static void MultiLine(VectorGeneric2[] sides, Color color, bool isClosed = true, float duration = 1f)
		{
			for(var index = 0; index < sides.Length; index++)
				Debug.DrawLine(sides[index].Origin, sides[index].Target, color, duration);
		}

		public static void MultiLine(RectTransform source, Color color, bool isClosed = true, float duration = 1f)
		{
			var corners = source.GetCornersInParentSpace();
			for(var index = 0; index < (isClosed ? corners.Length : corners.Length - 1); index++)
				Debug.DrawLine(corners[index], corners[(index + 1) % corners.Length], color, duration);
		}

		public static LinkedListNode<T> FindFromHead<T>(this LinkedList<T> source, int index)
		{
			var current = !ReferenceEquals(null, source) && source.Count > 0 ? source.First : null;
			while(--index > 0 && !ReferenceEquals(null, current))
			{
				current = current.Next;
			}
			return current;
		}

		public static LinkedListNode<T> FindFromTail<T>(this LinkedList<T> source, int index)
		{
			var current = !ReferenceEquals(null, source) && source.Count > 0 ? source.Last : null;
			while(--index > 0 && !ReferenceEquals(null, current))
			{
				current = current.Previous;
			}
			return current;
		}

		public static bool HeadContains<T>(this LinkedList<T> source, int index, T value)
		{
			var current = !ReferenceEquals(null, source) && source.Count > 0 ? source.First : null;
			while(--index > 0 && !ReferenceEquals(null, current) && !value.Equals(current.Value))
			{
				current = current.Next;
			}
			return !ReferenceEquals(null, current) && value.Equals(current.Value);
		}

		public static bool ContainsFromTail<T>(this LinkedList<T> source, int index, T value)
		{
			var current = !ReferenceEquals(null, source) && source.Count > 0 ? source.Last : null;
			while(--index > 0 && !ReferenceEquals(null, current) && !value.Equals(current.Value))
			{
				current = current.Previous;
			}
			return !ReferenceEquals(null, current) && value.Equals(current.Value);
		}

		public static LinkedList<T> RemoveFromHead<T>(this LinkedList<T> source, int quantity)
		{
			while(source.Count > 0 && quantity > 0)
			{
				source.RemoveFirst();
				quantity -= 1;
			}
			return source;
		} 

		public static LinkedList<T> RemoveFromTail<T>(this LinkedList<T> source, int quantity)
		{
			while(source.Count > 0 && quantity > 0)
			{
				source.RemoveLast();
				quantity -= 1;
			}
			return source;
		}

		public static LinkedListNode<T> FindFromHead<T>(this LinkedList<T> source, T value)
		{
			var current = source == null ? null : source.First;
			while(current != null && !(value == null ? ReferenceEquals(null, current.Value) : value.Equals(current.Value)))
				current = current.Next;
			return current;
		}

		public static LinkedListNode<T> FindFromTail<T>(this LinkedList<T> source, T value)
		{
			var current = source == null ? null : source.Last;
			while(current != null && !(value == null ? ReferenceEquals(null, current.Value) : value.Equals(current.Value)))
				current = current.Previous;
			return current;
		}

		public static int CountFromHead<T>(this LinkedList<T> source, Predicate<T> predicate)
		{
			var counter = 0;
			var current = !ReferenceEquals(null, source) && source.Count > 0 ? source.First : null;
			predicate = predicate ?? (_ => false);
			while(!ReferenceEquals(null, current) && !predicate(current.Value))
			{
				current = current.Next;
				counter += 1;
			}
			return counter;
		}

		public static int CountFromTail<T>(this LinkedList<T> source, Predicate<T> predicate)
		{
			var counter = 0;
			var current = !ReferenceEquals(null, source) && source.Count > 0 ? source.Last : null;
			predicate = predicate ?? (_ => false);
			while(!ReferenceEquals(null, current) && !predicate(current.Value))
			{
				current = current.Previous;
				counter += 1;
			}
			return counter;
		}
	}
}