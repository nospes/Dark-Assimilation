namespace MyGame;

public class AnimationManager
{
    //Cria um dicionario com objetos da classe presente no Animation.cs
    private readonly Dictionary<object, Animation> _anims = new();
    //Guarda o ultimo indice do dicionario
    private object _lastKey;

    public void AddAnimation(object key, Animation animation)
    {
        // Essa função é chamada por uma unidade que passa os parametros do seu spritesheet
        // e o salva em um dicionario de acordo com os indices passados
        _anims.Add(key, animation);
        _lastKey ??= key;
    }

    public void Update(object key)
    {
        //Essa função pega o indice do dicionario de animação correspondente, salva e retorna o valor
        //com a animação definida ele chama Start() do Animation.cs para ativar o sprite
        //A mesma coisa vale para o Update() que começa a passar de quadro em quadro.
        if (_anims.TryGetValue(key, out Animation value))
        {
            value.Start();
            _anims[key].Update();
            _lastKey = key;
        }
    }

    public void Reset(object key)
    {
        if (_anims.TryGetValue(key, out Animation value))
        {
            value.Reset();
        }
    }


    public bool ContainsAnimation(string key)
    {
        return _anims.ContainsKey(key);
    }


    public void Draw(Vector2 position, float scale, bool mirror, float rotation = 0,  Color? color = null)
    {
        Color drawColor = color ?? Color.White; //Se não for definida uma cor é utilizada a padrão: White
        //Com os quadros de animações definidos, ele passa os parametros para Animation.cs começar a desenhar
        //Utilizando o frame atual do Update() presente no Animation.cs
        _anims[_lastKey].Draw(position, scale, mirror, rotation, drawColor);
    }
}
