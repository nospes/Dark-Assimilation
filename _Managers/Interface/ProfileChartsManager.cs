using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using System.Linq;
using Myra.Graphics2D.TextureAtlases;

namespace MyGame
{
    public class ProfileChartsManager
    {
        private Desktop _desktop;
        private readonly int _screenWidth, _screenHeight;
        private static Panel chartPanel, framePanel;
        private Label titleLabel, synopsisLabel, playerprofileLabel, descriptionLabel;

        public ProfileChartsManager()
        {
            _desktop = new Desktop();
            _screenWidth = Globals.WindowSize.X;
            _screenHeight = Globals.WindowSize.Y;
            SetupUI();
        }

        private void SetupUI()
        {
            _desktop = new Desktop();

            int chartPanelWidth = 600;
            int chartPanelHeight = 300;
            int frameThickness = 10;
            Texture2D panelBackground = Globals.Content.Load<Texture2D>("UI/border_UI");

            // Inicializa o painel que envolve os gráficos
            framePanel = new Panel
            {
                Background = new TextureRegion(panelBackground),
                Width = chartPanelWidth + frameThickness * 2,
                Height = chartPanelHeight + frameThickness * 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Top = -25
            };

            // Inicializa o painel que contem grafico em barras.
            chartPanel = new Panel
            {
                Width = chartPanelWidth,
                Height = chartPanelHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Top = framePanel.Top - 15
            };

            // Titulo do menu de gameover
            titleLabel = new Label
            {
                Text = "GAME OVER",
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = 20, 
                Scale = new Vector2(2f)
            };

            // Texto para descrição do que contem nessa tela
            synopsisLabel = new Label
            {
                Text = "The session has concluded, this screen features a graph displaying summarized data from your encounters with enemies, categorizing your performance into three distinct profiles. These stats influence the attributes in subsequent encounters, adjusting based on the total points you've accumulated in each category. /n/nClick anywhere in the screen to return to Main menu.",
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = titleLabel.Top + 40 
            };

            // Tipo de perfil do jogador
            playerprofileLabel = new Label
            {
                Text = "Not enough data!", 
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = 175, 
                Scale = new Vector2(1.5f)
            };

            // Descrição do jogador do perfil do jogador 
            descriptionLabel = new Label
            {
                Text = "Seems like you were unable to generate enough data to define your playing style. You can press the '0' key to manually calculate your profile, but note that this data will not be reliable!", 
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = playerprofileLabel.Top + 60 
            };

            //Adicionando todos os elementos visuais no meu renderizador
            _desktop.Widgets.Add(titleLabel);
            _desktop.Widgets.Add(synopsisLabel);
            _desktop.Widgets.Add(framePanel);
            _desktop.Widgets.Add(chartPanel);
            _desktop.Widgets.Add(playerprofileLabel);
            _desktop.Widgets.Add(descriptionLabel);


        }

        //Metodo para desenhar as barras gráficas
        public void DrawBarChart(List<(int ProfileType, double Percentage)> profilePercentages)
        {
            chartPanel.Widgets.Clear(); // Limpa barras já existentes

            // Inicializador das barras
            int totalWidth = chartPanel.Bounds.Width;
            int spacing = 10;
            int availableWidth = totalWidth - (spacing * (profilePercentages.Count - 1));
            var barWidth = availableWidth / profilePercentages.Count;
            var totalHeight = chartPanel.Bounds.Height;

            // Para cada tipo de porcentagem de perfil cria uma barra
            for (int i = 0; i < profilePercentages.Count; i++)
            {
                var profile = profilePercentages[i];
                var barHeight = (int)(totalHeight * profile.Percentage);
                var panel = new Panel
                {
                    Left = i * (barWidth + spacing), 
                    Width = barWidth,
                    Height = barHeight,
                    Top = totalHeight - barHeight 
                };

                Color barColor;
                string profileLabel;
                switch (profile.ProfileType)
                {
                    case 1: 
                        barColor = Color.Red;
                        profileLabel = "Aggressive";
                        break;
                    case 2: 
                        barColor = Color.Green;
                        profileLabel = "Balanced";
                        break;
                    case 3: 
                        barColor = Color.Blue;
                        profileLabel = "Evasive";
                        break;
                    default:
                        barColor = Color.Gray;
                        profileLabel = "Unknown";
                        break;
                }
                panel.Background = new SolidBrush(barColor);

                // Titulo que define cada barra
                var label = new Label
                {
                    Text = profileLabel,
                    Width = barWidth,
                    Top = totalHeight, 
                    Left = panel.Left + barWidth / 3,
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                // Adiciona as barras e descrições na tela
                chartPanel.Widgets.Add(panel);
                chartPanel.Widgets.Add(label);


            }


            var orderedProfiles = profilePercentages.OrderByDescending(p => p.Percentage).ToList(); // Organiza as porcentagens em ordem decrescente
            const double threshold = 0.04; // Limiar de proximidade para definir a jogabilidade como balanceada

            //Caso esteja no limiar é do tipo balanceadeo
            if (orderedProfiles.Count > 1 && Math.Abs(orderedProfiles[0].Percentage - orderedProfiles[1].Percentage) <= threshold)
            {
                UpdateTitleAndDescription("Balanced");
            }
            else // Caso não, o maior valor é definido como perfil
            {

                var dominantProfile = orderedProfiles.First();
                string profileTypeName = dominantProfile.ProfileType switch
                {
                    1 => "Aggressive",
                    2 => "Balanced",
                    3 => "Evasive",
                    _ => "Unknown"
                };

                UpdateTitleAndDescription(profileTypeName); // Atualiza a descrição e o titulo de acordo com o perfil
            }

        }

        public void UpdateChartData() // Atualiza os conteudos nas barras
        {
            int aggressiveCount = ProfileManager.aggressiveCount;
            int balancedCount = ProfileManager.balancedCount;
            int evasiveCount = ProfileManager.evasiveCount;

            int totalCount = aggressiveCount + balancedCount + evasiveCount;
            if (totalCount > 0)
            {
                double aggressivePercentage = (double)aggressiveCount / totalCount;
                double balancedPercentage = (double)balancedCount / totalCount;
                double evasivePercentage = (double)evasiveCount / totalCount;

                var profilePercentages = new List<(int ProfileType, double Percentage)>
            {
                (1, aggressivePercentage),
                (2, balancedPercentage),
                (3, evasivePercentage)
            };


                DrawBarChart(profilePercentages); // Desenha as barras de acordo com o valores gerados
            }


        }

        public void UpdateTitleAndDescription(string profileType) // Atualiza as Descrições e os titulos de acordo com o tipo de perfil
        {
            // Texto do tipo de perfil
            playerprofileLabel.Text = $"You've been an {profileType} Player";

            // Descrição do tipo de perfil
            switch (profileType)
            {
                case "Aggressive":
                    playerprofileLabel.TextColor = Color.Red;
                    descriptionLabel.Text = "Your playstyle is marked by boldness and aggression. You engage enemies swiftly, often finishing fights in shorter times. Your damage window is narrow, indicating quick, decisive strikes soon after engagement prioritizing direct confrontation rather than long fights.";
                    break;
                case "Balanced":
                    playerprofileLabel.TextColor = Color.Green;
                    descriptionLabel.Text = "You adopt a balanced playstyle, navigating between aggression and caution. Your combat times and damage windows show a strategic mix of patience and decisiveness, engaging enemies with a calculated approach balancing between repositioning for tactical advantages and engaging directly.";
                    break;
                case "Evasive":
                    playerprofileLabel.TextColor = Color.Blue;
                    descriptionLabel.Text = "Your strategy emphasizes evasion and cunning, preferring to outmaneuver rather than outfight your enemies. With longests combat times you use this time to carefully choose when to strike. A higher number of dashes reflects your preference for mobility, using strategic positioning to your advantage.";
                    break;
                default:
                    playerprofileLabel.TextColor = Color.Gray;
                    descriptionLabel.Text = "Your play style is unique, so unique that you broke the game!";
                    break;
            }
        }

        public void Render() // Metodo que ao ser chamado renderiza todos os elementos graficos criados na tela
        {
            _desktop.Render();
        }

    }
}