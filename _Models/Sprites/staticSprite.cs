namespace MyGame;

//cria objetos tipo Sprites estaticos como mapas e icones
public class staticSprite
{
    private readonly Texture2D _texture;
    public Vector2 Position { get; protected set; }
    public Vector2 Origin { get; protected set; }
    public float Rotation { get; set; }
    public float Scale { get; set; }

    public staticSprite(Texture2D texture, Vector2 position, float scale = 1)
    {
        _texture = texture;
        Position = position;
        Scale = scale;
        Origin = new(_texture.Width / 2, _texture.Height / 2);
    }

    public void Draw()
    {
        Globals.SpriteBatch.Draw(_texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 1);
    }
}