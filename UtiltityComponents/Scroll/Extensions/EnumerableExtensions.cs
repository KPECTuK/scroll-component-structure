using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.UtiltityComponents.Scroll.Extensions
{
	public static class EnumerableExtensions
	{
		public static string ToText<T>(this IEnumerable<T> source, string header = null, Func<T, string> renderer = null)
		{
			var rows = 0;
			var builder = new StringBuilder();
			renderer = renderer ?? (@object => @object.ToString());
			if(source == null)
				return
					(header != null ? string.Format("{0} [rows: {1}]\n{2}", header, rows, builder) : builder.ToString())
					.TrimEnd(Environment.NewLine.ToCharArray());
			foreach(var item in source)
				builder.AppendLine(item == null ? "null" : string.Format("{0}: {1}", ++rows, renderer(item)));
			return
				(header != null ? string.Format("{0} [rows: {1}]\n{2}", header, rows, builder) : builder.ToString())
				.TrimEnd(Environment.NewLine.ToCharArray());
		}

		public static string ToText(this IEnumerable source, string header = null, Func<object, string> renderer = null)
		{
			return ToText(source.Cast<object>(), header, renderer);
		}
	}
}