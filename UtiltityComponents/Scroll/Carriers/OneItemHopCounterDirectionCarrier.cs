using System.Linq;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using Assets.Scripts.UtiltityComponents.Scroll.Extensions;
using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll.Carriers
{
	public class OneItemHopCounterDirectionCarrier<TData> : CarrierBase<TData>
	{
		private const float TIME_MAX_F = .2f;

		private VectorGeneric2 _path;
		private Vector2 _distance;
		private float _time = TIME_MAX_F;

		public OneItemHopCounterDirectionCarrier(IScrollController<TData> controller)
		{
			var last = controller.LastOrDefault();
			if(last == null)
				return;
			last.RectTransform.GetIntersectionInParentSpace(new Straight { Direction = controller.GrowDirection }, out _path);
		}

		protected override void Update(IScrollController<TData> controller)
		{
			var time = _time - Time.deltaTime;
			var normalized = 1f - Mathf.Clamp01(time / TIME_MAX_F);
			var offset = (1f - Mathf.Cos(Mathf.PI * normalized)) * .5f * _path.Direction;
			MoveAll(controller, offset - _distance);
			// Debug.Log(string.Format("<color=magenta>moving UP : [ time: {0}, distance: {1}, offset: {2}]</color>", _time, _distance, offset));
			if(_time < 0f)
				controller.CarrierFactory.SetDefaultCarrier(controller);
			_time = time;
			Shift(controller, offset - _distance);
			_distance = offset;
		}
	}
}