import sys
import os
import logging
import traceback
from datetime import datetime

# ==============================
# Função para resolver caminhos corretos (funciona no .exe e no modo dev)
# ==============================
def base_dir():
    """Retorna o diretório base, mesmo quando empacotado em .exe."""
    if getattr(sys, 'frozen', False) and hasattr(sys, '_MEIPASS'):
        # Quando o script é empacotado pelo PyInstaller
        return os.path.dirname(sys.executable)
    # Modo desenvolvimento
    return os.path.dirname(os.path.abspath(__file__))


# ==============================
# Configuração de LOG
# ==============================
def configurar_logger():
    """Configura o sistema de log em arquivo e no console."""
    log_dir = os.path.join(base_dir(), "logs")
    os.makedirs(log_dir, exist_ok=True)

    log_file = os.path.join(log_dir, f"execution_{datetime.now().strftime('%Y-%m-%d')}.log")

    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s [%(levelname)s] %(message)s",
        handlers=[
            logging.FileHandler(log_file, encoding='utf-8'),
            logging.StreamHandler(sys.stdout)
        ]
    )


# ==============================
# Ajuste do sys.path
# ==============================
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

# ==============================
# Importações dos controladores
# ==============================
from controllers.IntegrationController import IntegrationController
from controllers.DatabaseConnector import DatabaseConnector
from settings import Config
from controllers.ApiService import ApiService
from controllers.TeamMemberController import TeamMemberController
from controllers.ReportController import ReportController
from controllers.ExpensesController import ExpensesController
from controllers.ExpenseTypesController import ExpenseTypesController
from controllers.CostCenterController import CostsCenterController
from controllers.ProjectsController import ProjectsController
from controllers.ApprovalFlowsController import ApprovalFlowsController
from controllers.CurrenciesController import CurrenciesController
from controllers.ReportObjectController import ReportObjectController


# ==============================
# Função principal
# ==============================
def main():
    configurar_logger()
    logging.info("=== Iniciando execução principal ===")

    # Configura caminhos dos arquivos .config
    api_config_path = os.path.join(base_dir(), 'api_config.config')
    bd_config_path = os.path.join(base_dir(), 'bd.config')
    bd2_config_path = os.path.join(base_dir(), 'bd2.config')

    db_connector = None  # Inicializa para evitar NameError no finally

    try:
        # ==============================
        # Carregar token da API
        # ==============================
        config = Config(api_config_path)
        api_token = config.get_api_token()
        logging.info("Token da API carregado com sucesso.")

        # ==============================
        # Conectar ao banco de dados
        # ==============================
        db_connector = DatabaseConnector(bd_config_path)
        db_connector2 = DatabaseConnector(bd2_config_path)
        logging.info("Conexões com bancos de dados estabelecidas.")

        # ==============================
        # Testar conexão com o banco
        # ==============================
        try:
            # Aqui assumimos que o DatabaseConnector tem um método "get_connection"
            # ou "engine"/"connection" compatível com o método .execute()
            conn = db_connector.get_connection() if hasattr(db_connector, "get_connection") else db_connector
            test_cursor = conn.cursor() if hasattr(conn, "cursor") else None

            if test_cursor:
                test_cursor.execute("SELECT 1;")
                result = test_cursor.fetchone()
                logging.info(f"✅ Teste de conexão com o banco bem-sucedido: {result}")
                test_cursor.close()
            else:
                logging.warning("⚠️ Conector de banco não possui cursor direto para teste — ignorando verificação SQL.")

        except Exception as db_test_error:
            logging.error("❌ Erro ao testar a conexão com o banco de dados:")
            logging.error(str(db_test_error))
            logging.error(traceback.format_exc())
            logging.error("Encerrando execução, pois o banco não está acessível.")
            return  # Sai da função main() sem rodar a integração

        # ==============================
        # Inicializar serviço da API
        # ==============================
        api_service = ApiService(api_token)
        logging.info("Serviço da API inicializado.")

        # ==============================
        # Executar controladores
        # ==============================
        team_member_controller = TeamMemberController(api_service, db_connector, db_connector2)
        team_member_controller.import_team_members()
        logging.info("Importação de membros da equipe concluída.")

        expenses_controller = ExpensesController(api_service, db_connector)
        expenses_controller.import_expenses()
        logging.info("Importação de despesas concluída.")

        expense_types_controller = ExpenseTypesController(api_service, db_connector)
        expense_types_controller.import_expense_types()
        logging.info("Importação de tipos de despesas concluída.")

        costs_center_controller = CostsCenterController(api_service, db_connector)
        costs_center_controller.import_cost_centers()
        logging.info("Importação de centros de custo concluída.")

        projects_controller = ProjectsController(api_service, db_connector)
        projects_controller.import_projects()
        logging.info("Importação de projetos concluída.")

        approval_flow_controller = ApprovalFlowsController(api_service, db_connector)
        approval_flow_controller.import_approval_flows()
        logging.info("Importação de fluxos de aprovação concluída.")

        currency_controller = CurrenciesController(api_service, db_connector)
        currency_controller.import_currencies()
        logging.info("Importação de moedas concluída.")

        report_controller = ReportController(api_service, db_connector)
        report_controller.import_reports()
        logging.info("Importação de relatórios concluída.")

        costs_center_controller = CostsCenterController(api_service, db_connector)
        costs_center_controller.import_cost_centers_from_expenses()
        logging.info("Atualização de centros de custo a partir das despesas concluída.")

        integration_controller = IntegrationController(
            source_config=bd_config_path,
            destination_config=bd_config_path
        )
        integration_controller.run_integration()
        logging.info("Integração entre bancos concluída com sucesso.")

    except Exception as e:
        logging.error("❌ Ocorreu um erro na execução:")
        logging.error(str(e))
        logging.error(traceback.format_exc())

    finally:
        if db_connector:
            try:
                db_connector.close()
                logging.info("Conexão com o banco de dados encerrada.")
            except Exception as e:
                logging.warning(f"Erro ao encerrar conexão com o banco: {e}")

        logging.info("=== Execução finalizada ===")


# ==============================
# Ponto de entrada
# ==============================
if __name__ == '__main__':
    main()
