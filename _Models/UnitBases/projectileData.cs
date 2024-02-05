namespace MyGame;

public sealed class ProjectileData
{
    //Atributos do projetil
    public Microsoft.Xna.Framework.Vector2 Position { get; set; } //Posição
    public float Lifespan { get; set; } //Duração
    public float Scale { get; set; } //Escalonamento da sprite
    public int Speed { get; set; } //Velocidade
    public Microsoft.Xna.Framework.Vector2 Direction { get; set; } //Direção
    public bool Homing { get; set; } //Se é um projétil perseguidor
    public bool Friendly {get; set;} //Se é do jogador
    public string ProjectileType { get; set; } //O tipo de projétil

}