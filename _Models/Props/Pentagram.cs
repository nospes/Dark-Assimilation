namespace MyGame;

public class Pentagram
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _texture;
    private Vector2 position, origin, basehitbox;
    private int scale;
    public int gamearea;
    public bool teleport, teleportON;
    public static int enemyCount;

    public Pentagram(Vector2 pos)
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _texture ??= Globals.Content.Load<Texture2D>("Map/Props/pentagramm");



        //Definindo area,frames dos sprites sheets para fazer a animação
        _anims.AddAnimation("pentagram_on", new(_texture, 4, 4, 0.1f, this, null, 3));
        _anims.AddAnimation("pentagram_off", new(_texture, 4, 4, 0.1f, this, null, 2));

        position = pos;

        basehitbox.X = _texture.Width / 4; // Dividindo os frames de acordo com tamanho do spritesheet, usando como base a animação de Idle;
        basehitbox.Y = _texture.Height / 4;
        origin = new(basehitbox.X / 2, basehitbox.Y / 2); //Atribui o centro do frame X e Y a um vetor
        scale = 2;
        teleport = false;
        teleportON = false;
        enemyCount = 0;
        gamearea = 0;

    }

    public Rectangle GetBounds()
    {
        //Limites do topo e da esquerda da caixa de colisão
        return new Rectangle((int)position.X + 30, (int)position.Y + 110, (int)(basehitbox.X * scale) - 70, (int)(basehitbox.Y * scale) - 150);

    }


    public void Update()
    {
        if (gamearea == 0 && enemyCount >= 3) // Fase 1 > 2
        {
            _anims.Update("pentagram_on");
            teleportON = true;
        }
        else if (gamearea == 1 && enemyCount >= 5) // Fase 2 > 3 
        {
            _anims.Update("pentagram_on");
            teleportON = true;
        }
        else if (gamearea == 2 && enemyCount >= 7) // Fase 3 > 4
        {
            _anims.Update("pentagram_on");
            teleportON = true;
        }
        else if (gamearea == 3 && enemyCount >= 7) // Fase 4 > FIM
        {
            _anims.Update("pentagram_on");
            teleportON = true;
        }
        else // Caso esteja desligado
        {
            _anims.Update("pentagram_off");
            teleportON = false;
        }
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