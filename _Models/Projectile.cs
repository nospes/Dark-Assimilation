namespace MyGame;


public class Projectile
{
    private static AnimationManager _anims = new AnimationManager();
    public Vector2 Direction, Position;
    public float Lifespan { get; set; }

    private float speed;
    public bool Homing, Friendly, enemyHited;
    public string ProjectileType;
    public float Scale, Rotation;
    public int Damage;
    

    //Enemy Spell
    private static Texture2D _texturearrow, _texturedarkproj, _texturedarkspell;
    //Player Spell
    private static Texture2D _texture_iceSpell_cast, _texture_iceSpell_proj, _texture_iceSpell_hit;
    private static Texture2D _texture_thunderSpell_cast, _texture_thunderSpell_casted, _texture_thunderSpell_end;
    private static Texture2D _texture_fireSpell_cast, _texture_fireSpell_casted, _texture_fireSpell_end;


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
        Friendly = data.Friendly; // Se é um projetil do jogador ou não
        enemyHited = false; // Variavel para medir colisão em inimigos 
        Damage = data.Damage; // Dano do projétil





        if (!_anims.ContainsAnimation("arrow_spr")) // Caso não esteja no dicionário, uma entrada com seguinte sprite...
        {
            _texturearrow ??= Globals.Content.Load<Texture2D>("Creatures/Archer/projectile");
            _anims.AddAnimation("arrow_spr", new Animation(_texturearrow, 1, 1, 0.2f));
        }

        if (!_anims.ContainsAnimation("darkproj_spr"))
        {
            _texturedarkproj ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/spell_dark1");
            _anims.AddAnimation("darkproj_spr", new(_texturedarkproj, 10, 2, 0.2f));
            _anims.AddAnimation("darkprojEnd_spr", new(_texturedarkproj, 10, 2, 0.2f, null, null, 2)); // Pega a segunda linha do spritesheet
        }

        if (!_anims.ContainsAnimation("darkspell_spr"))
        {
            _texturedarkspell ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/spell_dark2");
            _anims.AddAnimation("darkspell_spr", new(_texturedarkspell, 16, 1, 0.3f));
        }

        if (!_anims.ContainsAnimation("iceSpell_cast"))
        {
            _texture_iceSpell_cast ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_ice_cast");
            _texture_iceSpell_proj ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_ice_proj");
            _texture_iceSpell_hit ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_ice_hit");
            _anims.AddAnimation("iceSpell_cast", new(_texture_iceSpell_cast, 3, 1, 0.1f));
            _anims.AddAnimation("iceSpell_proj", new(_texture_iceSpell_proj, 10, 1, 0.1f));
            _anims.AddAnimation("iceSpell_hit", new(_texture_iceSpell_hit, 7, 1, 0.1f));
        }

        if (!_anims.ContainsAnimation("thunderSpell_cast"))
        {
            _texture_thunderSpell_cast ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_thunder_cast");
            _texture_thunderSpell_casted ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_thunder_casted");
            _texture_thunderSpell_end ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_thunder_end");
            _anims.AddAnimation("thunderSpell_cast", new(_texture_thunderSpell_cast, 5, 1, 0.4f));
            _anims.AddAnimation("thunderSpell_casted", new(_texture_thunderSpell_casted, 4, 1, 0.4f));
            _anims.AddAnimation("thunderSpell_end", new(_texture_thunderSpell_end, 4, 1, 0.4f));
        }

        if (!_anims.ContainsAnimation("fireSpell_cast"))
        {
            _texture_fireSpell_cast ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_explosion_cast");
            _texture_fireSpell_casted ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_explosion_casted");
            _texture_fireSpell_end ??= Globals.Content.Load<Texture2D>("Player/Spells/spell_explosion_end");
            _anims.AddAnimation("fireSpell_cast", new(_texture_fireSpell_cast, 7, 1, 0.1f));
            _anims.AddAnimation("fireSpell_casted", new(_texture_fireSpell_casted, 7, 1, 0.1f));
            _anims.AddAnimation("fireSpell_end", new(_texture_fireSpell_end, 4, 1, 0.1f));
        }

    }

    //Caixas de colisão dos projéteis


    public void Update()
    {
        if (Friendly) // Projeteis do player
        {
            if (ProjectileType == "IceProj")
            {
                Rotation = (float)Math.Atan2(Direction.Y, Direction.X);
            }
        }
        else // Projeteis dos inimigos
        {
            if (Homing && Lifespan > 1) Direction = Vector2.Normalize(Globals.HEROLASTPOS - Position); //Se for do tipo Homing a direção sempre é atualizada para posição do heroi, para de mudar a duração quando projétil acaba
            if (ProjectileType != "DarkSpell") Rotation = (float)Math.Atan2(Direction.Y, Direction.X); //Rotação do projétil em relação ao personagem, não rotaciona caso seja um efeito de campo
        }
        Position += Vector2.Normalize(Direction) * speed * Globals.TotalSeconds; //Faz os projeteis se moverem
        Lifespan -= Globals.TotalSeconds;   //Diminui o tempo de duração, caso chegue em 0 ele deleta automaticamente
    }

    public Rectangle GetBounds()
    {
        switch (ProjectileType)
        {
            case "Arrow":
                return new Rectangle((int)Position.X, (int)Position.Y, 5, 5);
            case "DarkSpell":
                return new Rectangle((int)Position.X - 48, (int)Position.Y - 48, 48 * 2, 48 * 2);
            case "DarkProj":
                return new Rectangle((int)Position.X - 14, (int)Position.Y - 12, 28, 28);
            case "IceProj":
                return new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16);
            case "ThunderStrike":
                if (Lifespan < 0.6 && Lifespan > 0.3) return new Rectangle((int)Position.X - 32, (int)Position.Y, 64, 64);
                else return new Rectangle(0, 0, 0, 0); ;
            case "Explosion":
                if (Lifespan < 1 && Lifespan > 0.3) return new Rectangle((int)Position.X - (int)(20*Scale), (int)(Position.Y-12*Scale), (int)(40*Scale), (int)(32*Scale));
                else return new Rectangle(0, 0, 0, 0); ;
            default:
                throw new InvalidOperationException($"Tipo de caixa de colisão desconhecido: {ProjectileType}");
        }
    }

    public void Draw()
    {
        switch (ProjectileType)
        {
            case "Arrow":
                _anims.Update("arrow_spr"); // Atualiza a animação de acordo com tipo do projétil
                break;
            case "DarkSpell":
                _anims.Update("darkspell_spr");
                if (Lifespan <= 0.1) Hero.SLOWED = false; // Tira slow do heroi no fim da animação
                break;
            case "DarkProj":
                _anims.Update("darkproj_spr");
                if (Lifespan < 1) // Se a duração for menor que 1 segundo
                {
                    _anims.Update("darkprojEnd_spr"); //Começa animação de fim
                    speed = 0; //Tira velocidade do projétil
                }
                if (Lifespan <= 0.1) _anims.Reset("darkprojEnd_spr");
                break;
            case "IceProj":
                _anims.Update("iceSpell_proj");
                if (Lifespan < 0.5) { _anims.Update("iceSpell_hit"); speed = 0; }
                else _anims.Reset("iceSpell_hit");
                break;
            case "ThunderStrike":
                if (Lifespan > 1) _anims.Reset("thunderSpell_end");

                _anims.Update("thunderSpell_cast");

                if (Lifespan < 0.6f)
                {
                    _anims.Update("thunderSpell_casted");
                }

                if (Lifespan < 0.3f)
                {
                    _anims.Update("thunderSpell_end");
                }

                if (Lifespan <= 0.1f)
                {
                    _anims.Reset("thunderSpell_casted");
                    _anims.Reset("thunderSpell_cast");
                }
                break;
            case "Explosion":
                if (Lifespan > 1) _anims.Reset("fireSpell_end");

                _anims.Update("fireSpell_cast");

                if (Lifespan < 1f)
                {
                    _anims.Update("fireSpell_casted");
                }
                if (Lifespan < 0.3f)
                {
                    _anims.Update("fireSpell_end");
                }
                if (Lifespan <= 0.1f)
                {
                    _anims.Reset("fireSpell_cast");
                    _anims.Reset("fireSpell_casted");
                }

                break;
            default:
                throw new InvalidOperationException($"Tipo de projétil desconhecido: {ProjectileType}"); ;
        }

        //Rectangle Erect = GetBounds();
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        _anims.Draw(Position, Scale, false, Rotation);

    }
}