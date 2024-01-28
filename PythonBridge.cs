using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace MyGame
{
    public static class PythonBridge
    {
        public static void ExecutePythonScript()
        {
            //Data de exemplo do jogador:
            var sampleData = new Dictionary<string, List<int>>
        {
            { "Average Combat Time", new List<int> { 4, 8, 7, 10 } },
            { "Damage Window", new List<int> { 3, 2, 9, 5 } },
            { "Total Dashes", new List<int> { 1, 5, 2, 7 } }
        };

            string json_sampleData = JsonConvert.SerializeObject(sampleData);  // Converte os dados do jogador
            File.WriteAllText(@"PlayerData.json", json_sampleData); // Escreve eles em um JSON


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