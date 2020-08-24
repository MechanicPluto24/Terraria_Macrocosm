using System;
using System.IO;
using System.Windows.Forms;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Terraria;
using Terraria.ModLoader;
using Macrocosm;

namespace Macrocosm
{
	public class BaseShaderCompiler
	{
		/*public static string appDir = null;
		public static ContentManager manager = null;
		
		public static Effect CompileShader(Mod mod, string fileName, GraphicsDevice device = null)
		{
			try
			{
				if(device == null) device = Main.instance.GraphicsDevice;	
				if(appDir == null)
				{
					appDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\Content\\ModShaders";	
				    if (!Directory.Exists(appDir)) Directory.CreateDirectory(appDir);							
				}
				string fileDir = appDir + "\\" + mod.Name + "_" + fileName + ".xnb"; //moves the shader to this new file to be loaded.
				if(File.Exists(fileDir)) File.Delete(fileDir);	
				using (FileStream fs = File.Create(fileDir))
				{			
					byte[] info = mod.File.GetFile(fileName + ".xnb");					
					fs.Write(info, 0, info.Length);
				}
				if(manager == null) manager = new ContentManager(Main.instance.Content.ServiceProvider, "Content\\ModShaders");				
				Effect effect = manager.Load<Effect>(mod.Name + "_" + fileName);
				if(File.Exists(fileDir)) File.Delete(fileDir); //cleanup after
				return effect;
			}catch(Exception e){ ErrorLogger.Log("SHADER ERROR: " + e.Message); ErrorLogger.Log(e.StackTrace); ErrorLogger.Log("--------"); return null; }
		}*/
	}
}