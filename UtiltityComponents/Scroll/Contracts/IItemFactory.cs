namespace Assets.Scripts.UtiltityComponents.Scroll.Contracts
{
	public interface IItemFactory<TData>
	{
		IScrollItem<TData> Get(TData data);
		void Put(IScrollItem<TData> item);
		void ClearAssetCache();
		void MaintainCache();
	}
}