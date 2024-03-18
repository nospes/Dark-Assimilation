namespace MyGame;

public class Hero
{

    //Posição e Velocidade
    public Vector2 POSITION { get; set; } //Posição do jogador
    public float SPEED { get; set; } //Velocidade do jogador

    // Animações
    private readonly AnimationManager _anims = new(); //Gerenciador de animações
    private static Texture2D _textureIdle, _textureMove, _textureAttack, _textureCast, _textureDash, _textureDeath, _textureRecoil; //Texturas
    private readonly int _scale = 3; //Escalonamento do sprite
    private bool _mirror { get; set; } //Espelhamento do sprite
    private Vector2 _origin { get; set; } //Origem do centro do sprite


    //Atributos do hitbox
    private Vector2 _baseHitBoxsize, _minPos, _maxPos, _dashdir; //Tamanho da hitbox base e posições maximas e minimas do mapa
    public static Vector2 SCALEDHITBOXSIZE; //Hitbox escalonada
    public Vector2 CENTER; //Centro do sprite escalonado e atualizado com a posição atual

    //Estados
    public static bool ATTACKING = false, ATTACKHITTIME = false, CAST = false, CASTED = false, CASTLOCK = false, DASH = false, DASHPOSLOCK = false, RECOIL = false, KNOCKBACK = false, DEATH = false, SLOWED = false; //Variaveis de estados do jogador

    //Atributos de combate
    public int HP, heroAAdmg, heroSpelldmg, baseSpeed, spellTier, hpRegen; //Vida
    public float atkSpeed, castSpeed, dashTotalCD, critChance, critMult, skillTotalCD;
    public static Vector2 lastHitpos; //Guarda posição do ultimo inimigo que acertou o heroi, utilizado no calculo de Knockback

    //Gerenciadores de tempo de recarga
    public static SkillManager dashCD, skillCD, attackCD; //Gerenciadores
    private bool _dashCDlock = true, _skillCDlock = true; //Variaveis de controle para os gerenciadores

    public Matrix _heromatrix;

    private SpriteFont font;

    //Definindo bases do Hero/Jogador
    public Hero(Vector2 pos)
    {

        font = Globals.Content.Load<SpriteFont>("UI/baseFont");// FONTE PARA UI TEMPORARIA

        //Definição de Atributos do jogador
        POSITION = pos;
        HP = 100;
        hpRegen = 10;
        baseSpeed = 250;
        heroAAdmg = 10;
        heroSpelldmg = 10;
        dashTotalCD = 0.95f;
        atkSpeed = 0.075f;
        castSpeed = 0.06f;
        spellTier = 1;
        skillTotalCD = 5f;
        critChance = 5f;
        critMult = 1.4f;

        //Definindo texturas
        _textureIdle ??= Globals.Content.Load<Texture2D>("Player/hero.Idle");
        _textureMove ??= Globals.Content.Load<Texture2D>("Player/hero.Run");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Player/hero.Attack");
        _textureCast ??= Globals.Content.Load<Texture2D>("Player/hero.Cast");
        _textureDash ??= Globals.Content.Load<Texture2D>("Player/hero.Dash");
        _textureDeath ??= Globals.Content.Load<Texture2D>("Player/hero.Death");
        _textureRecoil ??= Globals.Content.Load<Texture2D>("Player/hero.Hurt");


        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation(0, new(_textureIdle, 4, 1, 0.2f, this));
        _anims.AddAnimation(1, new(_textureMove, 6, 1, 0.1f, this));
        _anims.AddAnimation(2, new(_textureAttack, 20, 1, atkSpeed, this));
        _anims.AddAnimation(3, new(_textureCast, 9, 1, castSpeed, this));
        _anims.AddAnimation(4, new(_textureDash, 5, 1, 0.1f, this));
        _anims.AddAnimation(5, new(_textureDeath, 5, 1, 0.2f, this));
        _anims.AddAnimation(6, new(_textureRecoil, 4, 1, 0.04f, this));


        // Tamanho base da hitbox
        _baseHitBoxsize.X = 10;
        _baseHitBoxsize.Y = 25;


        //Tamanho da Hitbox escalada
        SCALEDHITBOXSIZE.X = _baseHitBoxsize.X * _scale;
        SCALEDHITBOXSIZE.Y = _baseHitBoxsize.Y * _scale;

        //Centro do Frame
        var frameWidth = _textureIdle.Width / 4;
        var frameHeight = _textureIdle.Height / 1;
        _origin = new(frameWidth / 2, frameHeight / 2);


        //Cria objetos para gerenciar o cooldowns
        dashCD = new SkillManager();
        skillCD = new SkillManager();
        attackCD = new SkillManager();



    }

    //Pega os limites do jogador
    public Rectangle GetBounds()
    {
        int _centeroffsetX;//variavel para auxilhar no ajuste de posição
        //A seguir ele ajusta a hitbox de acordo com a ação e espelhamento
        //Hitbox se movimentando
        if (InputManager.Moving && !_mirror) _centeroffsetX = 20;
        //Hitbox espelhada
        else if (InputManager.Moving && _mirror) _centeroffsetX = -20;
        //Caso nenhum dos dois, não há ajuste
        else _centeroffsetX = 0;

        int _centeroffsetY;
        //Hitbox quando está atacando
        if (Hero.ATTACKING) _centeroffsetY = +10;
        else _centeroffsetY = 0;

        //Define os cantos superior e esquerdo da sprite.
        int left = (int)CENTER.X + _centeroffsetX - (int)SCALEDHITBOXSIZE.X / 2;
        int top = (int)CENTER.Y + _centeroffsetY - (int)SCALEDHITBOXSIZE.Y / 2;

        //Após todo o ajuste e casos aplicados, retorna um retangulo ajustado
        return new Rectangle(left, top, (int)SCALEDHITBOXSIZE.X, (int)SCALEDHITBOXSIZE.Y);
    }

    // Define até onde o jogador pode se movimentar
    public void MapBounds(Point mapSize, Point tileSize)
    {
        _minPos = new((-tileSize.X / 2) - SCALEDHITBOXSIZE.X, (-tileSize.Y / 2) + 78); //Limite esquerda e cima (limites minimos)
        _maxPos = new(mapSize.X - (tileSize.X / 2) - CENTER.X - 120, mapSize.Y - (tileSize.X / 2) - CENTER.Y - 110); //Limite direita e baixo (limites minimos)
    }

    //Define os limites dos golpes do jogador
    public Rectangle AttackBounds()
    {

        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            mousePosition = Vector2.Transform(mousePosition, Matrix.Invert(_heromatrix));
            Vector2 direction = mousePosition - CENTER;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }
            if (direction.X > 0 && !ATTACKHITTIME) _mirror = false;
            else if (!ATTACKHITTIME) _mirror = true;
        }
        var _hitsize = new Vector2(24 * _scale, 30 * _scale); //Define o tamanho

        if (!_mirror) //Caso não esteja espelhado ele retorna o seguinte retangulo
            return new Rectangle((int)CENTER.X, (int)(CENTER.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
        else //Caso esteja
            return new Rectangle((int)(CENTER.X - _hitsize.X), (int)(CENTER.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
    }

    public static Vector2 castPos;
    public Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = MathHelper.ToRadians(degrees);
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);

        return new Vector2(
            vector.X * cos - vector.Y * sin,
            vector.X * sin + vector.Y * cos);
    }
    public void SpellCast()
    {

        Random rnd = new Random();
        float angleOffset = 12f;
        if (!CASTLOCK)
        {
            var mousePosition = new Vector2(castPos.X, castPos.Y);
            mousePosition = Vector2.Transform(mousePosition, Matrix.Invert(_heromatrix));
            // Calcula a direção do centro do herói até a posição do mouse
            Vector2 direction = mousePosition - CENTER;
            // Normaliza o vetor caso não seja Vector.Zero
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            if (spellTier >= 1)
            {
                //Definindo atributos do projétil
                ProjectileData pd = new()
                {
                    Position = CENTER,
                    Direction = direction,
                    Lifespan = 3,
                    Homing = false,
                    ProjectileType = "IceProj",
                    Scale = 1.75f,
                    Speed = 300,
                    Friendly = true
                };
                ProjectileManager.AddProjectile(pd);// Adicionando o projétil ao gerenciado
            }
            if (spellTier >= 2)
            {
                ProjectileData pd2 = new()
                {
                    Position = CENTER,
                    Direction = RotateVector(direction, -angleOffset),
                    Lifespan = 3,
                    Homing = false,
                    ProjectileType = "IceProj",
                    Scale = 1.75f,
                    Speed = 300,
                    Friendly = true
                };
                ProjectileManager.AddProjectile(pd2);// Adicionando o projétil ao gerenciado
            }
            if (spellTier >= 3)
            {
                //Definindo atributos do projétil
                ProjectileData pd3 = new()
                {
                    Position = CENTER,
                    Direction = RotateVector(direction, angleOffset),
                    Lifespan = 3,
                    Homing = false,
                    ProjectileType = "IceProj",
                    Scale = 1.75f,
                    Speed = 300,
                    Friendly = true
                };
                ProjectileManager.AddProjectile(pd3);// Adicionando o projétil ao gerenciado
            }
            CASTLOCK = true;
        }
    }


    //Variaveis de temporizadores
    bool knockbackInitiated = false; // Flag to ensure knockback is initiated only once per recoil phase
    float _recoiltimer = 0f, _knockbackTimer = 0f, _knockbackDuration = 0.2f, _recoiltimerduration = 0.4f; //Contador e Duração de invulnerabilidade e recuo
    float _dashDuration = 0.3f, _dashTimer = 0f; // Contador e duração do DASH
    float _atkSpeedTracker, _castSpeedTracker;

    public void Update()
    {


        if (RECOIL)
        {
            //Trava para ativaro knockback apenas uma vez
            if (!knockbackInitiated)
            {
                KNOCKBACK = true;//Inicia o efeito de recuo
                knockbackInitiated = true;
            }

            //Temporizador para o fim da invulnerabildidade
            _recoiltimer += (float)Globals.TotalSeconds;
            if (_recoiltimer >= _recoiltimerduration)
            {
                RECOIL = false;
                _recoiltimer = 0f;
                knockbackInitiated = false;
            }
        }

        if (KNOCKBACK)
        {
            ATTACKING = false;
            CAST = false;
            POSITION += (Vector2.Normalize((CENTER - lastHitpos))) * 2;
            //Temporizador para fim do recuo
            _anims.Reset(2);
            _anims.Reset(3);
            _knockbackTimer += (float)Globals.TotalSeconds;
            if (_knockbackTimer >= _knockbackDuration)
            {
                KNOCKBACK = false;
                _knockbackTimer = 0f;
            }
        }


        if (CASTED)
        {
            CASTED = false;
            SpellCast();
        }

        if (!CAST) _anims.Reset(3);


        //define speed, aumentando-o caso esteja durante o dash ou reduzindo em caso de slow, dash quebra slow
        if (SLOWED && !DASH)
        {
            if (SPEED > 150) SPEED = 100;
            if (SPEED > 0) SPEED -= 1;
        }
        else if (DASH)
        {
            SPEED = baseSpeed * 2.5f;
            SLOWED = false;
        }
        else SPEED = baseSpeed;

        //Atualiza o centro do sprite utilizando posição atual + origem base + escalonamento da imagem
        CENTER = POSITION + _origin * _scale;

        //Movimenta o jogador com os comandos dado pelo Inputmanager.cs
        if (!ATTACKING && !CAST && !KNOCKBACK)
        {

            if (InputManager.Moving && !DASH) // Caso esteja se movendo ele anda nas direções do 'Direction' com base na speed e no tempo de jogo
            {
                POSITION += Vector2.Normalize(InputManager.Direction) * SPEED * Globals.TotalSeconds;
            }
            else if (DASH) //A mesma coisa que movimento, porem como um avanço rapido
            {
                if (DASHPOSLOCK)
                {
                    var mouseState = Mouse.GetState();
                    var mousePosition = new Vector2(mouseState.X, mouseState.Y);
                    mousePosition = Vector2.Transform(mousePosition, Matrix.Invert(_heromatrix));
                    // Calculate the direction from the center of the hero to the mouse position
                    _dashdir = mousePosition - CENTER;
                    if (_dashdir.X > 0) _mirror = false;
                    else _mirror = true;

                    _dashTimer = _dashDuration;

                    DASHPOSLOCK = false;
                }

                if (_dashTimer > 0)
                {
                    _dashTimer -= Globals.TotalSeconds;
                    if (_dashTimer <= 0)
                    {
                        // When the timer runs out, stop dashing
                        DASH = false;
                    }
                }

                POSITION += Vector2.Normalize(_dashdir) * SPEED * Globals.TotalSeconds;
            }


            POSITION = Vector2.Clamp(POSITION, _minPos, _maxPos); //Caso o jogador tente ultrapassar os limites do mapa ele retorna
        }

        //Se a vida chega a 0 entra em estado de morte
        if (HP <= 0) DEATH = true;
        //Atualiza para uma animação de acordo com o estado, caso nenhum esteja ativo ele volta para Idle.
        if (DEATH) _anims.Update(5);
        else if (KNOCKBACK) { _anims.Update(6); _anims.Reset(2); } // Além de Atualizar, Reseta Animação de Ataque
        else if (CAST) { _anims.Update(3); _anims.Reset(2); } // Além de Atualizar, Reseta Animação de Ataque
        else if (ATTACKING) _anims.Update(2);
        else if (DASH) { _anims.Update(4); _anims.Reset(2); } // Além de Atualizar, Reseta Animação de Ataque
        else if (InputManager.Direction != Vector2.Zero)
        {
            _anims.Update(1);
        }
        else
        {
            _anims.Update(0);
        }

        //Atualiza a velocidade de ataque e conjuro do jogador
        if (_castSpeedTracker != castSpeed)
        {
            _anims.ChangeAnimationSpeed(3, castSpeed);
            _castSpeedTracker = castSpeed;
        }
        if (_atkSpeedTracker != atkSpeed)
        {
            _anims.ChangeAnimationSpeed(2, atkSpeed);
            _atkSpeedTracker = atkSpeed;
        }




        //Espelha o sprite de acordo com a direção
        if (InputManager.Direction.X > 0 && !DASH && !ATTACKING) _mirror = false;
        else if (InputManager.Direction.X < 0 && !DASH && !ATTACKING) _mirror = true;


        //atualiza o tempo de recarga da ação com base no valor passado
        //cooldown do DASH
        dashCD.skillCooldown(dashTotalCD, () =>
            {
                //Console.WriteLine("Cooldown de 1 terminado. Você pode realizar a ação agora.");
            });
        if (!DASH)
        {
            if (!_dashCDlock)
            {
                dashCD.CheckCooldown = true;
                _dashCDlock = true;
            }
        }
        else _dashCDlock = false;

        //cooldown da skill
        skillCD.skillCooldown(skillTotalCD, () =>
            {
                CASTLOCK = false;
                //Console.WriteLine("Cooldown de 3 terminado. Você pode realizar a ação agora.");
            });
        if (!CAST)
        {
            if (!_skillCDlock)
            {
                skillCD.CheckCooldown = true;
                _skillCDlock = true;
            }
        }
        else _skillCDlock = false;



        if (!ATTACKING) ATTACKHITTIME = false;
    }

    public void Draw()
    {


        //hitbox check
        //Rectangle rect = AttackBounds();
        //if (ATTACKHITTIME) Globals.SpriteBatch.Draw(Game1.pixel, rect, Color.Red);

        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        if (!SLOWED) _anims.Draw(POSITION, _scale, _mirror);
        else _anims.Draw(POSITION, _scale, _mirror, 0, Color.Purple); // Caso jogador esteja sob efeito de SLOWED ele fica roxo


        Color brightRed = new Color(255, 0, 0);
        if (RECOIL) _anims.Draw(POSITION, _scale, _mirror, 0, brightRed);

        BasicInterface(); // Interface Temporaria
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //INTERFACE TEMPORARIA DO JOGADOR
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void BasicInterface()
    {
        FpsCounter();// CONTADOR DE FPS

        var textpos = new Vector2(CENTER.X - (Globals.WindowSize.X / 2), CENTER.Y - (Globals.WindowSize.Y / 2));
        if (textpos.X < -60) textpos.X = -60;
        if (textpos.Y < -60) textpos.Y = -60;
        if (textpos.X > 820) textpos.X = 820;
        if (textpos.Y > 1400) textpos.Y = 1400;
        //MOSTRA HP
        Globals.SpriteBatch.DrawString(font, "HP: " + HP.ToString(), textpos, Color.Red, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
        //MOSTRA O FPS
        Globals.SpriteBatch.DrawString(font, "FPS: " + fps.ToString(), new Vector2(textpos.X, textpos.Y + 150), Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        //CD'S
        //DASH
        Globals.SpriteBatch.DrawString(font, "DASH CD: " + (dashTotalCD - dashCD.cooldownTimer).ToString("F2"), new Vector2(textpos.X, textpos.Y + 50), Color.Red, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
        //SKILL
        Globals.SpriteBatch.DrawString(font, "SKILL CD: " + (skillTotalCD - skillCD.cooldownTimer).ToString("F2"), new Vector2(textpos.X, textpos.Y + 100), Color.Red, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    //METODO PARA CONTAGEM DE FPS
    int frameCount = 0;
    double elapsedTime = 0.0;
    int fps = 0;
    public void FpsCounter()
    {
        // Increase frame count
        frameCount++;

        // Increase elapsed time
        elapsedTime += Globals.TotalSeconds;

        // If one second has passed
        if (elapsedTime >= 1.0)
        {
            // Update FPS value to match the number of frames rendered in the last second
            fps = frameCount;

            // Reset for the next second
            frameCount = 0;
            elapsedTime = 0;
        }

        // Other game update logic...
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
