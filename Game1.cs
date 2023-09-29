namespace MyGame;

public class Game1 : Game
{

    private GraphicsDeviceManager _graphics;
    public static SpriteBatch _spriteBatch;
    private GameManager _gameManager;




    public Game1()
    {


        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

    }

    protected override void Initialize()
    {

        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 768;
        _graphics.ApplyChanges();

        Globals.Content = Content;

        _gameManager = new();
        _gameManager.Init();
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;


        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        // TODO: Add your update logic here

        Globals.Update(gameTime);
        _gameManager.Update();

        base.Update(gameTime);
    }



    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();
        _gameManager.Draw();
/*
        //Hitbox test
        Rectangle rect = new Rectangle((int)Hero._posHitbounds.X, (int)Hero._posHitbounds.Y, (int)Hero._hitBounds.X, (int)Hero._hitBounds.Y); // X, Y, largura, altura
        Rectangle Erect = new Rectangle((int)enemySkeleton._posHitbounds.X, (int)enemySkeleton._posHitbounds.Y, (int)enemySkeleton._hitBounds.X, (int)enemySkeleton._hitBounds.Y); // X, Y, largura, altura
        Rectangle Arect = new Rectangle((int)Hero._posHitbounds.X+28, (int)Hero._posHitbounds.Y+15, 60, 30); 
        Rectangle ArectM = new Rectangle((int)Hero._posHitbounds.X-52, (int)Hero._posHitbounds.Y+15, 60, 30); 
        Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.Red });
        _spriteBatch.Draw(pixel, rect, Color.Red);
        _spriteBatch.Draw(pixel, Erect, Color.Red);
        if(InputManager._attacking)
        _spriteBatch.Draw(pixel, Arect, Color.Red);
        if(InputManager._attacking)
        _spriteBatch.Draw(pixel, ArectM, Color.Red);
        //Hitbox test end
*/
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
