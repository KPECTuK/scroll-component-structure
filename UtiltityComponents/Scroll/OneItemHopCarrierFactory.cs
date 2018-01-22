using Assets.Scripts.UtiltityComponents.Scroll.Carriers;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;

namespace Assets.Scripts.UtiltityComponents.Scroll
{
	public class OneItemHopCarrierFactory<TData> : ICarrierFactory<TData>
	{
		//private readonly Dictionary<Type, ICarrier<TData>> _carriers;
		private ICarrier<TData> _current;
		private ICarrier<TData> _suspended;

		// ICarrierFactory
		public bool IsMoving { get { return _current is OneItemHopCoDirectionCarrier<TData> || _current is OneItemHopCounterDirectionCarrier<TData>; } }

		public OneItemHopCarrierFactory(IScrollController<TData> controller)
		{
			//_carriers = new Dictionary<Type, ICarrier<TData>>(5)
			//{
			//	{ typeof(OneItemHopCoDirectionCarrier<TData>), new OneItemHopCoDirectionCarrier<TData>(controller) },
			//	{ typeof(OneItemHopCounterDirectionCarrier<TData>), new OneItemHopCounterDirectionCarrier<TData>(controller) },
			//	{ typeof(BouncingCoDirectionCarrier<TData>), new BouncingCoDirectionCarrier<TData>() },
			//	{ typeof(BouncingCounterDirectionCarrier<TData>), new BouncingCounterDirectionCarrier<TData>() },
			//	{ typeof(DefaultCarrier<TData>), new DefaultCarrier<TData>() },
			//};

			SetDefaultCarrier(controller);
		}

		// ICarrierFactory
		public void SetCarrierCoDirection(IScrollController<TData> controller)
		{
			if(_current is SuspendedCarrier<TData>)
				return;
			_current = new OneItemHopCoDirectionCarrier<TData>(controller);
			controller.OnCarrierChange(_current);
		}

		// ICarrierFactory
		public void SetCarrierCounterDirection(IScrollController<TData> controller)
		{
			if(_current is SuspendedCarrier<TData>)
				return;
			_current = new OneItemHopCounterDirectionCarrier<TData>(controller);
			controller.OnCarrierChange(_current);
		}

		// ICarrierFactory
		public void SetBumperCoDirection(IScrollController<TData> controller)
		{
			if(_current is SuspendedCarrier<TData>)
				return;
			_current = new BouncingCoDirectionCarrier<TData>();
			controller.OnCarrierChange(_current);
		}

		// ICarrierFactory
		public void SetBumperCounterDirection(IScrollController<TData> controller)
		{
			if(_current is SuspendedCarrier<TData>)
				return;
			_current = new BouncingCounterDirectionCarrier<TData>();
			controller.OnCarrierChange(_current);
		}

		// ICarrierFactory
		public void SetDefaultCarrier(IScrollController<TData> controller)
		{
			if(_current is SuspendedCarrier<TData>)
				return;
			_current = new DefaultCarrier<TData>();
			controller.OnCarrierChange(_current);
		}

		// ICarrierFactory
		public void UpdateController(IScrollController<TData> controller)
		{
			_current.Update(controller);
		}

		public void Suspend()
		{
			_suspended = _current;
			_current = new SuspendedCarrier<TData>();
		}

		public void Resume()
		{
			_current = _suspended ?? new DefaultCarrier<TData>();
			_suspended = null;
		}
	}
}