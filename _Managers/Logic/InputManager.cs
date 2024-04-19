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
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed && GameManager.GAMEOVER)
        {
            GameManager.GAMEOVER = false;
            Game1.GAMESTART = false;
            Task.Run(async () => await PythonBridge.AggregatePlayerDataAsync()).Wait();
            PythonBridge.ClearJsonData("All");
            ProfileManager.ClearCounts();
            
        }

        //Caso o numero de teclas pressionadas seja maior que 0 e seja alguma da lista ele toma ações de acordo com cada caso
        if (!Hero.CAST && !Hero.ATTACKING && !Hero.KNOCKBACK && !Hero.DEATH && Game1.GAMESTART)
        {


            if (!Hero.DASH)
            {
                if (mouseState.LeftButton == ButtonState.Pressed) Hero.ATTACKING = true;
                else
                {

                    if (mouseState.RightButton == ButtonState.Pressed && !Hero.skillCD.CheckCooldown)
                    {
                        Hero.CAST = true;
                        Hero.castPos = new Vector2(mouseState.X, mouseState.Y);
                    }
                    else
                    {
                        if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
                        if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;
                        if (keyboardState.IsKeyDown(Keys.W)) _direction.Y--;
                        if (keyboardState.IsKeyDown(Keys.S)) _direction.Y++;
                    }
                }
                if (_direction != Vector2.Zero) _lastdir = _direction;

            }
            if (keyboardState.IsKeyDown(Keys.Space) || mouseState.MiddleButton == ButtonState.Pressed)
            {

                if (!Hero.dashCD.CheckCooldown && !Hero.DASH)
                {
                    Hero.DASH = true;
                    Hero.DASHPOSLOCK = true;
                }
            }
        }

        if (keyboardState.IsKeyDown(Keys.D0)) PythonBridge.ExecutePythonScript();
        //if (keyboardState.IsKeyDown(Keys.D9)) PythonBridge.ClearJsonData("All");

        if (keyboardState.IsKeyDown(Keys.P) && !GameManager.GAMEOVER && Game1.GAMESTART) GameManager.PauseGame();
    }



}
