using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace MyGame
{
    public static class PythonBridge
    {
        private static readonly string filePath = "PlayerData.json"; // Caminho para a data do jogador
        private static readonly string processedPath = "ProcessedData.json"; // Caminho para a data PROCESSADA do jogador

        // Função utilizada para atualizar os dados coletados do jogador, ele é chamado quando o inimigo é derrotado passando os valores de combate e o tipo de inimigo
        public static async Task UpdateCombatDataAsync(int enemyType, int averageCombatTime, int damageWindow, int totalDashes)
        {
            // Criando dicionario para organizar a data
            var data = new Dictionary<string, List<int>>
            {
                { "Average Combat Time", new List<int>() },
                { "Damage Window", new List<int>() },
                { "Total Dashes", new List<int>() },
                { "Enemy Type", new List<int>() }
            };

            // Se há o JSON no caminho passado...
            if (File.Exists(filePath))
            {
                try
                {
                    string json = await ReadFromFileWithRetryAsync(filePath); // Lendo o JSON ja criado com a data do jogador 
                    data = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(json) ?? data; // Convertendo ela e colocando-a no dicionario
                }
                catch (IOException ex)
                {
                    // Caso de erros
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

            }

            // Coloca a data coletada no dicionario
            data["Average Combat Time"].Add(averageCombatTime);
            data["Damage Window"].Add(damageWindow);
            data["Total Dashes"].Add(totalDashes);
            data["Enemy Type"].Add(enemyType);

            // Salva o dicionario com a data atualizada no JSON
            string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
            await WriteToFileWithRetryAsync(filePath, updatedJson);
        }

        // Função utilizada para escrever no JSON
        public static async Task WriteToFileWithRetryAsync(string filePath, string content, int maxRetries = 3, int delayMilliseconds = 100)
        {
            //Colocado metodo para evitar conflitos com o arquivo
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try // Tenta...
                {
                    // Utiliza da Classe FileStream do proprio C# para ajudar na leitura do arquivo sem conflitos
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (var writer = new StreamWriter(stream))
                    {
                        await writer.WriteAsync(content); // Escreve a Data do jogador no JSON
                    }
                    return;
                } // Caso receba um erro...
                catch (IOException) when (retry < maxRetries - 1)
                {
                    // Ele espera um pouco e tenta fazer denovo
                    await Task.Delay(delayMilliseconds);
                }
            }
            // Caso tudo de errado ele retorna o erro
            throw new IOException($"Failed to write to {filePath} after {maxRetries} attempts.");
        }

        // Função utilizada para ler o JSON segue uma logica bem similar ao de 'Escrever' porem sem a nescessidade do uso do FileStream
        public static async Task<string> ReadFromFileWithRetryAsync(string filePath, int maxRetries = 3, int delayMilliseconds = 100)
        {

            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string content = reader.ReadToEnd();
                        return content; // Caso leia, retorna o valor dele
                    }

                }
                catch (IOException) when (retry < maxRetries - 1)
                {
                    // Caso de errado espera um pouco para tentar denovo
                    await Task.Delay(delayMilliseconds);
                }
            }

            throw new IOException($"Failed to read from {filePath} after {maxRetries} attempts.");
        }

        public static async void ClearJsonData()
        {

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync("");
            }

            using (var stream = new FileStream(processedPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync("");
            }
        }


        public static void ExecutePythonScript()
        {


            string pythonScriptPath = "ProfileSelector.py"; // Caminho para script da IA
            RunPythonScript(pythonScriptPath); // Inicializa o script

            string json = File.ReadAllText("ProcessedData.json"); // Ler o JSON criado pelo Python 
            var resultData = JsonConvert.DeserializeObject<List<string>>(json); // Converter ele para uma lista

            // Checando se funcionou ou não
            if (resultData != null)
            {
                var agressivecount = 0;
                var balancedcount = 0;
                var evasivecount = 0;

                // Count each category
                foreach (var item in resultData)
                {
                    switch (item)
                    {
                        case "Aggressive":
                            agressivecount++;
                            break;
                        case "Balanced":
                            balancedcount++;
                            break;
                        case "Evasive":
                            evasivecount++;
                            break;
                    }
                }

                ProfileManager.aggressiveCount = agressivecount;
                ProfileManager.balancedCount = balancedcount;
                ProfileManager.evasiveCount = evasivecount;

                // Print the counts
                Console.WriteLine($"Aggressive: {ProfileManager.aggressiveCount}");
                Console.WriteLine($"Balanced: {ProfileManager.balancedCount}");
                Console.WriteLine($"Evasive: {ProfileManager.evasiveCount}");
            }
            else // Caso não tenha data ele avisa
            {
                Console.WriteLine("No data!");
            }
        }

        private static Dictionary<string, int> CountElements(List<string> elements)
        {
            var counts = new Dictionary<string, int>();

            foreach (var element in elements)
            {
                if (counts.ContainsKey(element))
                {
                    counts[element]++;
                }
                else
                {
                    counts[element] = 1;
                }
            }

            return counts;
        }

        private static string RunPythonScript(string scriptPath) // Metodo para inicializar o script de python (Modelo KNN)
        {
            ProcessStartInfo start = new ProcessStartInfo()
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Metodo para detecção de erros do script de Python
            using (Process process = new Process { StartInfo = start })
            {
                Console.WriteLine("Iniciando Script Python...");
                process.Start();
                // Lê toda a saída (saída padrão) do processo até o fim. Isso é tipicamente a saída que você imprime do seu script Python.
                string result = process.StandardOutput.ReadToEnd();
                // Lê toda a saída de erro (erro padrão) do processo até o fim. Isso inclui erros e exceções lançadas pelo seu script Python.
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Verifica se a string de resultado não é nula ou vazia, indicando que o script Python produziu alguma saída.
                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine("Output do Python:");
                    Console.WriteLine(result);
                }

                // Verifica se a string de erro não é nula ou vazia, indicando que o script Python encontrou erros durante sua execução.
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Erro do Python:");
                    Console.WriteLine(error);
                }
                return result;
            }
        }
    }
}