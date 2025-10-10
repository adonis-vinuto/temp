from datetime import datetime
from controllers.ApiService import ApiService
from models.ReportObjectModel import ReportObjectModel
from models.ExpenseModel import ExpenseModel

class ReportObjectController:
    def __init__(self, api_service, db_connector):
        self.api_service = api_service
        self.report_object_model = ReportObjectModel(db_connector)
        self.expense_model = ExpenseModel(db_connector)
        print("\nRELATÓRIOS DE OBJETO")

    def _normalize_expense(self, raw: dict, report_id: int) -> dict:
        """
        Normaliza o payload de despesa da API para o formato esperado pelo modelo/BD.
        - Garante report_id
        - Corrige chaves (reicept_url->receipt_url, validate->expense_validate, on->expense_on)
        - Mantém nomes esperados em ExpenseModel (value->expense_value tratado dentro do model)
        """
        # Fonte possui 'expense_id' que, na prática, é o ID do relatório no VExpenses
        resolved_report_id = raw.get("report_id") or raw.get("expense_id") or report_id

        # Corrigir campos com nomes divergentes/typo
        receipt_url = raw.get("receipt_url") or raw.get("reicept_url")
        expense_validate = raw.get("expense_validate") or raw.get("validate")
        expense_on = raw.get("expense_on")
        if expense_on is None:
            # API manda 'on' como bool; o model espera 'expense_on' (int/bool)
            expense_on = raw.get("on")

        # Datas: manter string como vem (o model/driver pode lidar), mas se quiser, parseie aqui
        created_at = raw.get("created_at")
        updated_at = raw.get("updated_at")

        normalized = dict(raw)  # cópia rasa
        normalized.update({
            "report_id": resolved_report_id,
            "receipt_url": receipt_url,
            "expense_validate": expense_validate,
            "expense_on": int(bool(expense_on)) if expense_on is not None else None,
            "created_at": created_at,
            "updated_at": updated_at,
        })
        return normalized

    def get_report_with_expenses(self, report_id):
        try:
            report_data = self.api_service.get_report_with_expenses(report_id)

            if not report_data or 'data' not in report_data:
                print(f"Nenhum relatório encontrado com ID {report_id}.")
                return

            if 'expenses' in report_data['data'] and 'data' in report_data['data']['expenses']:
                expenses = report_data['data']['expenses']['data'] or []
                for expense in expenses:
                    # Normaliza e injeta report_id antes de qualquer operação no BD
                    expense = self._normalize_expense(expense, report_id)

                    expense_id = expense.get('id')
                    if expense_id is None:
                        print(f"Despesa sem ID: {expense}")
                        continue

                    existing_expense = self.expense_model.get_expense_by_id(expense_id)

                    if existing_expense:
                        self.expense_model.update_expense(expense)
                    else:
                        try:
                            self.expense_model.insert_expense(expense)
                        except Exception as e:
                            print(f"Erro ao inserir Despesa {expense_id}: {e}")
            else:
                print("\nNenhuma despesa encontrada para este relatório.\n")

        except Exception as e:
            print(f"Erro ao obter o relatório com ID {report_id}: {e}")
