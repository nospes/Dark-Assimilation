import numpy as np
import pandas as pd
from sklearn.neighbors import KNeighborsClassifier
from sklearn.preprocessing import StandardScaler
import json
import warnings
warnings.filterwarnings("ignore", category=FutureWarning)

def load_base_data(enemy_type):
    # Define o tipo de DataFrame usado para a predições de acordo com tipo de inimigo

    # Tipo de perfil de acordo com os dados de jogo
    base_data = {
        #Skeleton
        '1': {"Average Combat Time": [7, 11, 25], "Damage Window": [6, 9, 25], "Total Dashes":[2, 4, 7], "Player Type": ["Aggressive", "Balanced", "Evasive"]},
        #Swarm
        '2': {"Average Combat Time": [3, 10, 25], "Damage Window": [3, 8, 20], "Total Dashes":[1, 2, 4], "Player Type": ["Aggressive", "Balanced", "Evasive"]},
        #Archer
        '3': {"Average Combat Time": [6, 13, 25], "Damage Window": [5, 9, 22], "Total Dashes":[2, 4, 9], "Player Type": ["Aggressive", "Balanced", "Evasive"]},
        #Mage
        '4': {"Average Combat Time": [5, 11, 25], "Damage Window": [4, 8, 19], "Total Dashes":[2, 4, 7], "Player Type": ["Aggressive", "Balanced", "Evasive"]},
    }
    return pd.DataFrame(base_data[str(enemy_type)])

def main():
    # Carrega o JSON que foi inscrito pelo codigo em C# (pythonbridge.cs)
    with open('PlayerData.json', 'r') as file:
        data = json.load(file)
    
    if not data:  # Caso a data esteja vazia...
        print("No player data available. Canceling prediction.")
        return
    
    # Escolhe data usando como base tipo de inimigo
    enemy_types = data["Enemy Type"]

    # Cria uma lista para armazenar as predições do modelo KNN
    predictions_list = []

    # Filtra as datas corrompidas, no caso os Damage Window e Average Combat Time que sejam iguais a 0
    valid_indices = [i for i, (dw, act) in enumerate(zip(data["Damage Window"], data["Average Combat Time"])) if dw != 0 and act != 0]
    # Escolhe data usando como base tipo de inimigo valido
    valid_enemy_types = [enemy_types[i] for i in valid_indices]


    for enemy_type in set(valid_enemy_types): # Utiliza um 'for' para ter certeza que vai rodar o modelo para CADA TIPO de inimigo VALIDO
        df = load_base_data(enemy_type)
        X = df.drop('Player Type', axis=1)  # X: Feature set - Esta linha remove a coluna 'Player Type' do dataframe, deixando apenas os recursos (colunas) que serão usados ​​como entrada para o modelo KNN.
        y = df['Player Type']  # y: Variável de destino - Extrai a coluna 'Tipo de jogador' do dataframe e a usa como variável de destino para o modelo KNN. O modelo aprenderá a prever o ‘Tipo de Jogador’ com base nos recursos de entrada fornecidos em X.

        scaler = StandardScaler()   #Cria uma instância da classe para normalizar valores
        X_scaled = scaler.fit_transform(X)  # Normaliza os valores
        
        # Cria uma instância da classe KNeighborsClassifier do scikit-learn
        knn = KNeighborsClassifier(n_neighbors=1)

        '''
        O método fit é chamado no objeto knn com dois argumentos: X_scaled e y. X_scaled é a versão 
        dimensionada dos recursos de entrada, garantindo que todos os recursos contribuam igualmente para 
        o cálculo da distância. O dimensionamento é crucial para algoritmos baseados em distância como 
        KNN. y é a variável alvo (por exemplo, 'Tipo de jogador') que o modelo está tentando prever. 
        Esta etapa “ensina” ao modelo o relacionamento entre os recursos de entrada e a variável de destino.
        '''
        # The fit method is called on the knn object
        knn.fit(X_scaled, y)  

        # Filtrando a data na qual indice estejam corretos
        indices = [i for i in valid_indices if enemy_types[i] == enemy_type]
        filtered_data = {k: [v[i] for i in indices] for k, v in data.items() if k in ["Average Combat Time", "Damage Window", "Total Dashes"]}

        # Para cada recurso nos dados recebidos filtra os valores não associados ao tipo de inimigo atual.
        # Isso cria um subconjunto dos dados originais com valores apenas para o tipo de inimigo especificado
        samples_df = pd.DataFrame(filtered_data)
        if samples_df.empty:  # If no data remains after filtering, skip this iteration
            continue

        #Escala os dados filtrados
        samples_scaled = scaler.transform(samples_df)
        
        # Chama o KNN para aplicar a predição na data escalada e organizada por tipo de inimigo
        predictions = knn.predict(samples_scaled)

        # Converte os resultados adicionando-os a uma lista
        predictions_list.extend(predictions.tolist())

     # Converte a lista de resultado para um JSON - 'ProcessedData.json', os valores são lidos em C#
    with open('ProcessedData.json', 'w') as file:
        json.dump(predictions_list, file)
    
    #print(predictions_list)

if __name__ == "__main__":
    main()
