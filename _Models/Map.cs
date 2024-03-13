namespace MyGame;

public class Map
{
    private readonly Point _mapTileSize = new(15, 15);
    private readonly staticSprite[,] _tiles;
    public Point TileSize { get; private set; }
    public Point MapSize { get; private set; }

    public Map()
    {
        _tiles = new staticSprite[_mapTileSize.X, _mapTileSize.Y];

        List<Texture2D> textures = new(1); // Cria uma lista de texturas

        // Adiciona as seguintes texturas na lista
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{1}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{2}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{3}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{4}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{5}"));

        TileSize = new(textures[0].Width, textures[0].Height); // Define tamanho de cada Tile
        MapSize = new(TileSize.X * _mapTileSize.X, TileSize.Y * _mapTileSize.Y); // Define tamanho do mapa de acordo com tamanho do tile

        Random random = new(); // Randomizes the possible textures

        // Use a specific sprite for the first row
        Texture2D specificTexture = Globals.Content.Load<Texture2D>($"Map/Wall");
        
        for (int y = 0; y < _mapTileSize.Y; y++)
        {
            for (int x = 0; x < _mapTileSize.X; x++)
            {
                if (y == 0)
                {
                    // Textura das paredes
                    _tiles[x, y] = new staticSprite(specificTexture, new Vector2(x * TileSize.X, y * TileSize.Y));
                }
                else
                {
                    // Textura do chão
                    int r = random.Next(0, textures.Count); 
                    _tiles[x, y] = new staticSprite(textures[3], new Vector2(x * TileSize.X, y * TileSize.Y));
                }
            }
        }
    }

    public void Draw()
    {
        for (int y = 0; y < _mapTileSize.Y; y++)
        {
            for (int x = 0; x < _mapTileSize.X; x++)
            {
                _tiles[x, y].Draw(); // Desenha os tiles nos limites do mapa
            }
        }
    }
}
