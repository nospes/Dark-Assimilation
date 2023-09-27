namespace MyGame;

public static class InputManager
{
    private static Vector2 _direction;
    public static Vector2 Direction => _direction;
    public static bool _attacking = false;
    public static bool Moving => _direction != Vector2.Zero;

    public static void Update()
    {
        //Enquanto nenhuma tecla estiver ativa, vetor de movimento é (0,0)
        _direction = Vector2.Zero;

        //Verifica se o teclado tem alguma tecla pressonada
        var keyboardState = Keyboard.GetState();

        //Caso o numero de teclas pressionadas seja maior que 0 e seja alguma da lista ele toma ações de acordo com cada caso
        if (keyboardState.GetPressedKeyCount() > 0)
        {
            if(keyboardState.IsKeyDown(Keys.K)) _attacking = true;
            if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
            if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;
            if (keyboardState.IsKeyDown(Keys.W)) _direction.Y--;
            if (keyboardState.IsKeyDown(Keys.S)) _direction.Y++;            
        }
    }
}