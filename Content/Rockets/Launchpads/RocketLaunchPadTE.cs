using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Localization;
using Terraria.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Rockets.Construction
{
    public class RocketLaunchPad : ModTile
    {
        const int width = 18;
        const int height = 1;

        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            MinPick = 1000;

            DustType = -1;
            HitSound = SoundID.Tink;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;

            TileObjectData.newTile.Origin = new Point16(0, height - 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, width, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<RocketLaunchPadTE>().Hook_AfterPlacement, -1, 0, false);

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(200, 200, 200), name);
        }

        public override bool CanPlace(int i, int j) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point16 origin = Utility.GetTileOrigin(i, j);
            ModContent.GetInstance<RocketLaunchPadTE>().Kill(origin.X, origin.Y);
        }

        public override bool RightClick(int i, int j)
        {
            Utility.UICloseOthers();

            if (Utility.TryGetTileEntityAs(i, j, out RocketLaunchPadTE entity) && Main.LocalPlayer.TryGetModPlayer(out RocketPlayer rocketPlayer))
            {
                //RocketSystem.Instance.ShowBuildingUI(new Point16(i, j));
                //return true;
            }

            return false;
        }
    }

    public class RocketLaunchPadTE : ModTileEntity
    {
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<RocketLaunchPad>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            TileObjectData tileData = TileObjectData.GetTileData(type, style, alternate);
            int topLeftX = i - tileData.Origin.X;
            int topLeftY = j - tileData.Origin.Y;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //Sync the entire multitile's area. 
                NetMessage.SendTileSquare(Main.myPlayer, topLeftX, topLeftY, tileData.Width, tileData.Height);

                //Sync the placement of the tile entity with other clients
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, topLeftX, topLeftY, Type);

                return -1;
            }

            //ModTileEntity.Place() handles checking if the entity can be placed, then places it
            int placedEntity = Place(topLeftX, topLeftY);

            if (Main.netMode == NetmodeID.SinglePlayer)
                PlaceRocket(new Vector2(topLeftX + 9, topLeftY + 16) * 16f);

            return placedEntity;
        }

        public Rocket PlaceRocket(Vector2 position)
        {
            return Rocket.Create(position);
        }

        public override void OnNetPlace()
        {
            int rocketId = PlaceRocket((Position + new Point16(9, 16)).ToVector2() * 16f).WhoAmI;
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, rocketId);
            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        }

        public override void OnInventoryDraw(Player player, SpriteBatch spriteBatch)
        {
            base.OnInventoryDraw(player, spriteBatch);
        }

        public override void OnKill()
        {
        }
    }
}
