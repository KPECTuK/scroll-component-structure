using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UtiltityComponents.Scroll
{
	public partial class ScrollController<TData>
	{
		//? to use Queue<ICarrier> might be useful

		public PointerEventData PointerEventData { get; private set; }
		public ICarrierFactory<TData> CarrierFactory { get; private set; }

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			PointerEventData = eventData;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			PointerEventData = eventData;
		}

		public void OnDrag(PointerEventData eventData)
		{
			PointerEventData = eventData;
			if(CarrierFactory.IsMoving)
				return;

			if(Vector2.Dot(eventData.delta, GrowDirection) < 0)
				CarrierFactory.SetCarrierCoDirection(this);
			else
				CarrierFactory.SetCarrierCounterDirection(this);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			PointerEventData = eventData;
			// it from the device touch that have no delta here (device only, another fucking Unity feature)
			// may result in slightly different aligning from Unity Remote device (and i guess all touch devices actually because of the Unity event generator)
			// need to store position and delta to calculate it manually
			//_carrier = PointerEventData.delta.magnitude > 0 || _freeMovementPredict == null ? new FreeCarrier(this) : _freeMovementPredict;
		}

		protected void ResumeMovement()
		{
			CarrierFactory.Resume();
		}

		protected void SuspendMovement()
		{
			CarrierFactory.Suspend();
		}

		protected void SetFactory(ICarrierFactory<TData> factory)
		{
			CarrierFactory = factory ?? new DefaultCarrierFactory<TData>(this);
		}

		protected void UpdateView()
		{
			foreach(var item in _window)
			{
				item.UpdateView();
			}
		}

		void IScrollController<TData>.OnCarrierChange(ICarrier<TData> current)
		{
			OnCarrierChange(current);
		}

		protected abstract void OnCarrierChange(ICarrier<TData> current);
		protected abstract void OnCoDirectionThresholdReach();
		protected abstract void OnCounterDirectionThresholdReach();
		protected abstract void OnCoDirectionBoundReach();
		protected abstract void OnCounterDirectionBoundReach();

		protected override void Awake()
		{
			base.Awake();

			GrowDirection = Vector2.down;
			SetFactory((ICarrierFactory<TData>)_itemFactory);
			SetFactory(CarrierFactory);
		}

		// ReSharper disable once UnusedMember.Local
		private void LateUpdate()
		{
			if(!Application.isPlaying)
				return;

			// movement updater
			CarrierFactory.UpdateController(this);
			PointerEventData = null;
			_itemFactory.MaintainCache();
		}

#if UNITY_EDITOR
		protected override void Reset() { }

		protected override void OnValidate() { }
#endif
	}
}
