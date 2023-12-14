namespace MyGame;


public class Projectile
{
    private static AnimationManager _anims = new AnimationManager();
    public Vector2 Direction, Position;
    public float Lifespan { get; set; }

    private float speed;
    public bool Homing;
    public string ProjectileType;
    public float Scale, Rotation;

    private static Texture2D _texturearrow, _texturedarkproj, _texturedarkspell;


    public Projectile(ProjectileData data)
    {
        //Maioria dos atributos são definidos pelo proprio atacante quando ele define um projetil
        speed = data.Speed; // Velocidade
        Homing = data.Homing; // Se o projétil segue ou não
        ProjectileType = data.ProjectileType; //Tipo de caixa de colisão do projétil
        Scale = data.Scale; //Escalonamento de tamanho da sprite do projetil
        Lifespan = data.Lifespan; //Duração do projétil no campo
        Direction = data.Direction; //Direção do projétil
        Position = data.Position; // Posição





        if (!_anims.ContainsAnimation("arrow_spr")) // Caso não esteja no dicionário, uma entrada com seguinte sprite...
        {
            _texturearrow ??= Globals.Content.Load<Texture2D>("Creatures/Archer/projectile");
            _anims.AddAnimation("arrow_spr", new Animation(_texturearrow, 1, 1, 0.2f));
        }

        if (!_anims.ContainsAnimation("darkproj_spr"))
        {
            _texturedarkproj ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/spell_dark1");
            _anims.AddAnimation("darkproj_spr", new(_texturedarkproj, 10, 2, 0.2f, null, null, 1));
            _anims.AddAnimation("darkprojEnd_spr", new(_texturedarkproj, 10, 2, 0.2f, null, null, 2)); // Pega a segunda linha do spritesheet
        }

        if (!_anims.ContainsAnimation("darkspell_spr"))
        {
            _texturedarkspell ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/spell_dark2");
            _anims.AddAnimation("darkspell_spr", new(_texturedarkspell, 16, 1, 0.3f, null, null, 1));
        }

        //Origem central do sprite
        var frameWidth = _texturearrow.Width / 1;
        var frameHeight = _texturearrow.Height / 1;

    }

    //Caixas de colisão dos projéteis


    public void Update()
    {
        //Atualiza o centro
        if (Homing && Lifespan > 1) Direction = Vector2.Normalize(Globals.HEROLASTPOS - Position); //Se for do tipo Homing a direção sempre é atualizada para posição do heroi, para de mudar a duração quando projétil acaba
        if (ProjectileType != "DarkSpell") Rotation = (float)Math.Atan2(Direction.Y, Direction.X); //Rotação do projétil em relação ao personagem, não rotaciona caso seja um efeito de campo

        Position += Vector2.Normalize(Direction) * speed * Globals.TotalSeconds; //Faz os projeteis se moverem
        Lifespan -= Globals.TotalSeconds;   //Diminui o tempo de duração, caso chegue em 0 ele deleta automaticamente

        if (Lifespan <= 0f && ProjectileType == "DarkProj") _anims.Reset("darkprojEnd_spr"); // Reincia a animação do fim do projétil
    }

    public Rectangle GetBounds()
    {
        switch (ProjectileType)
        {
            case "Arrow":
                return new Rectangle((int)Position.X, (int)Position.Y, 5, 5);
            case "DarkSpell":
                return new Rectangle((int)Position.X-48, (int)Position.Y-48, 48 * 2, 48 * 2);
            case "DarkProj":
                return new Rectangle((int)Position.X-14, (int)Position.Y-12, 28, 28);
            default:
                throw new InvalidOperationException($"Tipo de caixa de colisão desconhecido: {ProjectileType}");
        }
    }

    public void Draw()
    {
        if (ProjectileType == "Arrow") _anims.Update("arrow_spr"); // Atualiza a animação de acordo com tipo do projétil
        else if (ProjectileType == "DarkSpell") // Caso seja DarkSpell...
        {
            _anims.Update("darkspell_spr");
            if (Lifespan <= 0.1) Hero.SLOWED = false; // Tira slow do heroi no fim da animação
        }
        else if (ProjectileType == "DarkProj") // Caso seja um DarkProj...
        {
            _anims.Update("darkproj_spr");
            if (Lifespan < 1) // Se a duração for menor que 1 segundo
            {
                _anims.Update("darkprojEnd_spr"); //Começa animação de fim
                speed = 0; //Tira velocidade do projétil
            }

        }

        Rectangle Erect = GetBounds();
        Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        _anims.Draw(Position, Scale, false, Rotation);

    }
}