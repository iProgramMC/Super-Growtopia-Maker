using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SuperGrowtopiaMaker
{
    enum GameState
    {
        EditMode,
        PlayMode,
        TitleScr
    }

    enum TileCollision : int
    {
        None = 0,
        Solid = 8,
        Lava = 4,
        Death = 162,
        Gem = 112
    }

    class EditedTile
    {
        public int x;
        public int y;
        public int prev_id;
        public bool fg_layer;


        public EditedTile(int x, int y, int prev_id, bool fg_layer)
        {
            this.x = x; this.y = y; this.prev_id = prev_id; this.fg_layer = fg_layer;
        }

        public void ApplyChanges(List<int> map, int mapwidth)
        {
            map[y * mapwidth + x] = prev_id;
        }
    }

    class TileData
    {
        public int apX;
        public int apY;

        public byte[] data;
    }

    public class SGM : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

	    // Level storage
        List<int> sLevel_FG = new List<int>(6000);
        List<int> sLevel_BG = new List<int>(6000);

        List<bool> spriteEffectBlocks = new List<bool>(6000);

        List<EditedTile> editedTiles = new List<EditedTile>();

        public int AnimationStatus = 0;

        public bool DoAnimation = true;

        public bool ApplyDrag = true;

        public double AnimationDuration = 0.2f;
        public double HowLongTillSwitchAnim = 0.0f;

        List<TileData> tileData = new List<TileData>(6000);

	    int nLevelWidth;
	    int nLevelHeight;

        string sGamerTag = "@iProgramInCpp";

        bool IsGridEnabled = true;
        bool IsUIEnabled = true;

        int SelectedItem = 0;

        GameState gameState = GameState.EditMode;

        const float fGravityFactor = 20.0f;

	    // Player Properties
	    float fPlayerPosX = 50.0f;
	    float fPlayerPosY = 53.0f;
	    float fPlayerVelX = 0.0f;
	    float fPlayerVelY = 0.0f;
	    bool bPlayerOnGround = false;

        bool bIsSuperSupporter = true;

	    // Camera properties
	    float fCameraPosX = 50.0f;
	    float fCameraPosY = 53.0f;

        float scale_screen = 1f;

        Texture2D spriteMan;
        Texture2D spriteTiles;

        Texture2D CreateBtnTex;
        Texture2D PlayBtnTex;
        Texture2D MakerHeader;
        Texture2D HideShowUIBtn;
        Texture2D checkmarkTexture;
        Texture2D NextTexture;

        TimeSpan tRespawnTimeout = new TimeSpan(0, 0, 0);
        TimeSpan tMaxRespTimeout = new TimeSpan(0, 0, 3);

        Rectangle GridCheckBox = new Rectangle(10, 22, 16, 16);
        Rectangle HideUICheckBox = new Rectangle(375, 50, 50, 16);
        Rectangle ShowUICheckBox = new Rectangle(375, 0,  50, 16);
        Rectangle NextBox = new Rectangle(775, 15, 8, 15);

        string MessageBoxMsg = "";

        int GemsCount = 0;

        int nCheckpointX = 0;
        int nCheckpointY = 0;

        bool bDead = false;

        Texture2D Tiles_Page1;

        SpriteFont sf;
        SpriteFont sf_s2;
        SpriteFont sf_namess;

	    // Sprite selection flags
	    int nDirModX = 0;
	    int nDirModY = 0;

        void GenerateBaseWorld()
        {

            for (int x = 0; x < 5400; x++)
            {
                sLevel_FG.Add(0);
            }
            for (int x = 0; x < 600; x++)
            {
                sLevel_FG.Add(8);
            }

            for (int x = 0; x < 5400; x++)
            {
                sLevel_BG.Add(0);
            }
            for (int x = 0; x < 600; x++)
            {
                sLevel_BG.Add(14);
            }

            SetTile(fPlayerPosX, fPlayerPosY, 6);
            SetTile(fPlayerPosX, fPlayerPosY + 1, 8);

            for (int x = 0; x < 6000; x++)
            {
                spriteEffectBlocks.Add(false);
            }
        }

        public SGM()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            //Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            
		    nLevelWidth = 100;
		    nLevelHeight = 60;

            GenerateBaseWorld();

            prevKbState = new KeyboardState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteMan = Content.Load<Texture2D>(bIsSuperSupporter ? "minijario_ss" : "minijario");
            spriteTiles = Content.Load<Texture2D>("leveljario");
            Tiles_Page1 = Content.Load<Texture2D>("tiles");
            MakerHeader = Content.Load<Texture2D>("maker_ui_top");
            CreateBtnTex = Content.Load<Texture2D>("create_button");
            PlayBtnTex = Content.Load<Texture2D>("play_button");
            checkmarkTexture = Content.Load<Texture2D>("checkmark");
            HideShowUIBtn = Content.Load<Texture2D>("hide_show_ui_button");
            NextTexture = Content.Load<Texture2D>("Next");

            sf = Content.Load<SpriteFont>("Fonts/hud");
            sf_s2 = Content.Load<SpriteFont>("Fonts/hud2");
            sf_namess = Content.Load<SpriteFont>("Fonts/hudss");

        }



		int GetTile (float x, float y)
		{
            int nX = (int)Math.Floor(x);
            int nY = (int)Math.Floor(y);
			if (nX >= 0 && nX < nLevelWidth && y >= 0 && y < nLevelHeight)
				return sLevel_FG[(int)(nY * nLevelWidth + nX)];
			else
				return -1;
		}

        int GetTile(int x, int y)
        {
            if (x >= 0 && x < nLevelWidth && y >= 0 && y < nLevelHeight)
                return sLevel_FG[y * nLevelWidth + x];
            else
                return ' ';
        }

        int GetTileBG(float x, float y)
        {
            int nX = (int)Math.Floor(x);
            int nY = (int)Math.Floor(y);
            if (nX >= 0 && nX < nLevelWidth && y >= 0 && y < nLevelHeight)
                return sLevel_BG[(int)(nY * nLevelWidth + nX)];
            else
                return -1;
        }

        int GetTileBG(int x, int y)
        {
            if (x >= 0 && x < nLevelWidth && y >= 0 && y < nLevelHeight)
                return sLevel_BG[y * nLevelWidth + x];
            else
                return ' ';
        }

        void SetTile(int x, int y, int c)
        {
            if (x >= 0 && x < nLevelWidth && y >= 0 && y < nLevelHeight)
            {
                if (gameState == GameState.PlayMode)
                {
                    editedTiles.Add(new EditedTile(x, y, sLevel_FG[(int)(y * nLevelWidth + x)], true));
                }
                sLevel_FG[y * nLevelWidth + x] = c;
            }
            //sLevel_FG[(int)Math.Floor(y)*nLevelWidth + (int)Math.Floor(x)] = c;
        }

		void SetTile (float x, float y, int c)
		{
            int nX = (int)Math.Floor(x); int nY = (int)Math.Floor(y);
            if (nX >= 0 && nX < nLevelWidth && nY >= 0 && nY < nLevelHeight)
            {
                if (gameState == GameState.PlayMode)
                {
                    editedTiles.Add(new EditedTile(nX, nY, sLevel_FG[(int)(nY * nLevelWidth + nX)], true));
                }
                sLevel_FG[(int)(nY * nLevelWidth + nX)] = c;
            }
				//sLevel_FG[(int)Math.Floor(y)*nLevelWidth + (int)Math.Floor(x)] = c;
        }

        void SetTileBG(int x, int y, int c)
        {
            if (x >= 0 && x < nLevelWidth && y >= 0 && y < nLevelHeight)
            {
                if (gameState == GameState.PlayMode)
                {
                    editedTiles.Add(new EditedTile(x, y, sLevel_BG[(int)(y * nLevelWidth + x)], false));
                }
                sLevel_BG[y * nLevelWidth + x] = c;
            }
            //sLevel_FG[(int)Math.Floor(y)*nLevelWidth + (int)Math.Floor(x)] = c;
        }

        void Respawn()
        {
            nDirModX = 5;
            bDead = true;

            tRespawnTimeout = new TimeSpan(0, 0, 0);
        }

        void SetTileBG(float x, float y, int c)
        {
            int nX = (int)Math.Floor(x); int nY = (int)Math.Floor(y);
            if (nX >= 0 && nX < nLevelWidth && nY >= 0 && nY < nLevelHeight)
            {
                if (gameState == GameState.PlayMode)
                {
                    editedTiles.Add(new EditedTile(nX, nY, sLevel_FG[(int)(nY * nLevelWidth + nX)], false));
                }
                sLevel_BG[(int)(nY * nLevelWidth + nX)] = c;
            }
            //sLevel_FG[(int)Math.Floor(y)*nLevelWidth + (int)Math.Floor(x)] = c;
        }


        KeyboardState prevKbState = new KeyboardState();

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        void Save_World()
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();

            sfd.FileName = "world.smw";
            sfd.Filter = "Super Growtopia Maker World (*.smw)|*.smw";

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream str = null;
                try
                {
                    str = new FileStream(sfd.FileName, FileMode.CreateNew);
                }
                catch
                {
                    str = new FileStream(sfd.FileName, FileMode.Open, FileAccess.ReadWrite);
                }

                using (var arc = ZipArchive.OpenOnStream(str))
                {
                    using (var fs = arc.AddFile("bg").GetStream
                          (FileMode.Open, FileAccess.ReadWrite))
                    {
                        DataHandler.SaveData(sLevel_BG, fs);
                    }
                    using (var fs = arc.AddFile("fg").GetStream
                          (FileMode.Open, FileAccess.ReadWrite))
                    {
                        DataHandler.SaveData(sLevel_FG, fs);
                    }
                    using (var fs = arc.AddFile("fbl").GetStream
                          (FileMode.Open, FileAccess.ReadWrite))
                    {
                        DataHandler.SaveData(spriteEffectBlocks, fs);
                    }
                    using (var fs = arc.AddFile("note.txt").GetStream
                          (FileMode.Open, FileAccess.ReadWrite))
                    {
                        char[] buffed;
                        byte[] buffer;
                        string note = "You are cool if you see this :)";

                        buffed = note.ToCharArray();
                        buffer = new byte[buffed.Length];

                        for (int x = 0; x < buffed.Length; x++)
                        {
                            buffer[x] = (byte)buffed[x];
                        }

                        fs.Write(buffer, 0, buffer.Length);
                    }
                }

                str.Close();
            }
        }

        void Load_World()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            ofd.Filter = "Super Growtopia Maker World (*.smw)|*.smw";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream str = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite);

                using (var arc = ZipArchive.OpenOnStream(str))
                {
                    sLevel_BG = (List<int>)DataHandler.LoadData(arc.GetFile("bg").GetStream());
                    sLevel_FG = (List<int>)DataHandler.LoadData(arc.GetFile("fg").GetStream());
                    spriteEffectBlocks = (List<bool>)DataHandler.LoadData(arc.GetFile("fbl").GetStream());
                }
            }
        }

        int GetCollision(int id, byte dir, bool isSpacePressed)
        {
            switch (id)
            {
                case 0:
                case 6:
                case 12:
                case 14:
                case 16:
                case 20:
                case 22:
                case 24:
                case 26:
                case 28:
                case 30:
                case 52:
                case 60:
                case 62:
                case 106:
                case 108:
                case 110:
                case 114:
                case 118:
                case 224:
                case 246:
                case 252:
                case 254:
                    return 0;
                case 4:
                case 32:
                    //Bounce Player
                    float fPlayerVelYSign = (/* get sign */ fPlayerVelY / Math.Abs(fPlayerVelY));

                    fPlayerVelX = 10f * (/* get sign */ fPlayerVelX / Math.Abs(fPlayerVelX));
                    fPlayerVelY = 10f * ((fPlayerVelYSign > 0) ? -1f : 1f);
                    return 194;
                case 162: //Kill

                    if (!bDead)
                    {
                        Respawn();
                    }
                    return 8;
                case 102:
                    if ((dir & 0x08) > 0) //DOWN
                    {
                        return 8;
                    }
                    return 0;
                case 112:
                    return 112;
                case 194:
                    if ((dir == 0x08))
                    {
                        //fPlayerPosY += 1f;
                        if (isSpacePressed)
                        {
                            fPlayerVelY = -20f;
                            nDirModX = 1;
                        }
                        else
                        {
                            fPlayerVelY = -10f;
                            nDirModX = 1;
                        }
                    }
                    return 194;
                default:
                    return 8;
            }
        }

        bool IsFocused = true;
        protected override void OnActivated(object sender, EventArgs args)
        {
            IsFocused = true;
            //base.OnActivated(sender, args);
        }
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            IsFocused = false;
            //base.OnActivated(sender, args);
        }

        void StartPlayMode()
        {
            gameState = GameState.PlayMode;

            for (int y = 0; y < nLevelHeight - 6; y++)
            {
                for (int x = 0; x < nLevelWidth; x++)
                {
                    if (GetTile(x, y) == 6)
                    {
                        fPlayerPosX = x;
                        fPlayerPosY = y;

                        nCheckpointX = x;
                        nCheckpointY = y;
                    }
                }
            }


            GemsCount = 0;
        }

        void StartEditMode()
        {
            gameState = GameState.EditMode;

            for (int y = 0; y < nLevelHeight - 6; y++)
            {
                for (int x = 0; x < nLevelWidth; x++)
                {
                    if (GetTile(x, y) == 6)
                    {
                        fPlayerPosX = x;
                        fPlayerPosY = y;
                    }
                }
            }

            foreach (EditedTile editedTile in editedTiles)
            {
                if (editedTile.fg_layer)
                {
                    editedTile.ApplyChanges(sLevel_FG, 100);
                }
                else
                {
                    editedTile.ApplyChanges(sLevel_BG, 100);
                }
            }
            editedTiles.Clear();
        }

        void FlipTile(int x, int y)
        {
            if(x < nLevelWidth && x > -1 &&
               y < nLevelHeight && y > -1){
                
                if (!spriteEffectBlocks[y * nLevelWidth + x])
                {
                    spriteEffectBlocks[y * nLevelWidth + x] = true;
                }
                else
                {
                    spriteEffectBlocks[y * nLevelWidth + x] = false;
                }
            }
        }

        void FlipTile(float fX, float fY)
        {
            FlipTile((int)Math.Floor(fX), (int)Math.Floor(fY));
        }

        Vector2 MouseToWorld(Vector2 mouse)
        {
            Vector2 temp = new Vector2(-1, -1);

            int nVisibleTilesX = ScreenWidth() / nTileWidth;
            int nVisibleTilesY = ScreenHeight() / nTileHeight;


            float fOffsetX = fCameraPosX - (float)nVisibleTilesX / 2.0f;
            float fOffsetY = fCameraPosY - (float)nVisibleTilesY / 2.0f;

            // Clamp camera to game boundaries
            if (fOffsetX < 0) fOffsetX = 0;
            if (fOffsetY < 0) fOffsetY = 0;
            if (fOffsetX > nLevelWidth - nVisibleTilesX) fOffsetX = nLevelWidth - nVisibleTilesX;
            if (fOffsetY > nLevelHeight - nVisibleTilesY) fOffsetY = nLevelHeight - nVisibleTilesY;

            // Get offsets for smooth movement
            float fTileOffsetX = (fOffsetX - (int)fOffsetX) * nTileWidth;
            float fTileOffsetY = (fOffsetY - (int)fOffsetY) * nTileHeight;


            for (int x = -3; x < nVisibleTilesX + 3; x++)
            {
                for (int y = -3; y < nVisibleTilesY + 3; y++)
                {
                    if(new Rectangle((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), nTileWidth, nTileHeight).Contains(new Point((int)mouse.X, (int)mouse.Y))){
                        return new Vector2(x+fOffsetX, y+fOffsetY);
                    }
                }
            }

            return temp;
        }

        void FilterItemID(ref int ID)
        {
            while (BuildingBlocks.GetRectangleForTile(ID, DrawPurpose.Show, 0, 0, 0, 0, AnimationStatus).Width == 1 &&
                  BuildingBlocks.GetRectangleForTile(ID, DrawPurpose.Show, 0, 0, 0, 0, AnimationStatus).Height == 1)
            {
                ID++;
                if (ID == 8)
                {
                    ID++;
                }
                if (ID >= 3000)
                {
                    ID = 0;
                }
            }
            /*if (ID == 8)
            {
                ID = 10;
            }
            if (ID == 34)
            {
                ID = 52;
            }
            if (ID == 66)
            {
                ID = 100;
            }
            if (ID == 122)
            {
                ID = 162;
            }*/
        }

        bool IsFlippable(int ID)
        {
            switch (ID)
            {
                case 218:
                case 230:
                case 246:
                case 254:
                    return true;
                default:
                    return false;
            }
        }

        void FilterItemIDMinus(ref int ID)
        {
            while (BuildingBlocks.GetRectangleForTile(ID, DrawPurpose.Show, 0, 0, 0, 0, AnimationStatus).Width == 1 &&
                  BuildingBlocks.GetRectangleForTile(ID, DrawPurpose.Show, 0, 0, 0, 0, AnimationStatus).Height == 1)
            {
                ID--;
                if (ID == 8)
                {
                    ID++;
                }
                if (ID < 0)
                {
                    ID = 0;
                }
            }
        }

        int GetCategory(int ID)
        {
            switch (ID)
            {
                case 20:
                    return 1; // Sign
                case 30:
                    return 2; // Door
                default:
                    return 0; // Either a solid tile or a non passable tile
            }
        }

        bool IsBackground(int ID)
        {
            switch (ID)
            {
                case 14:
                case 18:
                case 52:
                case 54:
                case 56:
                case 58:
                case 104:
                case 118:
                    return true;
                default:
                    return false;
            }
        }

        bool doCollisionRoutine = false;

        MouseState prevMouseState = new MouseState();
        protected override void Update(GameTime gameTime)
        {
            if (bDead)
            {
                tRespawnTimeout += gameTime.ElapsedGameTime;
                if (tRespawnTimeout >= new TimeSpan(0,0,3))
                {
                    bDead = false;
                    tRespawnTimeout = new TimeSpan(0, 0, 0);
                    nDirModX = 0;

                    fPlayerPosX = nCheckpointX;
                    fPlayerPosY = nCheckpointY;
                }
            }
            else
            {
                tRespawnTimeout = new TimeSpan(0, 0, 0);
            }

            //Do Animations
            if (DoAnimation)
            {
                HowLongTillSwitchAnim += gameTime.ElapsedGameTime.TotalSeconds;

                if (HowLongTillSwitchAnim >= AnimationDuration)
                {
                    //Switch animation
                    HowLongTillSwitchAnim -= AnimationDuration;

                    AnimationStatus = (AnimationStatus == 1) ? 2 : 1;
                }
            }

            //scale_screen = GraphicsDevice.Viewport.Width / 800f;

            MouseState mouseState = Mouse.GetState();

            Point mPos = new Point(mouseState.X, mouseState.Y);

            Vector2 TilePressed = MouseToWorld(new Vector2(mouseState.X, mouseState.Y));

            if (mouseState.LeftButton == ButtonState.Pressed && gameState == GameState.EditMode)
            {
                if((mPos.Y < 60 && IsUIEnabled) || (mPos.Y < 10 && !IsUIEnabled))
                {
                    //for(int x = 0; x 

                    int CurrentDrawnItem = StartingDrawItem;
                    for (int x = 0; x < 17; x++)
                    {
                        FilterItemID(ref CurrentDrawnItem);

                        Rectangle BoundingBox = new Rectangle(150 + x * 36, 10, 32, 32);
                        if (BoundingBox.Contains(mPos))
                        {
                            //it is clicked
                            SelectedItem = CurrentDrawnItem;
                        }

                        CurrentDrawnItem += 2;

                        FilterItemID(ref CurrentDrawnItem);
                    }

                    if (NextBox.Contains(new Point(mouseState.X, mouseState.Y)))
                    {
                        if (prevMouseState.LeftButton != ButtonState.Pressed)
                        {
                            StartingDrawItem += 2;
                            FilterItemID(ref StartingDrawItem);
                        }
                    }
                    if(GridCheckBox.Contains(new Point(mouseState.X, mouseState.Y))){
                        if (prevMouseState.LeftButton != ButtonState.Pressed)
                        {
                            IsGridEnabled = !IsGridEnabled;
                        }
                    }
                    else if (HideUICheckBox.Contains(new Point(mouseState.X, mouseState.Y)) && IsUIEnabled)
                    {
                        if (prevMouseState.LeftButton != ButtonState.Pressed)
                        {
                            IsUIEnabled = !IsUIEnabled;
                        }
                    }
                    else if (ShowUICheckBox.Contains(new Point(mouseState.X, mouseState.Y)) && !IsUIEnabled)
                    {
                        if (prevMouseState.LeftButton != ButtonState.Pressed)
                        {
                            IsUIEnabled = !IsUIEnabled;
                        }
                    }
                }
                else
                {
                    if (SelectedItem == 6)
                    {
                        for (int y = 0; y < nLevelHeight - 6; y++)
                        {
                            for (int x = 0; x < nLevelWidth; x++)
                            {
                                if (GetTile(x, y) == 6)
                                {
                                    SetTile(x, y, 0);
                                    if (y + 1 < 54)
                                    {
                                        SetTile(x, y + 1, 0);
                                    }
                                }
                            }
                        }
                        if (TilePressed.Y < 54)
                        {
                            SetTile(TilePressed.X, TilePressed.Y, 6);
                            SetTile(TilePressed.X, TilePressed.Y + 1, 8);
                        }
                    }
                    else if (SelectedItem == 18)
                    {
                        SetTileBG(TilePressed.X, TilePressed.Y, 0);
                    }
                    else if (SelectedItem == 2966)
                    {
                        if (prevMouseState.LeftButton != ButtonState.Pressed && IsFlippable(GetTile(TilePressed.X, TilePressed.Y)))
                        {
                            FlipTile(TilePressed.X, TilePressed.Y);
                        }
                    }
                    else
                    {
                        if (TilePressed.Y < 54 && GetTile(TilePressed.X, TilePressed.Y) != 6 && GetTile(TilePressed.X, TilePressed.Y) != 8 && !IsBackground(SelectedItem))
                        {
                            SetTile(TilePressed.X, TilePressed.Y, SelectedItem);
                        }
                        if (TilePressed.Y < 54 && GetTile(TilePressed.X, TilePressed.Y) != 6 && GetTile(TilePressed.X, TilePressed.Y) != 8 && IsBackground(SelectedItem))
                        {
                            SetTileBG(TilePressed.X, TilePressed.Y, SelectedItem);
                        }

                        if (!IsFlippable(SelectedItem))
                        {
                            if (GetTileFlipping(TilePressed.X, TilePressed.Y) != SpriteEffects.None)
                            {
                                FlipTile(TilePressed.X, TilePressed.Y);
                            }
                        }
                    }
                }
            }

            //float fElapsedTime = 0.0164f;
            KeyboardState kbState = Keyboard.GetState();

            if (gameState == GameState.EditMode)
            {

                    if (kbState.IsKeyDown(Keys.Up))
                    {
                        fCameraPosY -= 0.1f;
                    }
                    if (kbState.IsKeyDown(Keys.Down))
                    {
                        fCameraPosY += 0.1f;
                    }
                    if (kbState.IsKeyDown(Keys.Left))
                    {
                        fCameraPosX -= 0.1f;
                    }
                    if (kbState.IsKeyDown(Keys.Right))
                    {
                        fCameraPosX += 0.1f;
                    }

                    if (kbState.IsKeyDown(Keys.F3) && prevKbState.IsKeyDown(Keys.F3))
                    {
                        Save_World();
                    }
                    if (kbState.IsKeyDown(Keys.F4) && prevKbState.IsKeyDown(Keys.F4))
                    {
                        Load_World();
                    }


                if (kbState.IsKeyDown(Keys.F5) && !prevKbState.IsKeyDown(Keys.F5))
                {
                    StartPlayMode();
                }

                if (kbState.IsKeyDown(Keys.OemPlus) && !prevKbState.IsKeyDown(Keys.OemPlus))
                {
                    //StartingDrawItem += 17 * 2;
                    //FilterItemID(ref StartingDrawItem);
                }
                if (kbState.IsKeyDown(Keys.OemMinus) && !prevKbState.IsKeyDown(Keys.OemMinus))
                {
                    //StartingDrawItem -= 17 * 2;
                    //FilterItemIDMinus(ref StartingDrawItem);
                }

                if (StartingDrawItem < 0) StartingDrawItem = 0;

            }
            else if (gameState == GameState.PlayMode)
            {
                if (kbState.IsKeyDown(Keys.F5) && !prevKbState.IsKeyDown(Keys.F5))
                {
                    StartEditMode();
                }

                //fPlayerVelX = 0.0f;
                fPlayerVelY += fGravityFactor * 0.0164f;


                if (IsFocused)
                {
                    if (bDead)
                    { }
                    else
                    {
                        if (kbState.IsKeyDown(Keys.N))
                        {
                            doCollisionRoutine = false;
                        }
                        else
                        {
                            doCollisionRoutine = true;
                        }

                        if (kbState.IsKeyDown(Keys.Up))
                        {
                            fPlayerVelY = -6.0f;
                        }
                        if (kbState.IsKeyDown(Keys.Down))
                        {
                            fPlayerVelY = 6.0f;
                        }
                        if (kbState.IsKeyDown(Keys.Left))
                        {
                            fPlayerVelX += (bPlayerOnGround) ? -2.0f : -2.0f;// *0.0164f;
                            nDirModY = 1;
                        }
                        if (kbState.IsKeyDown(Keys.Right))
                        {
                            fPlayerVelX += (bPlayerOnGround) ? 2.0f : 2.0f; // *0.0164f
                            nDirModY = 0;
                        }
                        if (kbState.IsKeyDown(Keys.Space) && !prevKbState.IsKeyDown(Keys.Space))
                        {
                            if (bPlayerOnGround)
                            {
                                fPlayerVelY = -12.0f;
                                if (bDead)
                                {
                                    nDirModX = 5;
                                }
                                else
                                {
                                    nDirModX = 1;
                                }
                            }
                            //fPlayerVelX += 6.0f * 0.0164f;
                        }
                    }
                }
                //if (bPlayerOnGround)
                //{
                    fPlayerVelX += -15.0f * fPlayerVelX * 0.0164f;
                    if (Math.Abs(fPlayerVelX) < 0.01f)
                        fPlayerVelX = 0.0f;
                //}

                if (fPlayerVelX > 100.0f)
                    fPlayerVelX = 100.0f;

                if (fPlayerVelX < -100.0f)
                    fPlayerVelX = -100.0f;

                if (fPlayerVelY > 30.0f)
                    fPlayerVelY = 30.0f;

                if (fPlayerVelY < -30.0f)
                    fPlayerVelY = -30.0f;

                if (float.IsNaN(fPlayerVelX))
                {
                    fPlayerVelX = 0;
                }
                if (float.IsNaN(fPlayerVelY))
                {
                    fPlayerVelY = 0;
                }
                if (float.IsNaN(fPlayerPosX))
                {
                    fPlayerPosX = 0;
                }
                if (float.IsNaN(fPlayerPosY))
                {
                    fPlayerPosY = 0;
                }

                float fNewPlayerPosX = fPlayerPosX + fPlayerVelX * 0.0164f;
                float fNewPlayerPosY = fPlayerPosY + fPlayerVelY * 0.0164f;

                bool isSpacePressed = kbState.IsKeyDown(Keys.Space);

                    if (GetCollision(GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 0.0f), 0, isSpacePressed) == 112)
                    {
                        SetTile((int)fNewPlayerPosX + 0.0f, (int)(int)fNewPlayerPosY + 0.0f, 0);
                        GemsCount += 1;
                    }

                    if (GetCollision(GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f), 0, isSpacePressed) == 112)
                    {
                        SetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f, 0);
                        GemsCount += 1;
                    }

                    if (GetCollision(GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f), 0, isSpacePressed) == 112)
                    {
                        SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f, 0);
                        GemsCount += 1;
                    }

                    if (GetCollision(GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f), 0, isSpacePressed) == 112)
                    {
                        SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f, 0);
                        GemsCount += 1;
                    }


                    if (doCollisionRoutine)
                    {
                        //Collision
                        if (fPlayerVelX < 0)
                        {
                            if (GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 0.0f)), 0x01, isSpacePressed) != 0 ||
                                GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 0.8f)), 0x01, isSpacePressed) != 0)
                            {
                                fNewPlayerPosX = (int)Math.Floor(fNewPlayerPosX + 1);
                                fPlayerVelX = 0;
                            }
                        }
                        else if (fPlayerVelX > 0)
                        {
                            if (GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 1.0f), (int)Math.Floor(fPlayerPosY + 0.0f)), 0x02, isSpacePressed) != 0 ||
                                GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 1.0f), (int)Math.Floor(fPlayerPosY + 0.8f)), 0x02, isSpacePressed) != 0)
                            {
                                fNewPlayerPosX = (int)Math.Floor(fNewPlayerPosX);
                                fPlayerVelX = 0;
                            }
                        }

                        bPlayerOnGround = false;
                        if (fPlayerVelY < 0)
                        {
                            if (GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY - 0.1f)), 0x04, isSpacePressed) != 0 ||
                                GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY - 0.1f)), 0x04, isSpacePressed) != 0)
                            {
                                fNewPlayerPosY = (float)Math.Floor(fNewPlayerPosY + 1);
                                fPlayerVelY = 0;
                            }
                        }
                        else if (fPlayerVelY > 0)
                        {
                            if (GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 1f)), 0x08, isSpacePressed) != 0 ||
                                GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY + 1f)), 0x08, isSpacePressed) != 0)
                            {
                                fNewPlayerPosY = (int)Math.Floor(fNewPlayerPosY);
                                if (GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 1f)), 0x08, isSpacePressed) != 194 ||
                                    GetCollision(GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY + 1f)), 0x08, isSpacePressed) != 194)
                                {
                                    fPlayerVelY = 0;
                                    bPlayerOnGround = true;
                                    nDirModX = 0;
                                }
                                if (bDead)
                                {
                                    nDirModX = 5;
                                }
                            }
                        }
                        fPlayerPosX = fNewPlayerPosX;
                        fPlayerPosY = fNewPlayerPosY;
                        if (GetCollision(GetTile((int)Math.Floor(fPlayerPosX + 0.5f), (int)Math.Floor(fPlayerPosY + 0.5f)), 0x01, isSpacePressed) != 0 ||
                            GetCollision(GetTile((int)Math.Floor(fPlayerPosX + 0.5f), (int)Math.Floor(fPlayerPosY + 0.5f)), 0x01, isSpacePressed) != 0)
                        {
                            if (!bDead)
                            {
                                Respawn();
                            }
                        }
                    }
                    else
                    {
                        fPlayerPosX = fNewPlayerPosX;
                        fPlayerPosY = fNewPlayerPosY;
                    }




                fCameraPosX = fPlayerPosX;
                fCameraPosY = fPlayerPosY;

            }


            prevKbState = kbState;
            prevMouseState = mouseState;

            int nVisibleTilesX = ScreenWidth() / nTileWidth;
            int nVisibleTilesY = ScreenHeight() / nTileHeight;


            if (fCameraPosY < (float)nVisibleTilesY / 2.0f)
            {
                fCameraPosY = (float)nVisibleTilesY / 2.0f;
            }
            

            if (fCameraPosX < (float)nVisibleTilesX / 2.0f)
            {
                fCameraPosX = (float)nVisibleTilesX / 2.0f;
            }

            if (fCameraPosX > (float)(nLevelWidth - nVisibleTilesX / 2.0f))
            {
                fCameraPosX = (float)(nLevelWidth - nVisibleTilesX / 2.0f);
            }
            if (fCameraPosY > (float)(nLevelHeight - nVisibleTilesY / 2.0f))
            {
                fCameraPosY = (float)(nLevelHeight - nVisibleTilesY / 2.0f);
            }

            base.Update(gameTime);
        }

        int nTileWidth = 32;
        int nTileHeight = 32;

        int nTileDrawWidth = 32;
        int nTileDrawHeight = 32;

        public int ScreenWidth()
        {

            return (int)(GraphicsDevice.Viewport.Width / scale_screen);
        }
        public int ScreenHeight()
        {
            return (int)(GraphicsDevice.Viewport.Height / scale_screen);
        }

        void Fill(Rectangle rect, Color color) { spriteBatch.Draw(Tiles_Page1, rect, new Rectangle(817, 465, 1, 1), color); }

        public void DrawPartialSprite(int x, int y, int w, int h, Texture2D texture, int ox, int oy, int ow, int oh) { spriteBatch.Draw(texture, new Rectangle(x, y, w, h), new Rectangle(ox, oy, ow, oh), Color.White); }
        public void DrawPlayerSprite(int x, int y, int w, int h, Texture2D texture, int ox, int oy, int ow, int oh) { spriteBatch.Draw(texture, new Rectangle(x, y, w, h), new Rectangle(ox, oy, ow, oh), doCollisionRoutine ? Color.White : Color.FromNonPremultiplied(255, 255, 255, 100)); }

        int StartingDrawItem = 0;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Vector2 TilePressed = MouseToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0,255,255,255));

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp,
                null, null, null, Matrix.CreateScale(scale_screen));
            // Draw Level
            int nVisibleTilesX = ScreenWidth() / nTileDrawWidth;
            int nVisibleTilesY = ScreenHeight() / nTileHeight;

            // Calculate Top-Leftmost visible tile
            float fOffsetX = fCameraPosX - (float)nVisibleTilesX / 2.0f;
            float fOffsetY = fCameraPosY - (float)nVisibleTilesY / 2.0f;

            // Clamp camera to game boundaries
            if (fOffsetX < 0f) fOffsetX = 0;
            if (fOffsetY < 0f) fOffsetY = 0;
            if (fOffsetX > nLevelWidth - nVisibleTilesX) fOffsetX = nLevelWidth - nVisibleTilesX;
            if (fOffsetY > nLevelHeight - nVisibleTilesY) fOffsetY = nLevelHeight - nVisibleTilesY;

            // Get offsets for smooth movement
            float fTileOffsetX = (fOffsetX - (int)fOffsetX) * nTileWidth;
            float fTileOffsetY = (fOffsetY - (int)fOffsetY) * nTileHeight;

            // Draw visible tile map
            for (int x = -3; x < nVisibleTilesX + 3; x++)
            {
                for (int y = -3; y < nVisibleTilesY + 3; y++)
                {
                    int sTileID = GetTile(x + fOffsetX, y + fOffsetY);
                    int sTileID_Below = GetTile(x + fOffsetX, y + fOffsetY + 1);
                    int sTileID_Above = GetTile(x + fOffsetX, y + fOffsetY - 1);
                    int sTileID_ToL = GetTile(x + fOffsetX - 1, y + fOffsetY);
                    int sTileID_ToR = GetTile(x + fOffsetX + 1, y + fOffsetY);


                    int sTileID_BG = GetTileBG(x + fOffsetX, y + fOffsetY);

                    if (sTileID_BG != 0)
                    {
                        spriteBatch.Draw(Tiles_Page1, new Rectangle((int)(x * nTileDrawWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), nTileDrawWidth, nTileHeight), BuildingBlocks.GetRectangleForTile(sTileID_BG, DrawPurpose.Tile, sTileID_Below, sTileID_Above, sTileID_ToL, sTileID_ToR, AnimationStatus), Color.White);
                    }
                    if (sTileID != 0)
                    {
                        spriteBatch.Draw(Tiles_Page1, new Rectangle((int)(x * nTileDrawWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), nTileDrawWidth, nTileHeight), BuildingBlocks.GetRectangleForTile(sTileID, DrawPurpose.Tile, sTileID_Below, sTileID_Above, sTileID_ToL, sTileID_ToR, AnimationStatus), Color.White, 0f, Vector2.Zero, GetTileFlipping(x + fOffsetX, y + fOffsetY), 0f);
                    }
                }
            }

            // Draw Player

            Vector2 GamertagSize = (bIsSuperSupporter ? sf_namess : sf).MeasureString(sGamerTag);

            spriteBatch.DrawString((bIsSuperSupporter ? sf_namess : sf), sGamerTag, new Vector2((int)(((fPlayerPosX - fOffsetX) * nTileDrawWidth) - (GamertagSize.X) / 2) + 15, (int)(((fPlayerPosY - fOffsetY) * nTileHeight) - (GamertagSize.Y) / 2) - 30), Color.Black);
            spriteBatch.DrawString((bIsSuperSupporter ? sf_namess : sf), sGamerTag, new Vector2((int)(((fPlayerPosX - fOffsetX) * nTileDrawWidth) - (GamertagSize.X) / 2) + 14, (int)(((fPlayerPosY - fOffsetY) * nTileHeight) - (GamertagSize.Y) / 2) - 31), (sGamerTag.StartsWith("@") ? (sGamerTag == "@iProgramInCpp" ? Color.Goldenrod : Color.Purple) : Color.White));
            DrawPlayerSprite((int)((fPlayerPosX - fOffsetX) * nTileDrawWidth), (int)((fPlayerPosY - fOffsetY) * nTileHeight), nTileDrawWidth, nTileHeight, spriteMan, nDirModX * nTileWidth, nDirModY * nTileHeight, nTileWidth, nTileHeight);

            Fill(new Rectangle((int)((int)Math.Floor(TilePressed.X - fOffsetX) * nTileDrawWidth - fTileOffsetX), (int)(Math.Floor(TilePressed.Y - fOffsetY) * nTileHeight - fTileOffsetY), nTileWidth, nTileHeight), Color.FromNonPremultiplied(0, 0, 0, 100));

            // Grid
            if (gameState == GameState.EditMode && IsGridEnabled)
            {
                for (int x = -3; x < nVisibleTilesX + 3; x++)
                {
                    for (int y = -3; y < nVisibleTilesY + 3; y++)
                    {
                        Fill(new Rectangle((int)(x * nTileDrawWidth - fTileOffsetX + 31), (int)(y * nTileHeight - fTileOffsetY), 1, 32), Color.Black);
                        Fill(new Rectangle((int)(x * nTileDrawWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY + 31), nTileDrawWidth, 1), Color.Black);
                    }
                }
            }

            //Debug info
            spriteBatch.DrawString(sf, "X Speed: " + fPlayerPosX.ToString(), new Vector2(10, 50), Color.Black);
            spriteBatch.DrawString(sf, "Y Speed: " + fPlayerPosY.ToString(), new Vector2(10, 70), Color.Black);

            spriteBatch.DrawString(sf, "Selected Item: " + SelectedItem.ToString(), new Vector2(10, 100), Color.Black);
           
            spriteBatch.DrawString(sf, "Start Item: " + StartingDrawItem.ToString(), new Vector2(10, 120), Color.Black);
            //spriteBatch.DrawString(sf, "mex timespan: " + bDead.ToString(), new Vector2(10, 140), Color.Black);

            //spriteBatch.DrawString(sf, "POSX " + fPlayerPosX.ToString(), new Vector2(10, 10), Color.White);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp,
                null, null, null, Matrix.CreateScale(1f));


            if (gameState == GameState.EditMode && IsUIEnabled)
            {
                spriteBatch.Draw(MakerHeader, new Rectangle(0, 0, 800, 50), Color.White);
                spriteBatch.Draw(checkmarkTexture, GridCheckBox, new Rectangle((IsGridEnabled) ? 16 : 0, 0, 16, 16), Color.White);
                spriteBatch.DrawString(sf, "Enable Grid Lines", new Vector2(40, 22), Color.Black);

                int CurrentDrawnItem = StartingDrawItem;
                for (int x = 0; x < 17; x++)
                {
                    FilterItemID(ref CurrentDrawnItem);

                    spriteBatch.Draw(Tiles_Page1, new Rectangle(150 + x * 36, 10, nTileWidth, nTileHeight), BuildingBlocks.GetRectangleForTile(CurrentDrawnItem, DrawPurpose.Show, 0, 0, 0, 0, AnimationStatus), Color.White);
                    CurrentDrawnItem += 2;

                    FilterItemID(ref CurrentDrawnItem);
                }

                spriteBatch.Draw(HideShowUIBtn, (IsUIEnabled ? HideUICheckBox : ShowUICheckBox), new Rectangle(0, (IsUIEnabled ? 0 : 16), 50, 16), Color.White);
                spriteBatch.Draw(NextTexture, NextBox, Color.White);
            }
            else if (gameState == GameState.EditMode && !IsUIEnabled)
            {
                spriteBatch.Draw(HideShowUIBtn, (IsUIEnabled ? HideUICheckBox : ShowUICheckBox), new Rectangle(0, (IsUIEnabled ? 0 : 16), 50, 16), Color.White);
            }
            else
            {

                spriteBatch.Draw(Tiles_Page1, new Rectangle(30, 30, nTileWidth, nTileHeight), BuildingBlocks.GetRectangleForTile(112, DrawPurpose.Show, 0, 0, 0, 0, AnimationStatus), Color.White);
                spriteBatch.DrawString(sf_s2, " x " + GemsCount.ToString(), new Vector2(66, 31), Color.Black);
                spriteBatch.DrawString(sf_s2, " x " + GemsCount.ToString(), new Vector2(65, 30), Color.White);

                spriteBatch.DrawString(sf_s2, "Play Mode", new Vector2(30 + 1, 400 + 1), Color.Black);
                spriteBatch.DrawString(sf_s2, "Play Mode", new Vector2(30, 400), Color.White);
                //spriteBatch.DrawString(sf_s2, "Super Growtopia Maker", new Vector2(30 + 1, 400 + 1), Color.Black);
                //spriteBatch.DrawString(sf_s2, "Super Growtopia Maker", new Vector2(30, 400), Color.Yellow);
            }

            //spriteBatch.DrawString(sf, "Player Position X: " + fCameraPosX.ToString(), new Vector2(10, 10), Color.Black);
            //spriteBatch.DrawString(sf, "Player Position Y: " + fCameraPosY.ToString(), new Vector2(10, 30), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        SpriteEffects GetTileFlipping(int x, int y)
        {
            if (x >= 0 && x < nLevelWidth && y >= 0 && y < nLevelHeight)
            {
                return spriteEffectBlocks[y * nLevelWidth + x] ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            else
            {
                return SpriteEffects.None;
            }
        }

        SpriteEffects GetTileFlipping(float fX, float fY)
        {
            return GetTileFlipping((int)Math.Floor(fX), (int)Math.Floor(fY));
        }
    }
}
