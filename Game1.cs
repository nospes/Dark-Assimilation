﻿namespace MyGame;
using Myra;

public class Game1 : Game
{

    private GraphicsDeviceManager _graphics;
    public static SpriteBatch _spriteBatch;
    private GameManager _gameManager;
    public static Texture2D pixel;
    private UpgradeManagerUI _upgradeManager;
    private ProfileChartsManager _profileChartsManager;
    private MainMenu _mainmenu;
    public static bool GAMESTART = false, GAMEEXIT = false;


    public Game1()
    {


        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

    }

    protected override void Initialize()
    {
        Globals.WindowSize = new(1280, 720);
        _graphics.PreferredBackBufferWidth = Globals.WindowSize.X;
        _graphics.PreferredBackBufferHeight = Globals.WindowSize.Y;
        _graphics.ApplyChanges();


        Globals.Content = Content;

        PythonBridge.ClearJsonData("All");
        ProfileManager.ClearCounts();

        _gameManager = new();
        _gameManager.Init();
        GameManager.PauseGame();



        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;

        MyraEnvironment.Game = this;
        _upgradeManager = new UpgradeManagerUI();
        _profileChartsManager = new ProfileChartsManager();
        _mainmenu = new MainMenu();

    }


    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || Globals.Exitgame)
            Exit();

        Globals.Update(gameTime);

        _gameManager.Update();
        if (Soul.MENUUPDATE)
        {
            _upgradeManager.UpdatePanelData();
            Soul.MENUUPDATE = false;
        }

        if (GAMEEXIT) this.Exit();


        base.Update(gameTime);
    }



    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.Red });

        _gameManager.Draw();
        if (Soul.UPGRADEMENU) _upgradeManager.Render();
        if (GameManager.GAMEOVER)
        {

            _profileChartsManager.UpdateChartData();
            _profileChartsManager.Render();

            _gameManager = new();
            _gameManager.Init();



        }
        if (!GAMESTART)
        {
            _mainmenu.Render();
            _gameManager = new();
            _gameManager.Init();

        }
        base.Draw(gameTime);
    }
}
