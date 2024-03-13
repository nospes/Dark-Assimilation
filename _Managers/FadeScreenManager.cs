namespace MyGame;

public class FadeEffectManager
{
    public static float _opacity = 0f; // Opacidade
    private float _fadeSpeed = 2f; // Velocidade de fade
    private bool _isFading = false; // Se está ativo ou não o fade
    private bool _fadeDirection = true; // true para fade-in, false para fade-out

    // Inicia o fade out
    public void StartFadeOut()
    {
        _isFading = true;
        _fadeDirection = false;
    }

    // Inicia o fade in
    public void StartFadeIn()
    {
        _isFading = true;
        _fadeDirection = true;
    }

    // Update do efeito de Fade
    public void Update()
    {
        if (!_isFading) return;

        if (_fadeDirection)
        {
            // Fade in
            _opacity -= _fadeSpeed * (float)Globals.TotalSeconds;
            if (_opacity <= 0)
            {
                _opacity = 0;
                _isFading = false; // Para ao ficar totalmente transparente
            }
        }
        else
        {
            // Fade out
            _opacity += _fadeSpeed * (float)Globals.TotalSeconds;
            if (_opacity >= 1)
            {
                _opacity = 1;
                _isFading = false; // Para ao ficar totalmente desenhado
            }
        }
    }

    
    // Metodo para desenhar
    Vector2 windowSize;

    public void Draw(Vector2 position)
    {
        position.X = position.X - 1500 - Globals.WindowSize.X/2 ;
        position.Y = position.Y - 1500 - Globals.WindowSize.Y/2 ;
        windowSize.X = 3000 + Globals.WindowSize.X;
        windowSize.Y = 3000 + Globals.WindowSize.Y;
        Color color = new Color(Color.Black, _opacity);
        Globals.SpriteBatch.Draw(Game1.pixel, new Rectangle((int)position.X,(int)position.Y, (int)windowSize.X, (int)windowSize.Y), color);
    }
}