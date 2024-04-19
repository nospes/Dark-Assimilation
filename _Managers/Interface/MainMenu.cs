using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;

namespace MyGame
{
    public class MainMenu
    {
        private Desktop _desktop;
        private Texture2D _logoTexture;

        public MainMenu()
        {
            _desktop = new Desktop();
            BuildUI();
        }

        private void BuildUI() // Cria o UI da tela inicial
        {
            // Logo
            _logoTexture = Globals.Content.Load<Texture2D>("UI/Logo");

            // Define o painel que vai conter os elementos gráficos
            var panel = new Panel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 1000,
                Height = 1000
            };

            // Define o logo
            var logoImage = new Image
            {
                Left = -250,
                Background = new TextureRegion(_logoTexture),
                Width = 600,
                Height = 600,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center // Position at the top of the panel
            };
            panel.Widgets.Add(logoImage); // Adiciona a tela de renderização

            // Define botão de iniciar o jogo
            var button1 = new Button
            {
                Left = 300, // Position below the logo
                Top = -75,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 200,
                Height = 75,
                Content = new Label
                {
                    Text = "Start Game",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                }
            };
            button1.Click += (s, a) => { Game1.GAMESTART = true; GameManager.PauseGame();};
            panel.Widgets.Add(button1); // Adiciona a tela de renderização

            // Define botao de sair do jogo
            var button2 = new Button
            {
                Left = button1.Left,
                Top = button1.Top + 150, // Ensure it's positioned correctly below the first button
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 200,
                Height = 75,
                Content = new Label
                {
                    Text = "Exit Game",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                }
            };
            button2.Click += (s, a) => { Game1.GAMEEXIT = true; };
            panel.Widgets.Add(button2); // Adiciona a tela de renderização

            _desktop.Root = panel; // Define o objeto tipo 'panel' para ser as raizes de todos os elementos gráficos criados
        }

        public void Render() // Renderiza a tela criada
        {
            _desktop.Render();
        }
    }
}