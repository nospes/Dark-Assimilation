namespace MyGame;

public class Game1 : Game
{

    private GraphicsDeviceManager _graphics;
    public static SpriteBatch _spriteBatch;
    private GameManager _gameManager;
    public static Texture2D pixel;




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

        _gameManager = new();
        _gameManager.Init();

        // TODO: Add your initialization logic here

        base.Initialize();
        PythonBridge.ClearJsonData();
        PythonBridge.ExecutePythonScript();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;


        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || Globals.Exitgame)
            Exit();


        // TODO: Add your update logic here

        Globals.Update(gameTime);
        _gameManager.Update();

        base.Update(gameTime);
    }



    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.Red });
        // TODO: Add your drawing code here

        _gameManager.Draw();

        base.Draw(gameTime);
    }
}
