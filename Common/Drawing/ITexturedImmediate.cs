namespace Macrocosm.Common.Drawing
{

	/// <summary>
	/// Implement this for classes that have Texture2D fields
	/// and their sizes are used in the code 
	/// </summary>
	public interface ITexturedImmediate
	{
		/// <summary>
		/// Set to true when textures were loaded
		/// </summary>
		bool TexLoaded { get; set; }

		/// <summary>
		/// Call this in the constructor/initializer/draw method, conditioned by TexLoaded 
		/// Load all required textures with AssetRequestMode.ImmediateLoad
		/// </summary>
		void LoadTextures();
	}
}
