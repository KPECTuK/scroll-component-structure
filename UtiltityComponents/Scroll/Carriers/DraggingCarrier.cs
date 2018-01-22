using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using Assets.Scripts.UtiltityComponents.Scroll.Extensions;
using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Carriers
{
	public class DraggingCarrier<TData> : CarrierBase<TData>
	{
		protected override void Update(IScrollController<TData> controller)
		{
			var delta = controller.PointerEventData == null
				? Vector2.zero
				: controller.PointerEventData.delta.ProjectTo(controller.GrowDirection);
			MoveAll(controller, delta);
			Shift(controller, delta);

#if DEBUG
			//ScrollerExtensions.MultiLine(controller.RectTransform.GetSidesInLocalSpace(), Color.yellow * .5f, true, .5f);
			//ScrollerExtensions.MultiLine(controller.First().RectTransform, Color.yellow, true, .5f);
			//ScrollerExtensions.MultiLine(controller.Last().RectTransform, Color.yellow, true, .5f);
			//ScrollerExtensions.Cross(intersection.Origin, Color.yellow, 30f, 0f);
			//ScrollerExtensions.Cross(intersection.Target, Color.yellow, 30f, 0f);
#endif
		}
	}
}