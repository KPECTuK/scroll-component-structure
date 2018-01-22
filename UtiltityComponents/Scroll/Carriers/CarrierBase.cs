using System.Linq;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using Assets.Scripts.UtiltityComponents.Scroll.Extensions;
using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Carriers
{
	public abstract class CarrierBase<TData> : ICarrier<TData>
	{
		protected abstract void Update(IScrollController<TData> controller);

		void ICarrier<TData>.Update(IScrollController<TData> controller)
		{
			Update(controller);
		}

		protected void Shift(IScrollController<TData> controller, Vector2 delta)
		{
			////! temporal - item height limit speed
			//delta = delta.magnitude > ScrollerExtensions.SCROLL_SPEED_LIMIT_F ? delta.normalized * ScrollerExtensions.SCROLL_SPEED_LIMIT_F : delta;

			var direction = new Straight { Direction = controller.GrowDirection };
			VectorGeneric2 intersection;
			if(!controller.RectTransform.GetIntersectionInLocalSpace(direction, out intersection))
				return;

			if(Vector2.Dot(delta, controller.GrowDirection) > 0)
			{
				var first = controller.FirstOrDefault();
				if(first != null && !first.RectTransform.IsInsideParentSpace(intersection.Origin) && controller.RectTransform.IsOverlapsChild(controller.First().RectTransform))
				{
					controller.ShiftWindowUp();
				}
			}

			if(Vector2.Dot(delta, controller.GrowDirection) < 0)
			{
				var last = controller.LastOrDefault();
				if(last != null && !last.RectTransform.IsInsideParentSpace(intersection.Target) && controller.RectTransform.IsOverlapsChild(controller.Last().RectTransform))
				{
					controller.ShiftWindowDown();
				}
			}
		}

		protected void MoveAll(IScrollController<TData> controller, Vector2 delta)
		{
			////! temporal - item height limit speed
			//delta = delta.magnitude > ScrollerExtensions.SCROLL_SPEED_LIMIT_F ? delta.normalized * ScrollerExtensions.SCROLL_SPEED_LIMIT_F : delta;
			foreach(var item in controller)
				item.RectTransform.anchoredPosition += delta;
		}
	}
}