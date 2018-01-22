using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UtiltityComponents.Scroll
{
	public partial class ScrollController<TData>
	{
		public float minWidth { get; private set; }
		public float preferredWidth { get; private set; }
		public float flexibleWidth { get; private set; }
		public float minHeight { get; private set; }
		public float preferredHeight { get; private set; }
		public float flexibleHeight { get; private set; }
		public int layoutPriority { get; private set; }

		// 

		public void Rebuild(CanvasUpdate executing)
		{
			Debug.Log("<color=red>Rebuild(CanvasUpdate executing)</color>");
		}

		public void LayoutComplete()
		{
			Debug.Log("<color=red>LayoutComplete()</color>");
		}

		public void GraphicUpdateComplete()
		{
			Debug.Log("<color=red>GraphicUpdateComplete()</color>");
		}

		public void OnButton()
		{
			Debug.Log("<color=red>OnButton()</color>");
		}

		// canvas layout

		public void CalculateLayoutInputHorizontal()
		{
			//Debug.Log("<color=green>CalculateLayoutInputHorizontal()</color>");
		}

		public void SetLayoutHorizontal()
		{
			//Debug.Log("<color=green>SetLayoutHorizontal()</color>");
		}

		public void CalculateLayoutInputVertical()
		{
			//Debug.Log("<color=green>CalculateLayoutInputVertical()</color>");
		}

		public void SetLayoutVertical()
		{
			//Debug.Log("<color=green>SetLayoutVertical()</color>");
		}
	}
}
