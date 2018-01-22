using Assets.Scripts.UtiltityComponents.Scroll.Contracts;

namespace Assets.Scripts.UtiltityComponents.Scroll.Carriers
{
	public class SuspendedCarrier<TData> : CarrierBase<TData>
	{
		protected override void Update(IScrollController<TData> controller) { }
	}
}