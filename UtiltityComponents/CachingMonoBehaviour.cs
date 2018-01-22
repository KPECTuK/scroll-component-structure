using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UtiltityComponents {
	[RequireComponent(typeof(CanvasGroup))]
// ReSharper disable once UnusedMember.Global
// ReSharper disable once CheckNamespace
	public abstract class CachingMonoBehaviour : UIBehaviour
	{
		// ReSharper disable ConvertToConstant.Local
		[SerializeField] private float _max = 1f;
		[SerializeField] private float _min = 0f;
		// ReSharper restore ConvertToConstant.Local

		private CanvasGroup _canvasGroup;
		private RectTransform _rectTransform;
		private Transform _transform;

		public CanvasGroup CanvasGroup { get { return _canvasGroup ?? (_canvasGroup = GetComponent<CanvasGroup>()); } }
		public RectTransform RectTransform { get { return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>()); } }
		public Transform Transform { get { return _transform ?? (_transform = GetComponent<Transform>()); } }

		protected void SetLimits(float min, float max)
		{
			if(max < min)
			{
				_max = Mathf.Clamp01(min);
				_min = Mathf.Clamp01(max);
			}
			else
			{
				_max = Mathf.Clamp01(max);
				_min = Mathf.Clamp01(min);
			}
		}

		public virtual IEnumerator PerformShow(float duration)
		{
			CanvasGroup.blocksRaycasts = true;

			var speed = duration > 0f ? (_max - CanvasGroup.alpha) / duration : float.MaxValue;
			yield return new WaitWhile(() =>
										{
											CanvasGroup.alpha += speed * Time.deltaTime;
											return CanvasGroup.alpha < _max;
										});
			CanvasGroup.alpha = _max;

			CanvasGroup.interactable = true;
		}

		public virtual IEnumerator PerformHide(float duration)
		{
			CanvasGroup.interactable = false;

			var speed = duration > 0 ? (_min - CanvasGroup.alpha) / duration : float.MinValue;
			yield return new WaitWhile(() =>
										{
											CanvasGroup.alpha += speed * Time.deltaTime;
											return CanvasGroup.alpha > _min;
										});
			CanvasGroup.alpha = _min;

			CanvasGroup.blocksRaycasts = false;
		}
	}
}