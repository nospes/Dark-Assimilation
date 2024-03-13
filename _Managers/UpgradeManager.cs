namespace MyGame;

public class Upgrademanager
{
    private static Texture2D menuWindow, upgradeButton, upgradeButton_hover;
    private SpriteFont font;
    private Vector2 position;
    private Rectangle buttonBounds;
    private bool isHovered, isPressed;
    private string title, description;
    private int upgradeID;

    public Upgrademanager()
    {
        font = Globals.Content.Load<SpriteFont>("UI/baseFont");
        menuWindow = Globals.Content.Load<Texture2D>("UI/border_UI");
        upgradeButton = Globals.Content.Load<Texture2D>("UI/frameNormal_UI");
        upgradeButton_hover = Globals.Content.Load<Texture2D>("UI/frameActive_UI");
        

        //font = Globals.Content.Load<SpriteFont>("");
        this.title = "Test";
        this.description = "Flavortext";
        this.buttonBounds = new Rectangle((int)position.X, (int)position.Y, menuWindow.Width, menuWindow.Height);
    }

    public void Update()
    {
        var mouseState = Mouse.GetState();
        isHovered = buttonBounds.Contains(mouseState.X, mouseState.Y);
        isPressed = isHovered && mouseState.LeftButton == ButtonState.Pressed;
    }

    public void Draw(Vector2 pos)
    {
        position = pos;
        Vector2 windowPosition = new Vector2(menuWindow.Width / 2, menuWindow.Height / 2);
        Color color = isHovered ? Color.Gray : Color.White;
        //Globals.SpriteBatch.Draw(menuWindow, position - windowPosition, new Rectangle(0,0,menuWindow.Width, menuWindow.Height), Color.White,0,Vector2.Zero,1,SpriteEffects.None,0);
        //Globals.SpriteBatch.DrawString(font, title, new Vector2(position.X, position.Y - 20), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
        //Globals.SpriteBatch.DrawString(font, description, new Vector2(position.X - windowPosition.X,position.Y - windowPosition.Y +20), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }

}