from controllers.ApiService import ApiService
from models.CurrencyModel import CurrenciesModel

class CurrenciesController:
    def __init__(self, api_service, dbconnector):
        self.api_service = api_service
        self.currency_model = CurrenciesModel(dbconnector)
        print("\nMOEDAS")

    def import_currencies(self):
        """
        Busca as Moedas via API e insere os cadastros no BD após truncar a tabela.
        """
        try:
            # Chamada de API para buscar as Moedas
            currencies = self.api_service.get_currencies()

            #print("Moedas recebidas: ", currencies)        # Imprimir as Moedas recebidas

            if not currencies:
                print("Nenhuma Moeda recebida.")
                return

            # Truncar a tabela antes de inserir os novos dados
            self.currency_model.truncate_currencies()

            # Inserir as novas Moedas no BD
            for currency in currencies:
                # Certifique-se de que cada 'currency' é um dicionário
                if isinstance(currency, dict):
                    self.currency_model.insert_currencies(currency)
                else:
                    print(f"Formato inválido de moeda: {currency}")

            print(f"Total de {len(currencies)} Moedas processadas com sucesso.")

        except Exception as e:
            print(f"Erro ao importar Moedas: {e}")
