using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using Assets.Scripts.UtiltityComponents.Scroll.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UtiltityComponents.Scroll
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Mask))]
	[RequireComponent(typeof(Image))]
	[SelectionBase]
	public abstract partial class ScrollController<TData> :
		CachingMonoBehaviour,
		IScrollController<TData>,
		//
		IInitializePotentialDragHandler,
		IBeginDragHandler,
		IEndDragHandler,
		IDragHandler,
		ICanvasElement,
		ILayoutElement,
		ILayoutGroup
		where TData : class
	{
		// TODO: придумать как обрабатывать зацикленные списки

#pragma warning disable 169, 649
		[SerializeField] private RectTransform _itemPrefab;
#pragma warning restore 169, 649

		private interface IBindRequest
		{
			void Execute();
			void Merge(IEnumerable<TData> set);
		}

		private class BindPreviousRequest : IBindRequest
		{
			private readonly ScrollController<TData> _controller;

			public BindPreviousRequest(ScrollController<TData> controller)
			{
				_controller = controller;
			}

			public void Execute()
			{
				_controller.OnCounterDirectionThresholdReach();
			}

			public void Merge(IEnumerable<TData> source)
			{
				UnityEngine.Debug.Log(source.ToText(string.Format("<color=magenta>method: {0} - merging with</color>", GetType().NameNice())));
				UnityEngine.Debug.Log(_controller._window.Select(_ => _.LinkedData).ToText("<color=magenta>window</color>"));

				if(ReferenceEquals(null, source))
					return;

				var set = source.ToArray();
				if(_controller._window.Count > 0)
				{
					// удалить нижнюю часть списка с пороговым запасом
					var lastWindowData = _controller._window.Last.Value.LinkedData;
					var toRemove = _controller._bindings.CountFromTail(_ => ReferenceEquals(_, lastWindowData)) - _controller._preBoundaryThreshold;
					toRemove = toRemove < 0 ? 0 : toRemove;
					_controller._bindings.RemoveFromTail(toRemove);
				}

				// мердж списка (пока без вставок)
				var enumerator = set.Reverse().GetEnumerator();
				TData lastData = null;
				while(enumerator.MoveNext())
				{
					var node = _controller._bindings.FindFromTail(enumerator.Current);
					lastData = node == null ? lastData : node.Value;
				}

				var toAppend = lastData == null ? set : set.TakeWhile(_ => !_.Equals(lastData));
				foreach (var data in toAppend.Reverse())
				{
					_controller._bindings.AddFirst(data);
				}
			}
		}

		private class BindNextRequest : IBindRequest
		{
			private readonly ScrollController<TData> _controller;

			public BindNextRequest(ScrollController<TData> controller)
			{
				_controller = controller;
			}

			public void Execute()
			{
				_controller.OnCoDirectionThresholdReach();
			}

			public void Merge(IEnumerable<TData> source)
			{
				UnityEngine.Debug.Log(source.ToText(string.Format("<color=magenta>method: {0} - merging with</color>", GetType().NameNice())));
				UnityEngine.Debug.Log(_controller._window.Select(_ => _.LinkedData).ToText("<color=magenta>window</color>"));

				if(ReferenceEquals(null, source))
					return;

				var set = source.ToArray();
				// удалить нижнюю часть списка с пороговым запасом
				if(_controller._window.Count > 0)
				{
					var lastWindowData = _controller._window.First.Value.LinkedData;
					var toRemove = _controller._bindings.CountFromHead(_ => ReferenceEquals(_, lastWindowData)) - _controller._preBoundaryThreshold;
					toRemove = toRemove < 0 ? 0 : toRemove;
					_controller._bindings.RemoveFromHead(toRemove);
				}
			
				// мердж списка (пока без вставок)
				var enumerator = set.Cast<TData>().GetEnumerator();
				TData lastData = null;
				while(enumerator.MoveNext())
				{
					var node = _controller._bindings.FindFromHead(enumerator.Current);
					lastData = node == null ? lastData : node.Value;
				}

				var toAppend = lastData == null ? set : set.TakeWhile(_ => !_.Equals(lastData));
				foreach (var data in toAppend)
				{
					_controller._bindings.AddLast(data);
				}
			}
		}

		private readonly LinkedList<TData> _bindings = new LinkedList<TData>();
		private readonly LinkedList<IScrollItem<TData>> _window = new LinkedList<IScrollItem<TData>>();
		private IItemFactory<TData> _itemFactory;
		private int _preBoundaryThreshold;
		private IBindRequest _currentBindRequest;

		public RectTransform ItemRendererPrefab { get { return _itemPrefab; } }
		public Vector2 GrowDirection { get; private set; }

		protected TData LastInWindow { get { return _window.Last.Value.LinkedData; } }
		protected TData FirsInWindow { get { return _window.First.Value.LinkedData; } }

		protected void SetFactory(IItemFactory<TData> factory)
		{
			_itemFactory = factory ?? new DefaultItemFactory<TData>(this);
		}

		protected void SetWindow(int thresholdSize)
		{
			_preBoundaryThreshold = thresholdSize;
			_currentBindRequest = new BindNextRequest(this);
		}

		public void Remove(TData data)
		{
			// BUG: not correct
			// TODO: implement shut carrier

			_bindings.Remove(data);
			var item = _window.Last.Value;
			_window.Remove(item);
			_itemFactory.Put(item);
			if(_window.All(_ => !data.Equals(_.LinkedData)))
				return;
			var next = _bindings.Find(_window.Last.Value.LinkedData).Next;
			if(next != null)
				_window.AddLast(_itemFactory.Get(data));
		}

		public void RemoveAll()
		{
			foreach(var item in _window)
				_itemFactory.Put(item);
			_window.Clear();
			_bindings.Clear();
		}

		public void Release()
		{
			_itemFactory.ClearAssetCache();
		}

		public void ShiftWindowUp()
		{
			// попадает ли голова в диапазон и отправлен ли запрос
			if(!(_currentBindRequest is BindPreviousRequest) && _bindings.HeadContains(_preBoundaryThreshold, _window.First.Value.LinkedData))
			{
				_currentBindRequest = new BindPreviousRequest(this);
				_currentBindRequest.Execute();
			}

			var previous = _bindings.Find(_window.First.Value.LinkedData).Previous;

			// while scrolling down (moving up)
			if(previous == null)
			{
				var measureCutting = new Straight { Direction = GrowDirection };
				VectorGeneric2 firstIntersection;
				VectorGeneric2 lastIntersection;
				VectorGeneric2 boundIntersection;
				_window.First.Value.RectTransform.GetIntersectionInParentSpace(measureCutting, out firstIntersection);
				_window.Last.Value.RectTransform.GetIntersectionInParentSpace(measureCutting, out lastIntersection);
				RectTransform.GetIntersectionInLocalSpace(measureCutting, out boundIntersection);
				if(boundIntersection.Direction.sqrMagnitude < (lastIntersection.Target - firstIntersection.Origin).sqrMagnitude)
				{
					CarrierFactory.SetBumperCounterDirection(this);
				}
				else
				{
					CarrierFactory.SetBumperCounterDirection(this);
				}
				return;
			}

			_itemFactory.Put(_window.Last.Value);
			_window.RemoveLast();

			var cutting = new Straight { Direction = -GrowDirection, };
			VectorGeneric2 intersection;

			var item = _itemFactory.Get(previous.Value);
			// TODO: one record cause en error here
			_window.First.Value.RectTransform.GetIntersectionInParentSpace(cutting, out intersection);
			_window.AddFirst(item);
			item.RectTransform.anchoredPosition = intersection.Target + intersection.Direction;

			if(ReferenceEquals(item.LinkedData, _bindings.First.Value))
				OnCounterDirectionBoundReach();
		}

		public void ShiftWindowDown()
		{
			// попадает ли хвост в диапазон и отправлен ли запрос
			if(!(_currentBindRequest is BindNextRequest) && _bindings.ContainsFromTail(_preBoundaryThreshold, _window.Last.Value.LinkedData))
			{
				_currentBindRequest = new BindNextRequest(this);
				_currentBindRequest.Execute();
			}

			var next = _bindings.Find(_window.Last.Value.LinkedData).Next;

			// while scrolling up (moving down)
			if(next == null)
			{
				var measureCutting = new Straight { Direction = GrowDirection };
				VectorGeneric2 firstIntersection;
				VectorGeneric2 lastIntersection;
				VectorGeneric2 boundIntersection;
				_window.First.Value.RectTransform.GetIntersectionInParentSpace(measureCutting, out firstIntersection);
				_window.Last.Value.RectTransform.GetIntersectionInParentSpace(measureCutting, out lastIntersection);
				RectTransform.GetIntersectionInLocalSpace(measureCutting, out boundIntersection);
				if(boundIntersection.Direction.sqrMagnitude < (lastIntersection.Target - firstIntersection.Origin).sqrMagnitude)
				{
					CarrierFactory.SetBumperCoDirection(this);
				}
				else
				{
					CarrierFactory.SetBumperCounterDirection(this);
				}
				return;
			}

			_itemFactory.Put(_window.First.Value);
			_window.RemoveFirst();

			var cutting = new Straight { Direction = GrowDirection, };
			VectorGeneric2 intersection;

			var item = _itemFactory.Get(next.Value);
			// TODO: one record cause en error here
			_window.Last.Value.RectTransform.GetIntersectionInParentSpace(cutting, out intersection);
			_window.AddLast(item);
			item.RectTransform.anchoredPosition = intersection.Target;

			if(ReferenceEquals(item.LinkedData, _bindings.Last.Value))
				OnCoDirectionBoundReach();
		}

		public void AppendToTop(IEnumerable<TData> set)
		{
			if(ReferenceEquals(null, set))
				return;

			if(_currentBindRequest is BindPreviousRequest)
			{
				_currentBindRequest.Merge(set);
			}

			var link = _window.Count > 0 ? _bindings.FindFromHead(_window.First.Value.LinkedData) : _bindings.Last;
			while(!ReferenceEquals(null, link))
			{
				TryAppendToWindowTop(link.Value);
				link = link.Previous;
			}

			// обнуляется он только когда есть запас записей
		   _currentBindRequest = _bindings.HeadContains(_preBoundaryThreshold, _window.First.Value.LinkedData) 
				? _currentBindRequest 
				: null;

			UnityEngine.Debug.Log(
				_bindings.ToText(
					string.Format(
						"<color=magenta>BINDINGS now are with request: {0}</color>", 
						_currentBindRequest == null ? "null" : _currentBindRequest.GetType().NameNice())));
		}

		private void TryAppendToWindowTop(TData data)
		{
			VectorGeneric2 intersection;
			if(data == null || !IsAllInside(out intersection, true) || _window.Count == 6)
				return;

			var item = _itemFactory.Get(data);
			item.RectTransform.anchoredPosition = intersection.Origin - intersection.Direction;
			_window.AddFirst(item);

			DebugWindow(item);
		}

		public void AppendToBottom(IEnumerable<TData> set)
		{
			if(ReferenceEquals(null, set))
				return;

			if(_currentBindRequest is BindNextRequest)
			{
				_currentBindRequest.Merge(set);
			}

			var link = _window.Count > 0 ? _bindings.FindFromTail(_window.Last.Value.LinkedData) : _bindings.First;
			while(!ReferenceEquals(null, link))
			{
				TryAppendToWindowBottom(link.Value);
				link = link.Next;
			}

			// обнуляется он только когда есть запас записей
			_currentBindRequest = _bindings.ContainsFromTail(_preBoundaryThreshold, _window.Last.Value.LinkedData) 
				? _currentBindRequest 
				: null;

			UnityEngine.Debug.Log(
				_bindings.ToText(
					string.Format(
						"<color=magenta>BINDINGS now are with request: {0}</color>", 
						_currentBindRequest == null ? "null" : _currentBindRequest.GetType().NameNice())));
		}

		private void TryAppendToWindowBottom(TData data)
		{
			VectorGeneric2 intersection;
			if(data == null || !IsAllInside(out intersection, false) || _window.Count == 6)
				return;

			var item = _itemFactory.Get(data);
			item.RectTransform.anchoredPosition = intersection.Target;
			_window.AddLast(item);

			DebugWindow(item);
		}

		private bool IsAllInside(out VectorGeneric2 lastItemIntersection, bool oppositeDirection)
		{
			var cutting = new Straight { Direction = GrowDirection, };
			var result = true;
			var intersection = new VectorGeneric2();
			var current = _window.Count > 0
				? oppositeDirection ? _window.Last : _window.First
				: null;
			while(!ReferenceEquals(null, current))
			{
				current.Value.RectTransform.GetIntersectionInParentSpace(cutting, out intersection);
				result = result &&
				         RectTransform.IsInsideLocalSpace(intersection.Origin) ||
				         RectTransform.IsInsideLocalSpace(intersection.Target);

				DebugWindow(current.Value);

				current = oppositeDirection ? current.Previous : current.Next;
			}
			lastItemIntersection = intersection;
			return result;
		}

		private void TryRemoveFromWindow(TData data) { }

		IEnumerator<IScrollItem<TData>> IEnumerable<IScrollItem<TData>>.GetEnumerator()
		{
			return _window.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _window.GetEnumerator();
		}

		[Conditional("UNITY_EDITOR")]
		private void DebugWindow(IScrollItem<TData> item)
		{
			var cutting = new Straight { Direction = GrowDirection, };
			VectorGeneric2 intersection;
			_window.Last.Value.RectTransform.GetIntersectionInParentSpace(cutting, out intersection);
			// UnityEngine.Debug.Log("<color=magenta> inside: " + intersection.Origin + " :: " + intersection.Target + "</color>");

			ScrollerExtensions.MultiLine(item.RectTransform, Color.yellow);
			ScrollerExtensions.Cross(intersection.Target, Color.magenta);
			ScrollerExtensions.Cross(intersection.Origin, Color.red);
			ScrollerExtensions.MultiLine(RectTransform.GetSidesInLocalSpace(), Color.yellow * .5f);
		}
	}
}
