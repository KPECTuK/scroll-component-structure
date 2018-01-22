using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Contracts
{
	public interface IScrollItem<TData>
	{
		TData LinkedData { get; }
		// assuming the position is anchoredPosition, had been removed here
		RectTransform RectTransform { get; }
		void Resume(TData data);
		void Suspend();
		void UpdateView();
	}
}
