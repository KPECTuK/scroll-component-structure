using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.Scripts.UtiltityComponents.Scroll.Extensions
{
	public static class TypeExtensions
	{
		private static readonly Regex _typeTemplate = new Regex("(?'name'[^`]*)");

		private static string WriteArgs(Type type)
		{
			return
				type.IsGenericType
					? type.GetGenericArguments()
					      .Aggregate(new StringBuilder(), (builder, _) => builder.Append(_.NameNice() + ", "))
					      .ToString()
					      .TrimEnd(", ".ToArray())
					: string.Empty;
		}

		public static string NameNice(this Type source)
		{
			if(source == null)
			{
				return "null";
			}

			return
				source.IsGenericType
					? string.Format("{0}<{1}>", _typeTemplate.Match(source.Name).Groups["name"].Value, WriteArgs(source)) 
					: source.Name;
		}
	}
}