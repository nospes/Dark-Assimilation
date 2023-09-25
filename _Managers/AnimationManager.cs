namespace MyGame;

public class AnimationManager
{
    //Cria um dicionario com objetos da classe presente no Animation.cs
    private readonly Dictionary<object, Animation> _anims = new();
    //Guarda teclas apertadas
    private object _lastKey;

    public void AddAnimation(object key, Animation animation)
    {
        // Cria as classes de Animation com parametros passados pelos modelos, ex: hero.cs e salva de acordo com os indices passados
        _anims.Add(key, animation);
        _lastKey ??= key;
    }

    public void Update(object key)
    {
        //Essa função pega o indice do dicionario de animação correspondente, salva esse indice e retorna o valor da animação correspondente a ele e começa a dar a ordem para o Drawn anima-la na tela usando Animation.cs, enquanto for verdade ele fica dando update nela
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
        //Para a ultima animação caso outra entre em prioridade (Talvez de para tirar ou alterar esse else)
    }

    public void Draw(Vector2 position, int scale, bool mirror)
    {
        //Com as animações devidamente feitas e definidas de acordo com a nescessidade, ele chama o Animation.cs para começar a desenhe-las na tela
        _anims[_lastKey].Draw(position,scale,mirror);
    }
}
