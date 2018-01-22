using System.Linq;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using Assets.Scripts.UtiltityComponents.Scroll.Extensions;
using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Carriers
{
	public class FreeCarrier<TData> : CarrierBase<TData>
	{
		private const float STOP_TIME_F = 1f;

		private readonly VectorGeneric2 _target;
		private readonly Vector2 _acceleration;
		private Vector2 _delta;
		private bool _isAtTarget;
		private float _passedDist;
		private readonly float _necessaryDist;

		public FreeCarrier(IScrollController<TData> controller)
		{
			var delta = controller.PointerEventData.delta.ProjectTo(controller.GrowDirection);
			//! temporal - item height limit speed
			_delta = delta.magnitude > ScrollerExtensions.SCROLL_SPEED_LIMIT_F ? delta.normalized * ScrollerExtensions.SCROLL_SPEED_LIMIT_F : delta;
			var cutting = new Straight { Direction = _delta.normalized, };
			_isAtTarget = !controller.RectTransform.GetIntersectionInLocalSpace(cutting, out _target);
			var item = controller.FirstOrDefault(_ => _.RectTransform.IsInsideParentSpace(_target.Origin)) ?? (Vector2.Dot(_delta, controller.GrowDirection) > 0 ? controller.First() : controller.Last());

			VectorGeneric2 @internal;
			item.RectTransform.GetIntersectionInParentSpace(cutting, out @internal);

			_necessaryDist = (_target.Origin - @internal.Origin).magnitude;
			_acceleration = -_delta.normalized * (_necessaryDist / (STOP_TIME_F * STOP_TIME_F) + _delta.magnitude / STOP_TIME_F);

			//ScrollerExtensions.MultiLine(controller.RectTransform.GetSidesInLocalSpace(), Color.yellow * .5f, true, 1f);
			//ScrollerExtensions.Cross(_target.Origin, Color.yellow, 30f, 1f);
			//ScrollerExtensions.Cross(@internal.Origin, Color.yellow, 30f, 1f);

			//Debug.Log(string.Format("speed: {0}, acceleration: {1}, distance is: {2}", _delta, _acceleration, _necessuaryDist));
		}

		protected override void Update(IScrollController<TData> controller)
		{
			if(_isAtTarget)
			{
				controller.CarrierFactory.SetDefaultCarrier(controller);
				return;
			}

			var direction = new Straight { Direction = controller.GrowDirection };
			VectorGeneric2 intersection;
			if(!controller.RectTransform.GetIntersectionInLocalSpace(direction, out intersection))
				return;

			if(Vector2.Dot(_delta, controller.GrowDirection) > 0)
			{
				if(!controller.First().RectTransform.IsInsideParentSpace(intersection.Origin) && controller.RectTransform.IsOverlapsChild(controller.First().RectTransform))
				{
					controller.ShiftWindowUp();
				}
			}
			else
			{
				if(!controller.Last().RectTransform.IsInsideParentSpace(intersection.Target) && controller.RectTransform.IsOverlapsChild(controller.Last().RectTransform))
				{
					controller.ShiftWindowDown();
				}
			}

			var delta = _passedDist + _delta.magnitude < _necessaryDist ? _delta : _delta.normalized * (_necessaryDist - _passedDist);
			foreach(var item in controller)
				item.RectTransform.anchoredPosition += delta;

			_passedDist += delta.magnitude;
			_delta = delta + Time.deltaTime * _acceleration;
			_isAtTarget = _necessaryDist - _passedDist <= 0f;

			//Debug.Log(string.Format("speed: {0} nDis: {1} pDis: {2} delta: {3}", _delta.magnitude, _necessuaryDist, _passedDist, delta.magnitude));
		}
	}
}