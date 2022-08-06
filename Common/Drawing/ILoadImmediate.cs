using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Drawing
{
	public interface ITexturedImmediate
	{
		bool TexLoaded { get; set; }

		void LoadTextures(); 
	}
}
