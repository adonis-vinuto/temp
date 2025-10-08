from controllers.ApiService import ApiService
from models.CostCenterModel import CostCentersModel
from models.ExpenseModel import ExpenseModel
from models.ReportModel import ReportModel

class ExpenseObjectController:
    def __init__(self, api_service, db_connector):
        self.api_service = api_service
        self.cost_center_model = CostCentersModel(db_connector)
        self.expense_model = ExpenseModel(db_connector)  # Instancia a ExpenseModel
        self.report_model = ReportModel(db_connector)    # Instancia a ReportModel
        print("\nPROCESSANDO DESPESAS E CENTROS DE CUSTO")

    def get_all_expenses_ids(self):
        """
        Obtém todos os IDs das despesas.
        :return: Lista com os IDs das despesas.
        """
        try:
            expense_ids = self.expense_model.get_all_expenses_ids()  # Assumindo que há esse método no ExpenseModel
            return expense_ids
        except Exception as e:
            print(f"❌ Erro ao buscar IDs das despesas: {e}")
            return []

    def import_costs_center_from_expenses(self, expense_ids):
        """
        Busca os objetos 'costs_center' de despesas via API e os insere ou atualiza no BD.
        :param expense_ids: Lista de IDs de despesas para buscar os centros de custo.
        """
        try:
            for expense_id in expense_ids:
                # print(f"Processando despesa com ID: {expense_id}...")

                # Busca os detalhes da despesa pelo ID, incluindo o 'costs_center'
                expense_data = self.api_service.get_expense_details(expense_id, include="costs_center")
                
                # Verifica se há dados retornados
                if not expense_data:
                    print(f"⚠️ Nenhum dado retornado para a despesa {expense_id}.")
                    continue

                # Extrai o objeto 'costs_center' da resposta
                costs_center_data = expense_data.get("data", {}).get("costs_center", {}).get("data")

                # Verifica se costs_center_data é um dicionário antes de tentar acessar o 'id'
                if isinstance(costs_center_data, dict):
                    # Agora que sabemos que costs_center_data é um dicionário, podemos acessar o 'id' corretamente
                    cost_center_id = costs_center_data.get("id")
                    # print(f"ID Centro de Custo: {cost_center_id}")
                    
                    # Verifica se o Centro de Custo já existe no banco de dados
                    existing_cost_center = self.cost_center_model.get_cost_center_by_id(cost_center_id)

                    if self.cost_center_model.cost_center_exists(cost_center_id):
                        # Atualiza
                        self.cost_center_model.update_cost_center(costs_center_data)
                        # print(f"✅ Centro de Custo com ID {cost_center_id} atualizado.")
                    else:
                        # Insere
                        self.cost_center_model.insert_cost_centers(costs_center_data)
                        # print(f"✅ Novo Centro de Custo com ID {cost_center_id} inserido.")

                    
                    # Atualiza a coluna 'cost_center_id' na despesa
                    expense = {"id": expense_id, "cost_center_id": cost_center_id}  # Prepara os dados da despesa
                    self.expense_model.update_expense(expense)  # Chama o método para atualizar a despesa

                else:
                    # Se não for um dicionário válido, imprime erro e pula para a próxima despesa
                    print(f"⚠️ 'costs_center_data' não é um dicionário válido para a despesa {expense_id}. Dados recebidos: {costs_center_data}")
                    continue  # Continua para a próxima despesa

            print("\n🚀 Processamento de Centros de Custo concluído com sucesso!")
        
        except Exception as e:
            print(f"❌ Erro ao processar Centros de Custo: {e}")

    def import_reports_from_expenses(self, expense_ids):
        """
        Busca o objeto 'report' de cada despesa via API e insere/atualiza no BD.
        Também atualiza a coluna 'report_id' da despesa.
        """
        try:
            for expense_id in expense_ids:
                # Busca detalhes da despesa, incluindo o 'report'
                expense_data = self.api_service.get_expense_details(expense_id, include="report")

                if not expense_data:
                    print(f"⚠️ Nenhum dado retornado para a despesa {expense_id}.")
                    continue

                # Extrai o objeto 'report' da resposta
                report_data = expense_data.get("data", {}).get("report", {}).get("data")
                if isinstance(report_data, dict):
                    report_id = report_data.get("id")

                    if not report_id:
                        print(f"⚠️ Relatório sem ID para a despesa {expense_id}. Dados: {report_data}")
                        continue
                    # Verifica se o relatório já existe no banco
                    existing_report = self.report_model.get_report_by_id(report_id)
                    if existing_report:
                        # Atualiza
                        self.report_model.update_report(report_data)
                        print(f"✅ Relatório {report_id} atualizado.")
                    else:
                        # Insere
                        self.report_model.insert_report(report_data)
                        print(f"✅ Novo relatório {report_id} inserido.")

                    # Atualiza a despesa com o report_id
                    expense = {"id": expense_id, "report_id": report_id}
                    self.expense_model.update_expense(expense)

                else:
                    print(f"⚠️ 'report_data' não é um dicionário válido para a despesa {expense_id}. Dados recebidos: {report_data}")
                    continue

            print("\n🚀 Processamento de Relatórios concluído com sucesso!")

        except Exception as e:
            print(f"❌ Erro ao processar Relatórios: {e}")