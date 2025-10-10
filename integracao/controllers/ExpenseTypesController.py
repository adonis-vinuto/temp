# ExpenseTypesController.py
from controllers.ApiService import ApiService
from models.ExpenseTypeModel import ExpenseTypesModel  # Importar a model
from datetime import datetime

class ExpenseTypesController:
    def __init__(self, api_service, db_connector):
        """
        Inicializa o controlador de tipo de despesas, configurando a conexão com o serviço de API
        e o conector de banco de dados, além de instanciar o modelo de despesas.
        """
        self.api_service = api_service
        self.db_connector = db_connector
        self.expense_types_model = ExpenseTypesModel(db_connector)  # Criar a instância da model
        print("\nTIPO DE DESPESA")
    
    def get_last_updated_date(self):
        """
        Retorna a MAIOR data de updated_at ou None se não houver.
        Usa TRY_CAST para ignorar valores inválidos.
        """
        cursor = self.db_connector.get_connection().cursor()
        try:
            query = """
                SELECT MAX(TRY_CAST(updated_at AS DATETIME2))
                FROM VEXP_ExpenseTypes
            """
            cursor.execute(query)
            result = cursor.fetchone()[0]
            return result  # retorna datetime ou None
        except Exception as e:
            print(f"Erro SQL em ExpenseTypesModel.get_last_updated_date: {e}")
            return None
        finally:
            cursor.close()
        
    def import_expense_types(self):
        """
        Busca os tipos de despesas via API, utilizando a última data de atualização registrada no banco de 
        dados como ponto de partida, e insere ou atualiza os tipos de despesa no banco de dados.
        """
        try:
            # Consultar a última data de atualização
            # start_date = self.get_last_updated_date()
            start_date = '2025-10-01'
            # Definir a data atual como end_date
            end_date = datetime.now().strftime('%Y-%m-%d')
            print(f"Buscando tipos de despesas entre {start_date} e {end_date}...")

            # Chamar a API para buscar os tipos de despesas nesse intervalo
            expense_types = self.api_service.get_expense_types(start_date, end_date)

            if not expense_types:
                print("Nenhum tipo de despesa encontrado.")
                return
            
            # Inserir ou atualizar os tipos de despesas no BD
            for expense_type in expense_types:
                expense_type_id = expense_type.get('id')

                if not expense_type_id:
                    print(f"Tipo de despesa sem ID: {expense_type}")
                    continue

                existing_expense_type = self.expense_types_model.get_expense_type_by_id(expense_type_id)

                if existing_expense_type:
                    expense_updated_at_str = expense_type.get('updated_at')
                    if not expense_updated_at_str:
                        continue

                    try:
                        expense_updated_at = datetime.strptime(expense_updated_at_str, '%Y-%m-%d %H:%M:%S.%f')
                    except ValueError:
                        try:
                            expense_updated_at = datetime.strptime(expense_updated_at_str, '%Y-%m-%d %H:%M:%S')
                        except ValueError:
                            continue

                    db_updated_at = existing_expense_type.get('updated_at')

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
                        self.expense_types_model.update_expense_type(expense_type)
                else:
                    try:
                        self.expense_types_model.insert_expense_type(expense_type)
                    except Exception as e:
                        print(f"Erro ao inserir Tipo de Despesa {expense_type_id}: {e}")

            print(f"Total de {len(expense_types)} tipos de despesas processadas com sucesso.")

        except Exception as e:
            print(f"Erro ao importar tipo de despesas: {e}")
