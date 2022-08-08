namespace Macrocosm.Common.Drawing
{
	public interface ITexturedImmediate
	{
		bool TexLoaded { get; set; }

		void LoadTextures(); 
	}
}
