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
            int frameThickness = 10; // Thickness of the frame
            Texture2D panelBackground = Globals.Content.Load<Texture2D>("UI/border_UI");

            // Initialize the chart panel
            // Initialize the frame panel
            framePanel = new Panel
            {
                Background = new TextureRegion(panelBackground),
                Width = chartPanelWidth + frameThickness * 2,
                Height = chartPanelHeight + frameThickness * 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Top = -25
            };

            // Initialize the chart panel
            chartPanel = new Panel
            {
                Width = chartPanelWidth,
                Height = chartPanelHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Top = framePanel.Top - 15
            };

            //Title of the menu
            titleLabel = new Label
            {
                Text = "GAME OVER",
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = 25, // Margin from the top of the screen,
                Scale = new Vector2(2f)
            };

            // Little description about the menu
            synopsisLabel = new Label
            {
                Text = "The session has concluded, but you can still analyze your gameplay. This screen features a graph displaying summarized data from your encounters with enemies, categorizing your performance into three distinct profiles. These stats influence the attributes in subsequent encounters, adjusting based on the total points you've accumulated in each category. /nNote: When the averages across profiles are close, the profile that affects future encounters is chosen randomly from the top-performing categories. ",
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = titleLabel.Top + 40 // Margin from the top of the screen
            };

            // Player profile Name
            playerprofileLabel = new Label
            {
                Text = "Player Profile", // Temporary text
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = 175, // Margin from the top of the screen
                Scale = new Vector2(1.5f)
            };

            // Player Profile Description 
            descriptionLabel = new Label
            {
                Text = "The play style description based in the profile", // Temporary text
                Width = _screenWidth,
                Wrap = true,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = playerprofileLabel.Top + 60 // Positioned below the title label
            };

            _desktop.Widgets.Add(titleLabel);
            _desktop.Widgets.Add(synopsisLabel);
            _desktop.Widgets.Add(framePanel);
            _desktop.Widgets.Add(chartPanel);
            _desktop.Widgets.Add(playerprofileLabel);
            _desktop.Widgets.Add(descriptionLabel);


        }

        public void DrawBarChart(List<(int ProfileType, double Percentage)> profilePercentages)
        {
            chartPanel.Widgets.Clear(); // Clear existing widgets in the chart panel

            int totalWidth = chartPanel.Bounds.Width;
            int spacing = 10; // Space between bars
            int availableWidth = totalWidth - (spacing * (profilePercentages.Count - 1));
            var barWidth = availableWidth / profilePercentages.Count;
            var totalHeight = chartPanel.Bounds.Height;

            for (int i = 0; i < profilePercentages.Count; i++)
            {
                var profile = profilePercentages[i];
                var barHeight = (int)(totalHeight * profile.Percentage);
                var panel = new Panel
                {
                    Left = i * (barWidth + spacing), // Add spacing between bars
                    Width = barWidth,
                    Height = barHeight,
                    Top = totalHeight - barHeight // Align to bottom
                };

                // Set the color of the bar based on the profile type
                Color barColor;
                string profileLabel;
                switch (profile.ProfileType)
                {
                    case 1: // Aggressive
                        barColor = Color.Red;
                        profileLabel = "Aggressive";
                        break;
                    case 2: // Balanced
                        barColor = Color.Green;
                        profileLabel = "Balanced";
                        break;
                    case 3: // Evasive
                        barColor = Color.Blue;
                        profileLabel = "Evasive";
                        break;
                    default:
                        barColor = Color.Gray;
                        profileLabel = "Unknown";
                        break;
                }
                panel.Background = new SolidBrush(barColor);

                // Add label below each bar
                var label = new Label
                {
                    Text = profileLabel,
                    Width = barWidth,
                    Top = totalHeight, // Position below the bar
                    Left = panel.Left + barWidth / 3,
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                chartPanel.Widgets.Add(panel);
                chartPanel.Widgets.Add(label);


            }

            // Sort the profile percentages in descending order
            var orderedProfiles = profilePercentages.OrderByDescending(p => p.Percentage).ToList();
            const double threshold = 0.04; // 5% percent

            // Check if there are at least two profiles and if the top two are within the threshold
            if (orderedProfiles.Count > 1 && Math.Abs(orderedProfiles[0].Percentage - orderedProfiles[1].Percentage) <= threshold)
            {
                UpdateTitleAndDescription("Balanced");
            }
            else
            {
                // Otherwise, we take the first profile as the dominant profile
                var dominantProfile = orderedProfiles.First();
                string profileTypeName = dominantProfile.ProfileType switch
                {
                    1 => "Aggressive",
                    2 => "Balanced",
                    3 => "Evasive",
                    _ => "Unknown"
                };
                // Update the title and description
                UpdateTitleAndDescription(profileTypeName);
            }

        }

        public void UpdateChartData()
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

                // Now use profilePercentages to draw the bar chart
                DrawBarChart(profilePercentages);
            }


        }

        public void UpdateTitleAndDescription(string profileType)
        {
            // Update the text of the title label based on the profile type
            playerprofileLabel.Text = $"You've been an {profileType} Player";

            // Set the title color and description text based on the profile type
            switch (profileType)
            {
                case "Aggressive":
                    playerprofileLabel.TextColor = Color.Red;
                    descriptionLabel.Text = "Your playstyle is marked by boldness and aggression. You engage enemies swiftly, often finishing fights in shorter times. Your damage window is narrow, indicating quick, decisive strikes soon after engagement. With fewer dashes, you tend to stand your ground, relying on direct confrontation rather than long fights.";
                    break;
                case "Balanced":
                    playerprofileLabel.TextColor = Color.Green;
                    descriptionLabel.Text = "You adopt a balanced playstyle, expertly navigating between aggression and caution. Your combat times and damage windows show a strategic mix of patience and decisiveness, engaging enemies with a calculated approach. With a moderate number of dashes, you balance between repositioning for tactical advantages and engaging directly.";
                    break;
                case "Evasive":
                    playerprofileLabel.TextColor = Color.Blue;
                    descriptionLabel.Text = "Your strategy emphasizes evasion and cunning, preferring to outmaneuver rather than outfight your enemies. With the longest average combat times and damage windows, you carefully choose when to strike, often after studying enemy patterns. A higher number of dashes reflects your preference for mobility, using strategic positioning to your advantage.";
                    break;
                default:
                    playerprofileLabel.TextColor = Color.Gray;
                    descriptionLabel.Text = "Your play style is unique, so unique that you broke the game!";
                    descriptionLabel.TextColor = Color.Gray; // Set the description color
                    break;
            }
        }

        public void Render()
        {
            _desktop.Render();
        }

        // Assuming you have other methods for handling game logic and updates
    }
}