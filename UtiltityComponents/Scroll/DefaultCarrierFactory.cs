using System;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;

namespace Assets.Scripts.UtiltityComponents.Scroll
{
	public class DefaultCarrierFactory<TData> : ICarrierFactory<TData>
	{
		public bool IsMoving { get; private set; }

		public DefaultCarrierFactory(IScrollController<TData> controller) { }

		public void SetCarrierCoDirection(IScrollController<TData> controller)
		{
			throw new NotImplementedException();
		}

		public void SetCarrierCounterDirection(IScrollController<TData> controller)
		{
			throw new NotImplementedException();
		}

		public void SetBumperCoDirection(IScrollController<TData> controller)
		{
			throw new NotImplementedException();
		}

		public void SetBumperCounterDirection(IScrollController<TData> controller)
		{
			throw new NotImplementedException();
		}

		public void SetDefaultCarrier(IScrollController<TData> controller)
		{
			throw new NotImplementedException();
		}

		public void UpdateController(IScrollController<TData> controller)
		{
			throw new NotImplementedException();
		}

		public void Suspend()
		{
			throw new NotImplementedException();
		}

		public void Resume()
		{
			throw new NotImplementedException();
		}
	}
}