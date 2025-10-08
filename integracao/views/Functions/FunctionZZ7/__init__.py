# import os
# import azure.functions as func
# import sys

# # Adicionar o caminho raiz ao sys.path
# sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

# from controllers.IntegrationController import IntegrationController  # Importar o controlador de Integração de dados entre os BDs
# from controllers.DatabaseConnector import DatabaseConnector          # Importar o conector com o Banco de Dados

# def main(req: func.HttpRequest) -> func.HttpResponse:
#     """
#     Função HTTP Trigger do Azure para executar o script de integração.

#     Retorna:
#        - 200: Se a integração foi executada com sucesso.
#        - 500: Em caso de erro.
#     """
#     try:
#         # Conectar ao banco de dados
#         db_connector = DatabaseConnector('bd.config')
#         db_connector2 = 'bd2.config'

#         # Inicializar e executar o controlador de Integração
#         integration_controller = IntegrationController(source_config='bd.config', destination_config=db_connector2)
#         integration_controller.run_integration()

#         # Fechar conexão
#         db_connector.close()

#         # Retornar sucesso
#         return func.HttpResponse("Integração executada com sucesso.", status_code=200)

#     except Exception as e:
#         # Log de erro
#         print(f"Erro na execução: {e}")

#         # Retornar erro
#         return func.HttpResponse(f"Erro na execução: {str(e)}", status_code=500)
