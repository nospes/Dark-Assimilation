using Microsoft.Xna.Framework.Media;

namespace MyGame;
public class MusicManager
{
    private Song menuMusic;
    private Song gameMusic;
    private Song gameOverMusic;
    private Song bossFightMusic;
    private String currentPlaying;

    public MusicManager()
    {
        // Carregar as músicas
        menuMusic = Globals.Content.Load<Song>("BGM/Title");
        gameMusic = Globals.Content.Load<Song>("BGM/Castle");
        gameOverMusic = Globals.Content.Load<Song>("BGM/GameOver");
        bossFightMusic = Globals.Content.Load<Song>("BGM/BossFight");
    }

    public void PlayMusic(string state,bool isRepeating = true)
    {
        if (currentPlaying != state)
        {
            currentPlaying = state;
            MediaPlayer.IsRepeating = isRepeating;
            switch (state)
            {
                case "Menu":
                    MediaPlayer.Play(menuMusic);
                    break;
                case "InGame":
                    MediaPlayer.Play(gameMusic);
                    break;
                case "GameOver":
                    MediaPlayer.Play(gameOverMusic);
                    break;
                case "BossFight":
                    MediaPlayer.Play(bossFightMusic);
                    break;
            }
        }
    }

    public void StopMusic()
    {
        MediaPlayer.Stop();
        currentPlaying = "";
    }
}