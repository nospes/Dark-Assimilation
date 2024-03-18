using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Attributes;

namespace MyGame
{
    public class UpgradeManagerUI
    {
        private Desktop _desktop; // Cria ambiente do Myra para renderização da UI
        private readonly int _screenWidth, _screenHeight; // Tamanhos da tela
        static Panel centerPanel = new Panel(), rightPanel = new Panel(), leftPanel = new Panel(); // Paineis

        public UpgradeManagerUI() // Informações básicas e Inicialização
        {
            _desktop = new Desktop();
            _screenWidth = Globals.WindowSize.X;
            _screenHeight = Globals.WindowSize.Y;
            SetupUI();
        }

        private Panel CreatePanel(int width, int height) // Função para criar um painel
        {
            Texture2D panelBackground = Globals.Content.Load<Texture2D>("UI/border_UI");
            TextureRegion background = new TextureRegion(panelBackground);
            return new Panel
            {
                Background = background,
                Width = width,
                Height = height
            };
        }

        private void SetupUI() // Configuração da UI
        {
            _desktop = new Desktop();
            Texture2D panelBackground = Globals.Content.Load<Texture2D>("UI/border_UI");

            // Criar painéis passando o tamanho deles
            centerPanel = CreatePanel(350, 450);
            rightPanel = CreatePanel(350, 450);
            leftPanel = CreatePanel(350, 450);

            // Posicionando os painéis
            centerPanel.Left = _screenWidth / 2 - (centerPanel.Width ?? 0) / 2;
            centerPanel.Top = (_screenHeight / 2 - (centerPanel.Height ?? 0) / 2) - 30;

            rightPanel.Left = centerPanel.Left + (centerPanel.Width ?? 0) + 30;
            rightPanel.Top = centerPanel.Top;

            leftPanel.Left = centerPanel.Left - (leftPanel.Width ?? 0) - 30;
            leftPanel.Top = centerPanel.Top;

            // Adicionar painéis ao desktop
            _desktop.Widgets.Add(centerPanel);
            _desktop.Widgets.Add(rightPanel);
            _desktop.Widgets.Add(leftPanel);
        }
        public void Render()
        {
            _desktop.Render(); // Renderização da UI
        }

        public void UpdatePanelData() // Atualizar os dados do painel, selecionando Melhorias Aleatórias
        {
            var random = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());
            var selectedIndices = new HashSet<int>(); // Para acompanhar os índices de presets selecionados
            int index;

            // Selecionando um preset aleatório para o centerPanel
            do
            {
                index = random.NextInt(0, _presets.Count);
            } while (!selectedIndices.Add(index)); // Tentativa de adicionar índice ao conjunto, loop se já estiver presente (não deve acontecer na primeira iteração)
            ApplyPresetToPanel(_presets[index], centerPanel);

            // Selecionando um preset aleatório para o leftPanel
            do
            {
                index = random.NextInt(0, _presets.Count);
            } while (!selectedIndices.Add(index));
            ApplyPresetToPanel(_presets[index], leftPanel);

            // Selecionando um preset aleatório para o rightPanel
            do
            {
                index = random.NextInt(0, _presets.Count);
            } while (!selectedIndices.Add(index));
            ApplyPresetToPanel(_presets[index], rightPanel);

            selectedIndices.Clear(); // Limpar o índice de painéis selecionados para poder escolher neles na próxima atualização
        }

        public class PanelPreset // Elementos pré-definidos dos painéis
        {
            public Texture2D Image { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public Action ButtonAction { get; set; }
        }

        public void ApplyPresetToPanel(PanelPreset preset, Panel panel) // Função onde aplica o "esqueleto" dos Painéis
        {
            panel.Widgets.Clear(); // Limpar o painel
            // Configuração da Imagem
            var imageWidget = new Image
            {
                Left = 100,
                Top = 25,
                Width = 150,
                Height = 150,
                Renderable = new TextureRegion(preset.Image),
                BorderThickness = new Myra.Graphics2D.Thickness(5, 5, 5, 5),
                Border = new SolidBrush(Color.SaddleBrown)
            };

            // Configuração do título
            var titleLabel = new Label
            {
                Text = preset.Title,
                TextColor = Color.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Scale = new Vector2(2, 2)
            };

            // Configuração do texto descritivo
            var descriptionLabel = new Label
            {
                Text = preset.Description,
                TextColor = Color.White,
                VerticalAlignment = VerticalAlignment.Center,
                Top = 80,
                HorizontalAlignment = HorizontalAlignment.Center,
                Wrap = true,
            };
            // Configuração do botão
            var button = new Button
            {
                Width = 100,
                Height = 50,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Top = -25,
                Background = new SolidBrush(Color.SaddleBrown)
            };
            // Ao clicar no botão:
            button.Click += (sender, args) =>
            {
                preset.ButtonAction?.Invoke();
            };

            // Adicionar widgets ao painel
            panel.Widgets.Add(imageWidget);
            panel.Widgets.Add(titleLabel);
            panel.Widgets.Add(descriptionLabel);
            panel.Widgets.Add(button);
        }

        private List<PanelPreset> _presets = new List<PanelPreset>  // Os possíveis Presets/Melhorias para adicionar nos painéis
        {
            new PanelPreset //Damage Upgrade
                {
                    Image = Globals.Content.Load<Texture2D>("UI/Icons/DMG_upgrade"),
                    Title = "Damage Upgrade",
                    Description = "- Improve sword damage/n- Improve spell damage",
                    ButtonAction = () => {UpgradeHero.Upgrade(1);}
                },
            new PanelPreset //Speed Upgrade
                {
                    Image = Globals.Content.Load<Texture2D>("UI/Icons/SPEED_upgrade"),
                    Title = "Speed Upgrade",
                    Description = "- Improve movement speed/n- Reduces dash cooldown",
                    ButtonAction = () => {UpgradeHero.Upgrade(2);}
                },
            new PanelPreset //Critical Upgrade
                {
                    Image = Globals.Content.Load<Texture2D>("UI/Icons/CRIT_upgrade"),
                    Title = "Critical Upgrade",
                    Description = "- Improve critical chance/n- Improve critical damage",
                    ButtonAction = () => {UpgradeHero.Upgrade(3);}
                },
            new PanelPreset //Vitality Upgrade
                {
                    Image = Globals.Content.Load<Texture2D>("UI/Icons/VIT_upgrade"),
                    Title = "Vitality Upgrade",
                    Description = "- Improve total health/n- Improve regeneration between stages",
                    ButtonAction = () => {UpgradeHero.Upgrade(4);}
                },
            new PanelPreset //Spell Upgrade
                {
                    Image = Globals.Content.Load<Texture2D>("UI/Icons/SPELL_upgrade"),
                    Title = "Spell Upgrade",
                    Description = "- Reduces spell cooldown/n- Improve spell effects",
                    ButtonAction = () => {UpgradeHero.Upgrade(5);}
                },
            new PanelPreset //Attack Speed Upgrade
                {
                    Image = Globals.Content.Load<Texture2D>("UI/Icons/FIERCE_upgrade"),
                    Title = "Fierce Upgrade",
                    Description = "- Improve attack speed/n- Reduces total cast animation",
                    ButtonAction = () => {UpgradeHero.Upgrade(6);}
                },
        };

        public class UpgradeHero // Função onde os Botões no Painel aplicam as Mudanças no herói E fecham a UI de Melhorias
        {
            private static Hero _hero;
            public static void Init(Hero hero) // Pegar a instância do jogador ativo
            {
                _hero = hero;
            }

            public static void Upgrade(int _upgradeType)
            {
                switch (_upgradeType)
                {
                    case 1://Attack Upgrade
                        _hero.heroAAdmg += 5;
                        _hero.heroSpelldmg += 8;
                        break;
                    case 2://Speed Upgrade
                        _hero.baseSpeed += 20;
                        if (_hero.baseSpeed == 270) _hero.dashTotalCD -= 0.15f;
                        if (_hero.baseSpeed == 310) _hero.dashTotalCD -= 0.15f;
                        break;
                    case 3://Critical Upgrade
                        _hero.critChance += 5;
                        if (_hero.critChance == 15) _hero.critMult = 1.7f;
                        if (_hero.critChance == 30) _hero.critMult = 2f;
                        break;
                    case 4://Vitality Upgrade
                        _hero.HP += 20;
                        _hero.hpRegen += 10;
                        break;
                    case 5://Spell Upgrade
                        _hero.skillTotalCD -= 0.4f;
                        if (_hero.skillTotalCD == 3.6f) _hero.spellTier = 2;
                        if (_hero.skillTotalCD == 3.2f) _hero.spellTier = 3;
                        break;
                    case 6://Attack Speed Upgrade
                        _hero.atkSpeed -= 0.005f;
                        _hero.castSpeed -= 0.005f;
                        break;
                }
                Soul.UPGRADEMENU = false; // Fecha o menu de aprimoramento
                GameManager.PAUSE = false; // Despausa o jogo
            }

        }


    }
}
