namespace MyGame;

public abstract class playerBase
{

    public bool _mirror {get; set;}
    public Vector2 _origin{get; set;}
    
    public Vector2 _position { get; set; }
    public float _speed { get; set;}
}