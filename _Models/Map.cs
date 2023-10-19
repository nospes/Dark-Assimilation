namespace MyGame;

public class Map
{
    private readonly Point _mapTileSize = new(15, 15);
    private readonly Sprite[,] _tiles;
    public Point TileSize { get; private set; }
    public Point MapSize { get; private set; }

    public Map()
    {
        _tiles = new Sprite[_mapTileSize.X, _mapTileSize.Y];

        List<Texture2D> textures = new(1);

        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{1}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{2}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{3}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{4}"));
        textures.Add(Globals.Content.Load<Texture2D>($"Map/tile{5}"));

        TileSize = new(textures[0].Width, textures[0].Height);
        MapSize = new(TileSize.X * _mapTileSize.X, TileSize.Y * _mapTileSize.Y);

        Random random = new();
        int r = random.Next(0, textures.Count);

        for (int y = 0; y < _mapTileSize.Y; y++)
        {
            for (int x = 0; x < _mapTileSize.X; x++)
            {

                _tiles[x, y] = new(textures[r], new(x * TileSize.X, y * TileSize.Y));
            }
        }
    }

    public void Draw()
    {
        for (int y = 0; y < _mapTileSize.Y; y++)
        {
            for (int x = 0; x < _mapTileSize.X; x++) _tiles[x, y].Draw();
        }
    }
}