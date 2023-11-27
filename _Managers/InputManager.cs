namespace MyGame;

public static class InputManager
{
    private static Vector2 _lastdir = Vector2.One;
    private static Vector2 _direction;
    public static Vector2 Lastdir => _lastdir;
    public static Vector2 Direction => _direction;

    public static bool Moving => _direction != Vector2.Zero;

    public static void Update()
    {

        //Enquanto nenhuma tecla estiver ativa, vetor de movimento é (0,0)
        _direction = Vector2.Zero;


        //Verifica se o teclado tem alguma tecla pressonada
        var keyboardState = Keyboard.GetState();

        //Caso o numero de teclas pressionadas seja maior que 0 e seja alguma da lista ele toma ações de acordo com cada caso
        if (keyboardState.GetPressedKeyCount() > 0 && !Hero.CAST && !Hero.ATTACKING && !Hero.RECOIL && !Hero.DEATH)
        {


            if (!Hero.DASH)
            {
                if (keyboardState.IsKeyDown(Keys.K)) Hero.ATTACKING = true;
                else
                {
                    if (keyboardState.IsKeyDown(Keys.L) && !Hero.skillCD.CheckCooldown) Hero.CAST = true;
                    if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
                    if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;
                    if (keyboardState.IsKeyDown(Keys.W)) _direction.Y--;
                    if (keyboardState.IsKeyDown(Keys.S)) _direction.Y++;
                }
                if (_direction != Vector2.Zero) _lastdir = _direction;

            }
            if (keyboardState.IsKeyDown(Keys.J))
            {

                if (!Hero.dashCD.CheckCooldown)
                {
                    Hero.DASH = true;
                    _direction += _lastdir;
                }
            }
        }
    }



}
