# controllers/integration_controller.py

from models.DataExtractorModel import DataExtractor
from models.DataInserterModel import DataInserter


class IntegrationController:
    def __init__(self, source_config='bd.config', destination_config='bd2.config'):
        self.extractor = DataExtractor(source_config)
        self.inserter = DataInserter(destination_config)
        print("\nINTEGRAÇÃO ENTRE BD LOCAL E PROTHEUS")

    def run_integration(self):
        """
        Executa o processo de integração de dados.
        Realiza a extração e depois verifica se o registro existe no banco de destino.
        Caso exista, atualiza; caso contrário, insere o novo registro.
        """
        try:
            print("Iniciando o processo de integração de dados...")

            # Extrai dados do banco de origem
            data = self.extractor.fetch_data()

            if data:
                print(f"Dados extraídos: {len(data)} registros.")
                # Inserção/atualização dos dados no banco de destino
                self.inserter.insert_data(data)
                print("Integração de dados concluída com sucesso.")
            else:
                print("Nenhum dado foi encontrado para integração.")
        except Exception as e:
            print(f"Erro ao integrar dados: {e}")
