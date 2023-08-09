using System.Collections.Generic;

namespace Macrocosm.Common.UI
{
	public interface IFocusable
	{
		public static Dictionary<string, IFocusable> FocusedElements { get; set; }
		public bool HasFocus => FocusedElements.ContainsValue(this);

		public void Focus(string context)
		{
			if (FocusedElements.ContainsKey(context))
 				FocusedElements[context].Unfocus(context);

			FocusedElements[context] = this;
		}

		public void Unfocus(string context)
		{
			FocusedElements.Remove(context);
		}
	}
}
