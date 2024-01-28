import numpy as np
import pandas as pd
from sklearn.neighbors import KNeighborsClassifier
from sklearn.preprocessing import StandardScaler
import json
import warnings

# Ignorar warnings das bibliotecas
warnings.filterwarnings("ignore", category=FutureWarning)



def main():
    # Definição base de cada perfil do jogador
    data = {
        "Average Combat Time": [3, 6, 9],
        "Damage Window": [3, 3, 6],
        "Total Dashes": [2, 6, 8],
        "Player Type": ["Berzerk", "Strategist", "Fencer"]
    }


    # Cria o dataframe para usar de base no modelo KNN 
    df = pd.DataFrame(data)

    # Mostra o dataframe
    #print(df)

    # Preprocessamento da data do perfil de jogador
    X = df.drop('Player Type', axis=1)
    y = df['Player Type']

    # Normaliza os valores para diferentes tamanhos entre dados não ter pesos maiores que outros
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)

    # Implementação do KNN
    knn = KNeighborsClassifier(n_neighbors=1) # utilizando apenas 1 neighbor para classificação
    knn.fit(X_scaled, y) # Define os limites do modelo, utilizando os valores dados como média e aplicando essas médias para cada perfil

    # Abre um JSON que contem a data passada pelo código de C#
    with open('PlayerData.json', 'r') as file:
        data = json.load(file)

    # Converte para um array numpy
    csharp_data = {key: np.array(value) for key, value in data.items()}


    # Criando Dataframe utilizando os valores do C#
    samples_df = pd.DataFrame(csharp_data)

    # Normalizando valores do C#
    samples_scaled = scaler.transform(samples_df)


    # Predição de valores utilizando o KNN
    predictions = knn.predict(samples_scaled)

    # Organizando os valores em uma lista 
    predictions_list = predictions.tolist()

    # Definindo o caminho para o Json onde será armazenado os resultados das predições
    file_path = 'ProcessedData.json'

    # Gravando as predições no 'data.json'
    with open(file_path, 'w') as file:
        json.dump(predictions_list, file)

    #Imprimindo os valores (Usado para correção de bugs)
    #print(predictions_list)

#Inicialização do código
if __name__ == "__main__": 
    main()