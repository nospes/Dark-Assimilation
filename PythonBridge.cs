using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace MyGame
{
    public static class PythonBridge
    {
        private static readonly string filePath = "PlayerData.json";

        public static async Task UpdateCombatDataAsync(string enemyType, int averageCombatTime, int damageWindow, int totalDashes)
        {
            var data = new Dictionary<string, List<int>>
            {
                { "Average Combat Time", new List<int>() },
                { "Damage Window", new List<int>() },
                { "Total Dashes", new List<int>() }
            };

            // Read existing data
            if (File.Exists(filePath))
            {
                try
                {
                    string json = await ReadFromFileWithRetryAsync(filePath);
                    data = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(json) ?? data;
                    // Continue processing the file content...
                }
                catch (IOException ex)
                {
                    // Handle the error (e.g., log it or notify the user)
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

            }

            // Append new data
            data["Average Combat Time"].Add(averageCombatTime);
            data["Damage Window"].Add(damageWindow);
            data["Total Dashes"].Add(totalDashes);

            // Save updated data
            string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
            await WriteToFileWithRetryAsync(filePath, updatedJson);
        }

        public static async Task WriteToFileWithRetryAsync(string filePath, string content, int maxRetries = 3, int delayMilliseconds = 100)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    await File.WriteAllTextAsync(filePath, content);
                    return; // Success
                }
                catch (IOException) when (retry < maxRetries - 1)
                {
                    // Wait before retrying
                    await Task.Delay(delayMilliseconds);
                }
            }

            // If reached here, all retries failed
            throw new IOException($"Failed to write to {filePath} after {maxRetries} attempts.");
        }

        public static async Task<string> ReadFromFileWithRetryAsync(string filePath, int maxRetries = 3, int delayMilliseconds = 100)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    // Attempt to read the file
                    string content = await File.ReadAllTextAsync(filePath);
                    return content; // Success, return the content read
                }
                catch (IOException) when (retry < maxRetries - 1)
                {
                    // If an IOException is caught, wait for a bit before retrying
                    await Task.Delay(delayMilliseconds);
                }
            }

            // If all retries fail, rethrow the last exception or throw a custom exception
            throw new IOException($"Failed to read from {filePath} after {maxRetries} attempts.");
        }


        public static void ExecutePythonScript()
        {
            /*Data de exemplo do jogador:
            var sampleData = new Dictionary<string, List<int>>
        {
            { "Average Combat Time", new List<int> { 4, 8, 7, 10 } },
            { "Damage Window", new List<int> { 3, 2, 9, 5 } },
            { "Total Dashes", new List<int> { 1, 5, 2, 7 } }
        };

            string json_sampleData = JsonConvert.SerializeObject(sampleData);  // Converte os dados do jogador
            File.WriteAllText(@"PlayerData.json", json_sampleData); // Escreve eles em um JSON */


            string pythonScriptPath = "ProfileSelector.py"; // Caminho para script da IA
            RunPythonScript(pythonScriptPath); // Inicializa o script

            string json = File.ReadAllText("ProcessedData.json"); // Ler o JSON criado pelo Python 
            var resultData = JsonConvert.DeserializeObject<List<string>>(json); // Converter ele para uma lista

            // Checando se funcionou ou não
            if (resultData != null)
            {
                foreach (var item in resultData) //Para elemento presente na lista...
                {
                    Console.WriteLine(item); // faz um print do elemento.
                }
            }
            else // Caso não tenha data ele avisa pelo print
            {
                Console.WriteLine("No data!");
            }
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

            using (Process process = new Process { StartInfo = start }) // Metodo para verificação de erros
            {
                Console.WriteLine("Iniciando Script Python...");
                process.Start();

                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine("Output do Python:");
                    Console.WriteLine(result);
                }

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