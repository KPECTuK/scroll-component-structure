using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UtiltityComponents.Scroll
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[SelectionBase]
	public class ScrollBarController : UIBehaviour
	{
#if UNITY_EDITOR
		protected override void Reset() { }

		protected override void OnValidate() { }
#endif
	}
}