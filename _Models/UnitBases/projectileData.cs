namespace MyGame;

public sealed class ProjectileData
{
    //Atributos do projetil
    public Microsoft.Xna.Framework.Vector2 Position { get; set; } //Posição
    public float Lifespan { get; set; } //Duração
    public int Speed { get; set; } //Velocidade
    public Microsoft.Xna.Framework.Vector2 Direction { get; set; } //Direção
    public float Scale { get; set; } //Escalonamento da sprite
    public bool Homing { get; set; } //Se é ou não um projétil perseguidor
    public string ProjectileType { get; set; } //O tipo de projétil

}