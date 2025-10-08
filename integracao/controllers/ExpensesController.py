# ExpensesController.py
from controllers.ApiService import ApiService
from models.ExpenseModel import ExpenseModel  # Importar a model
from datetime import datetime, timedelta

class ExpensesController:
    def __init__(self, api_service, db_connector):
        """
        Inicializa o controlador de despesas, configurando a conexão com o serviço de API
        e o conector de banco de dados, além de instanciar o modelo de despesas.
        """
        self.api_service = api_service
        self.db_connector = db_connector
        self.expense_model = ExpenseModel(db_connector)  # Criar instância da model
        print("\nDESPESAS")

    def get_last_updated_date(self):
        """
        Retorna a MAIOR data de updated_at (ou 2025-09-01 se não houver).
        Usa TRY_CAST para evitar erros de conversão.
        """
        try:
            print("Consultando a última data de atualização no banco de dados...")
            cursor = self.db_connector.get_connection().cursor()
            query = """
                SELECT MAX(TRY_CAST(updated_at AS DATETIME2))
                FROM VEXP_Expenses
            """
            cursor.execute(query)
            result = cursor.fetchone()[0]
            cursor.close()

            if result:
                # Retorna só a parte da data (YYYY-MM-DD)
                result_str = result.strftime("%Y-%m-%d")
                print(f"Última data de atualização encontrada: {result_str}")
                return result_str
            else:
                print("Nenhuma data encontrada, usando data padrão '2025-09-01'.")
                return "2025-09-01"

        except Exception as e:
            print(f"Erro SQL em get_last_updated_date: {e}")
            return "2025-09-01"

    def import_expenses(self, end_date=None):
        """
        Busca as despesas via API em janelas de até 6 meses,
        começando da última data registrada no banco (ou 2025-09-01 se não houver),
        e insere ou atualiza no banco de dados.
        """
        try:
            # Pega a última data registrada no banco (ou 2025-09-01 se vazio/erro)
            start_date = self.get_last_updated_date()

            if end_date is None:
                end_date = datetime.now().strftime('%Y-%m-%d')  # Data atual por padrão

            # Converte para datetime
            start_dt = datetime.strptime(start_date, "%Y-%m-%d")
            end_dt = datetime.strptime(end_date, "%Y-%m-%d")

            # Se a data inicial for maior que a final, não faz nada
            if start_dt > end_dt:
                print("A data inicial é maior que a final. Nada a importar.")
                return

            # Processar em blocos de no máximo 6 meses (~180 dias)
            chunk_size = 180
            current_start = start_dt

            while current_start <= end_dt:
                current_end = min(current_start + timedelta(days=chunk_size), end_dt)

                print(f"\nBuscando despesas entre {current_start.date()} e {current_end.date()}...")

                # Buscar despesas nesse intervalo
                expenses = self.api_service.get_expenses(
                    current_start.strftime('%Y-%m-%d'),
                    current_end.strftime('%Y-%m-%d')
                )

                if not expenses:
                    print("Nenhuma despesa encontrada nesse período.")
                else:
                    for expense in expenses:
                        expense_id = expense.get('id')
                        if not expense_id:
                            print(f"Despesa sem ID: {expense}")
                            continue

                        existing_expense = self.expense_model.get_expense_by_id(expense_id)

                        if existing_expense:
                            # Verifica atualização
                            expense_updated_at_str = expense.get('updated_at')
                            if not expense_updated_at_str:
                                continue

                            try:
                                # Trata frações de segundos também
                                expense_updated_at = datetime.strptime(expense_updated_at_str, '%Y-%m-%d %H:%M:%S.%f')
                            except ValueError:
                                try:
                                    expense_updated_at = datetime.strptime(expense_updated_at_str, '%Y-%m-%d %H:%M:%S')
                                except ValueError:
                                    continue

                            db_updated_at = existing_expense.get('updated_at')

                            # Normaliza db_updated_at
                            if isinstance(db_updated_at, str):
                                parsed = None
                                for fmt in ("%Y-%m-%d %H:%M:%S.%f", "%Y-%m-%d %H:%M:%S", "%Y-%m-%d"):
                                    try:
                                        parsed = datetime.strptime(db_updated_at, fmt)
                                        break
                                    except ValueError:
                                        continue
                                db_updated_at = parsed
                            elif not isinstance(db_updated_at, datetime):
                                db_updated_at = None

                            if db_updated_at is None or expense_updated_at > db_updated_at:
                                self.expense_model.update_expense(expense)
                        else:
                            try:
                                self.expense_model.insert_expense(expense)
                            except Exception as e:
                                print(f"Erro ao inserir {expense_id}: {e}")

                    print(f"Total de {len(expenses)} despesas processadas no período {current_start.date()} a {current_end.date()}.")

                # Avança para o próximo bloco
                current_start = current_end + timedelta(days=1)

        except Exception as e:
            print("Erro ao importar despesas:", e)
