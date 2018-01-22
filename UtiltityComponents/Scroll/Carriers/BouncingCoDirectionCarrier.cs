using System.Linq;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using Assets.Scripts.UtiltityComponents.Scroll.Extensions;

namespace Assets.Scripts.UtiltityComponents.Scroll.Carriers
{
	public class BouncingCoDirectionCarrier<TData> : BumperCarierBase<TData>
	{
		protected override void Update(IScrollController<TData> controller)
		{
			var cutting = new Straight { Direction = -controller.GrowDirection, };
			VectorGeneric2 intersection;
			controller.RectTransform.GetIntersectionInLocalSpace(cutting, out intersection);
			var isFirst = true;
			foreach(var item in controller.Reverse())
			{
				VectorGeneric2 intersectionSelf;
				item.RectTransform.GetIntersectionInLocalSpace(cutting, out intersectionSelf);
				item.RectTransform.anchoredPosition = (isFirst ? intersection.Origin : intersection.Target) + intersectionSelf.Direction;
				item.RectTransform.GetIntersectionInParentSpace(cutting, out intersection);
				isFirst = false;
			}
		}
	}
}