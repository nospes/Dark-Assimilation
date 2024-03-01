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
        '1': { # Esqueleto (Esqueleto com maça)
            "Average Combat Time": [3, 5, 7, 9],
            "Damage Window": [1, 2, 3, 4],
            "Total Dashes": [0, 1, 2, 3],
            "Player Type": ["Berzerk", "Berzerk", "Berzerk", "Berzerk"]
        },
        '2': { # Swarm (Cranios de fogo)
            "Average Combat Time": [2, 4, 6, 8],
            "Damage Window": [2, 3, 4, 5],
            "Total Dashes": [1, 2, 3, 4],
            "Player Type": ["TypeA", "TypeA", "TypeA", "TypeA"]
        },
        '3': { #Arqueiro
            "Average Combat Time": [2, 4, 6, 8],
            "Damage Window": [2, 3, 4, 5],
            "Total Dashes": [1, 2, 3, 4],
            "Player Type": ["TypeB", "TypeB", "TypeB", "TypeB"]
        },
        '4': {  #Mago
            "Average Combat Time": [2, 4, 6, 8],
            "Damage Window": [2, 3, 4, 5],
            "Total Dashes": [1, 2, 3, 4],
            "Player Type": ["TypeC", "TypeC", "TypeC", "TypeC"]
        },
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

    for enemy_type in set(enemy_types):  # Utiliza um 'for' para ter certeza que vai rodar o modelo para CADA TIPO de inimigo
        df = load_base_data(enemy_type)
        
        # Preprocessamento da data do perfil do player
        
        # X: Feature set - Esta linha remove a coluna 'Player Type' do dataframe, deixando apenas os recursos (colunas) que serão usados ​​como entrada para o modelo KNN.
        X = df.drop('Player Type', axis=1)

        # y: Variável de destino - Extrai a coluna 'Tipo de jogador' do dataframe e a usa como variável de destino para o modelo KNN. O modelo aprenderá a prever o ‘Tipo de Jogador’ com base nos recursos de entrada fornecidos em X.
        y = df['Player Type']
        
        #Cria uma instância da classe para normalizar valores
        scaler = StandardScaler()
        # Normaliza os valores para diferentes tamanhos entre dados não ter pesos maiores que outros
        X_scaled = scaler.fit_transform(X)
        
        
        # Cria uma instância da classe KNeighborsClassifier do scikit-learn
        knn = KNeighborsClassifier(n_neighbors=1)

        '''
        O método fit é chamado no objeto knn com dois argumentos: X_scaled e y. X_scaled é a versão 
        dimensionada dos recursos de entrada, garantindo que todos os recursos contribuam igualmente para 
        o cálculo da distância. O dimensionamento é crucial para algoritmos baseados em distância como 
        KNN. y é a variável alvo (por exemplo, 'Tipo de jogador') que o modelo está tentando prever. 
        Esta etapa “ensina” ao modelo o relacionamento entre os recursos de entrada e a variável de destino.
        '''
        knn.fit(X_scaled, y)    
        
        # Constrói um dicionário chamado incoming_data, mapeando cada nome de recurso
        incoming_data = {
            "Average Combat Time": data["Average Combat Time"],
            "Damage Window": data["Damage Window"],
            "Total Dashes": data["Total Dashes"]
        }
        
        # Gera uma lista de índices para linhas nos dados que correspondem ao tipo de inimigo atual que está sendo processado.
        # Isso filtra os dados para incluir apenas as instâncias que correspondem ao tipo de inimigo.
        indices = [i for i, e in enumerate(enemy_types) if e == enemy_type]
        filtered_data = {k: [v[i] for i in indices] for k, v in incoming_data.items()}
        
        # Para cada recurso nos dados recebidos filtra os valores não associados ao tipo de inimigo atual.
        # Isso cria um subconjunto dos dados originais com valores apenas para o tipo de inimigo especificado
        samples_df = pd.DataFrame(filtered_data)
        #Escala os dados filtrados
        samples_scaled = scaler.transform(samples_df)
        
        # Chama o KNN para aplicar a predição na data escalada e organizada por tipo de inimigo
        predictions = knn.predict(samples_scaled)
        
        # Converte os resultados adicionando-os a uma lista
        predictions_list.extend(predictions.tolist())

    # Converte a lista de resultado para um JSON - 'ProcessedData.json', os valores são lidos em C#
    with open('ProcessedData.json', 'w') as file:
        json.dump(predictions_list, file)
    
    # Da o print da lista
    print(predictions_list)

if __name__ == "__main__":
    main()

