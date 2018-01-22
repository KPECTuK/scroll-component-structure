using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using Assets.Scripts.UtiltityComponents.Scroll.Extensions;

namespace Assets.Scripts.UtiltityComponents.Scroll.Carriers
{
	public class BouncingCounterDirectionCarrier<TData> : BumperCarierBase<TData>
	{
		protected override void Update(IScrollController<TData> controller)
		{
			var cutting = new Straight { Direction = controller.GrowDirection, };
			VectorGeneric2 intersection;
			controller.RectTransform.GetIntersectionInLocalSpace(cutting, out intersection);
			var isFirst = true;
			foreach(var item in controller)
			{
				item.RectTransform.anchoredPosition = isFirst ? intersection.Origin : intersection.Target;
				item.RectTransform.GetIntersectionInParentSpace(cutting, out intersection);
				isFirst = false;
			}
		}
	}
}