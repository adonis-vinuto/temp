from datetime import datetime
from controllers.ApiService import ApiService
from models.ReportObjectModel import ReportObjectModel
from models.ExpenseModel import ExpenseModel

class ReportObjectController:
    def __init__(self, api_service, db_connector):
        self.api_service = api_service
        self.report_object_model = ReportObjectModel(db_connector)  # Modelo para relatórios
        self.expense_model = ExpenseModel(db_connector)  # Modelo para despesas
        print("\nRELATÓRIOS DE OBJETO")

    def get_report_with_expenses(self, report_id):
        """
        Obtém um relatório específico junto com suas despesas associadas
        através da API.
        """
        try:
            # print(f"Buscando relatório com ID {report_id} e suas despesas...")
            report_data = self.api_service.get_report_with_expenses(report_id)

            if not report_data or 'data' not in report_data:
                print(f"Nenhum relatório encontrado com ID {report_id}.")
                return

            # Verifica se há despesas para o relatório
            if 'expenses' in report_data['data'] and 'data' in report_data['data']['expenses']:
                expenses = report_data['data']['expenses']['data']
                
                # Verifica se há despesas
                if expenses:
                    #print("\nDespesas Associadas:")
                    for expense in expenses:

                        expense_id = expense.get('id')  # Usar .get() para evitar KeyError

                        if expense_id is None:
                            print(f"Despesa sem ID: {expense}")
                            continue

                        # Verificar se o ID já existe no banco de dados
                        existing_expense = self.expense_model.get_expense_by_id(expense_id)

                        if existing_expense:
                            # Atualizar a despesa se a data de atualização da API for mais recente
                            # print(f"Atualizando despesa com ID {expense_id}...")
                            self.expense_model.update_expense(expense)
                        else:
                            # Inserir uma nova despesa
                            try:
                                # print(f"Inserindo nova despesa com ID {expense_id}...")
                                self.expense_model.insert_expense(expense)
                            except Exception as e:
                                print(f"Erro ao inserir Despesa {expense_id}: {e}")

                else:
                    print("Nenhuma despesa associada ao relatório.")

                #print("\nTodas as despesas foram processadas com sucesso.\n")
            else:
                print("\nNenhuma despesa encontrada para este relatório.\n")

        except Exception as e:
            print(f"Erro ao obter o relatório com ID {report_id}: {e}")
