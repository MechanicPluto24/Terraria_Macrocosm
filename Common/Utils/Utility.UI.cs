using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Macrocosm.Common.Utils
{
	public partial class Utility
	{
		public static void ReplaceChildWith(this UIElement parent, UIElement toRemove, UIElement newElement)
		{
			parent.RemoveChild(toRemove);
			toRemove = newElement;
			parent.Append(toRemove);
			toRemove.Activate();
		}
	}
}
