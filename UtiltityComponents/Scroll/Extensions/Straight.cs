using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Extensions
{
    public struct Straight
    {
        public Vector2 Direction { get; set; }
        public Vector2 Origin { get; set; }

        public bool Intersect(VectorGeneric2 vector, out Vector2 point)
        {
            point = Vector2.zero;
            var plane = new Plane(Vector3.Cross(Direction, Vector3.forward), Origin);
            if(plane.SameSide(vector.Origin, vector.Target))
                return false;
            var originDistance = Mathf.Abs(plane.GetDistanceToPoint(vector.Origin));
            var targetDistance = Mathf.Abs(plane.GetDistanceToPoint(vector.Target));
            point = vector.Origin + vector.Direction * originDistance / (originDistance + targetDistance);
            return true;
        }
    }
}