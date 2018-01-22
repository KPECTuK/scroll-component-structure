using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UtiltityComponents.Scroll.Contracts
{
	public interface IScrollController<TData> : IEnumerable<IScrollItem<TData>>
	{
		ICarrierFactory<TData> CarrierFactory { get; }
		PointerEventData PointerEventData { get; }
		Vector2 GrowDirection { get; }
		RectTransform ItemRendererPrefab { get; }
		RectTransform RectTransform { get; }

		void ShiftWindowUp();
		void ShiftWindowDown();
		
		void OnCarrierChange(ICarrier<TData> current);
	}
}