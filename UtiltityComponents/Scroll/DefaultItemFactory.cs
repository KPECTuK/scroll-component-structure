using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UtiltityComponents.Scroll.Contracts;
using UnityEngine;

namespace Assets.Scripts.UtiltityComponents.Scroll
{
	public class DefaultItemFactory<TData> : IItemFactory<TData> where TData : class
	{
		private readonly Stack<IScrollController<TData>> _framePocket = new Stack<IScrollController<TData>>(); 
		private readonly Stack<IScrollItem<TData>> _itemsCache = new Stack<IScrollItem<TData>>();
		private readonly IScrollController<TData> _container;

		public DefaultItemFactory(IScrollController<TData> container)
		{
			_container = container;
		} 

		// IItemFactory
		public IScrollItem<TData> Get(TData data)
		{
			var widget = _itemsCache.Count == 0
				? UnityEngine.Object.Instantiate(_container.ItemRendererPrefab, _container.RectTransform, false).GetComponent<IScrollItem<TData>>()
				: _itemsCache.Pop();
			//! remove
			widget.RectTransform.gameObject.SetActive(true);
			widget.Resume(data);
			return widget;
		}

		// IItemFactory
		public void Put(IScrollItem<TData> widget)
		{
			foreach(var item in widget.RectTransform.GetComponentsInChildren<IScrollItem<TData>>())
				item.Suspend();
			//! remove
			widget.RectTransform.gameObject.SetActive(false);
			_itemsCache.Push(widget);
		}

		// IItemFactory
		public void ClearAssetCache()
		{
			var group = _itemsCache.Select(_ => _ as MonoBehaviour).Where(_ => _ != null).ToArray();
			Array.ForEach(group, UnityEngine.Object.Destroy);
			_itemsCache.Clear();
		}

		public void MaintainCache()
		{
			// do cache opertions here (hiding unused)
		}
	}
}