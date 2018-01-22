namespace Assets.Scripts.UtiltityComponents.Scroll.Contracts
{
	public interface ICarrierFactory<TData>
	{
		bool IsMoving { get; }
		void SetCarrierCoDirection(IScrollController<TData> controller);
		void SetCarrierCounterDirection(IScrollController<TData> controller);
		void SetBumperCoDirection(IScrollController<TData> controller);
		void SetBumperCounterDirection(IScrollController<TData> controller);
		void SetDefaultCarrier(IScrollController<TData> controller);
		void UpdateController(IScrollController<TData> controller);
		void Suspend();
		void Resume();
	}
}