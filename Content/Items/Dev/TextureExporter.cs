using Macrocosm.Common.Sets;
using Macrocosm.Common.WorldGeneration;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Items.Dev
{
    class TextureExporter : ModItem
    {
        private static string BasePath => Program.SavePathShared + "/ModSources/Macrocosm/Content/WorldGeneration/Structures";

        private Point? topLeft = null;
        private Point? bottomRight = null;

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            ItemSets.DeveloperItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.UseSound = SoundID.Item6;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.AltFunction()) 
            {
                if (topLeft.HasValue && bottomRight.HasValue)
                {
                    int startX = topLeft.Value.X;
                    int startY = topLeft.Value.Y;
                    int width = bottomRight.Value.X - startX;
                    int height = bottomRight.Value.Y - startY;

                    string exportFolderPath = BasePath + "/" + GetNextIncrementalFolderName();
                    Directory.CreateDirectory(exportFolderPath); 

                    string tilePath = exportFolderPath + "/tiles.png";
                    string wallPath = exportFolderPath + "/walls.png";
                    string liquidPath = exportFolderPath + "/liquids.png";
                    string slopePath = exportFolderPath + "/slopes.png";
                    string tileColorMapPath = exportFolderPath + "/tileColorMap.json";
                    string wallColorMapPath = exportFolderPath + "/wallColorMap.json";
                    string objectMapPath = exportFolderPath + "/objectMap.json";

                    // Call export function
                    Tex2GenExporter.ExportRegion(
                        startX, startY, width, height,
                        tilePath, wallPath, liquidPath, slopePath,
                        tileColorMapPath, wallColorMapPath, objectMapPath
                    );

                    Main.NewText("Tex2Gen region exported successfully to " + exportFolderPath, Color.Green);
                    topLeft = null;
                    bottomRight = null;
                }
                else
                {
                    Main.NewText("Set both corners before exporting.", Color.Red);
                }
            }
            else // Left-click to set top-left or bottom-right
            {
                Point clickPosition = Main.MouseWorld.ToTileCoordinates();

                if (!topLeft.HasValue)
                {
                    topLeft = clickPosition;
                    Main.NewText($"Top-left corner set at {topLeft.Value}.", Color.Yellow);
                }
                else if (!bottomRight.HasValue)
                {
                    bottomRight = clickPosition;
                    Main.NewText($"Bottom-right corner set at {bottomRight.Value}.", Color.Yellow);

                    Tex2GenExporter.RegionRectangle = new Rectangle(
                        topLeft.Value.X,
                        topLeft.Value.Y,
                        bottomRight.Value.X - topLeft.Value.X,
                        bottomRight.Value.Y - topLeft.Value.Y
                    );
                    Tex2GenExporter.ShowExportRegion = true;
                }
                else
                {
                    topLeft = clickPosition;
                    bottomRight = null;
                    Main.NewText($"Top-left corner reset to {topLeft.Value}. Set the bottom-right next.", Color.Yellow);
                }
            }
            return true;
        }

        private string GetNextIncrementalFolderName()
        {
            int index = 1;
            string folderPath;
            do
            {
                folderPath = Path.Combine(BasePath, index.ToString());
                index++;
            } while (Directory.Exists(folderPath));
            return (index - 1).ToString(); // Adjust since index is incremented after finding an empty folder
        }
    }
}
