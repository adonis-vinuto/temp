import sys
import os

# Adicionar o caminho raiz ao sys.path
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from controllers.IntegrationController import IntegrationController           # Importar o controlador de Integração de dados entre os BDs
from controllers.DatabaseConnector import DatabaseConnector                   # Importar o conector com o Banco de Dados
from settings import Config
from controllers.ApiService import ApiService                               # Importar o serviço da API
from controllers.TeamMemberController import TeamMemberController           # Importar o controlador de Membro da Equipe
from controllers.ReportController import ReportController                   # Importar o controlador de Relatório
from controllers.ExpensesController import ExpensesController               # Importar o controlador de Despesas
from controllers.ExpenseTypesController import ExpenseTypesController       # Importar o controlador de Despesas
from controllers.CostCenterController import CostsCenterController          # Importar o controlador de Centros de Custo
from controllers.ProjectsController import ProjectsController               # Importar o controlador de Projetos
from controllers.ApprovalFlowsController import ApprovalFlowsController     # Importar o controlador de Fluxos de Aprovação
from controllers.CurrenciesController import CurrenciesController           # Importar o controlador de Fluxos de Moedas
from controllers.ReportObjectController import ReportObjectController       # Importar o controlador de Fluxos de Moedas

def main():
    try:
        # Carregar token da API
        config = Config('api_config.config')
        api_token = config.get_api_token()

        # Conectar ao banco de dados
        db_connector = DatabaseConnector('bd.config')

        db_connector2 = DatabaseConnector('bd2.config')
        dbconnector2 = 'bd2.config'

        # Inicializar serviço da API
        api_service = ApiService(api_token)

        # Inicializar e executar o controlador de Membros de Equipe
        # team_member_controller = TeamMemberController(api_service, db_connector, db_connector2)
        # team_member_controller.import_team_members()

        # Inicializar e executar o controlador de Despesas - Agora não precisa mais
        # expenses_controller = ExpensesController(api_service, db_connector)
        # expenses_controller.import_expenses()  # Função responsável pela carga de despesas

        # Inicializar e executar o controlador de Tipo de Despesas
        # expense_types_controller = ExpenseTypesController(api_service, db_connector)
        # expense_types_controller.import_expense_types()  # Função responsável pela carga de tipos de despesas

        # Inicializar e executar o controlador de Centros de Custo
        # costs_center_controller = CostsCenterController(api_service, db_connector)
        # costs_center_controller.import_cost_centers()   # Função responsável pela carga de Centros de Custo

        # Inicializar e executar o controlador de Projetos
        # projects_controller = ProjectsController(api_service, db_connector)
        # projects_controller.import_projects()   # Função responsável pela carga de Projetos

        # Inicializar e executar o controlador de Fluxos de Aprovação
        # approval_flow_controller = ApprovalFlowsController(api_service, db_connector)
        # approval_flow_controller.import_approval_flows()   # Função responsável pela carga de Fluxos de Aprovação

        # Inicializar e executar o controlador de Moedas
        # currency_controller = CurrenciesController(api_service, db_connector)
        # currency_controller.import_currencies()   # Função responsável pela carga de Moedas

        # Inicializar e executar o controlador de Relatórios
        # report_controller = ReportController(api_service, db_connector)
        # report_controller.import_reports()

        # Inicializar e executar o controlador de Centros de Custo
        # costs_center_controller = CostsCenterController(api_service, db_connector)
        # costs_center_controller.import_cost_centers_from_expenses()   # Função responsável pela carga de Centros de Custo

        # Inicializar e executar o controlador de Integração
        integration_controller = IntegrationController(source_config='bd.config', destination_config='bd.config')
        integration_controller.run_integration()

    except Exception as e:
        print(f"Erro na execução: {e}")
    finally:
        db_connector.close()

if __name__ == '__main__':
    main()