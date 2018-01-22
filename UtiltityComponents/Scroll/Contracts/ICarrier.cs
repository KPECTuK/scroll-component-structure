namespace Assets.Scripts.UtiltityComponents.Scroll.Contracts
{
	public interface ICarrier<TData>
	{
		void Update(IScrollController<TData> controller);
	}
}