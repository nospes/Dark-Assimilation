namespace MyGame;

public class AnimationManager
{
    //Cria um dicionario com objetos da classe presente no Animation.cs
    private readonly Dictionary<object, Animation> _anims = new();
    //Guarda teclas apertadas
    private object _lastKey;

    public void AddAnimation(object key, Animation animation)
    {
        // Cria as classes de Animation com parametros passados pelos modelos, ex: hero.cs
        _anims.Add(key, animation);
        _lastKey ??= key;
    }

    public void Update(object key)
    {
        //Enquanto está com a tecla apertada, uma animação é feita de acordo com valor atribuido na respectiva classe
        if (_anims.TryGetValue(key, out Animation value))
        {
            value.Start();
            _anims[key].Update();
            _lastKey = key;
        }
        else
        {
            _anims[_lastKey].Stop();
            _anims[_lastKey].Reset();
        }
        //Para a ultima animação caso outra entre em prioridade
    }

    public void Draw(Vector2 position, int scale, bool mirror)
    {
        //Com as animações devidamente feitas e definidas de acordo com a nescessidade, ele chama o Animation.cs para começar a desenhe-las na tela
        _anims[_lastKey].Draw(position,scale,mirror);
    }
}