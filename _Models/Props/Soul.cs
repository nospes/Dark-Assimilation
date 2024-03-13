namespace MyGame;

public class Soul
{

    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _texture;
    private Vector2 position, origin, basehitbox;
    private float scale;
    public bool alive;
    public Soul(Vector2 pos)
    {
        _texture ??= Globals.Content.Load<Texture2D>("Map/Props/Soul_spr");
        _anims.AddAnimation("Soul_spr", new(_texture, 60, 1, 0.1f, this, null, 1));

        scale = 3;
        position = pos;
        alive = true;

        basehitbox.X = _texture.Width / 60; // Dividindo os frames de acordo com tamanho do spritesheet, usando como base a animação de Idle;
        basehitbox.Y = _texture.Height / 1;
        origin = new(basehitbox.X / 2, basehitbox.Y / 2); //Atribui o centro do frame X e Y a um vetor
    }

    public Rectangle GetBounds()
    {
        //Limites do topo e da esquerda da caixa de colisão
        return new Rectangle((int)position.X + 48, (int)position.Y + 48, (int)(basehitbox.X * scale) / 2, (int)(basehitbox.Y * scale) / 2);

    }

    public void Update()
    {
        _anims.Update("Soul_spr");
        if (!alive)
        {
            scale -= 3f * Globals.TotalSeconds;
            position.X += 1.5f;
            position.Y += 1.5f; 
        }
        if (scale < 0) scale = 0;
    }

    public void Draw()
    {

        //hitbox test
        //Rectangle Erect = GetBounds();
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);



        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(position, scale, false);


    }
}