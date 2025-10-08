from datetime import datetime
from controllers.ApiService import ApiService
from controllers.ReportObjectController import ReportObjectController
from models.ReportModel import ReportModel
from models.ExpenseTypeModel import ExpenseTypesModel
from models.CurrencyModel import CurrenciesModel
from models.ExpenseModel import ExpenseModel
from models.ApportionmentModel import ApportionmentModel

class ReportController:
    def __init__(self, api_service, db_connector):
        self.api_service = api_service
        self.report_model = ReportModel(db_connector)
        self.expense_type_model = ExpenseTypesModel(db_connector)
        self.currency_model = CurrenciesModel(db_connector)
        self.expense_model = ExpenseModel(db_connector)
        self.apportionment_model = ApportionmentModel(db_connector)
        self.report_object_controller = ReportObjectController(api_service, db_connector)
        print("\nRELATÓRIOS")

    def import_reports(self):
        """
        Busca as despesas via API, utilizando a última data de atualização registrada no BD
        como ponto de partida, e insere ou atualiza as despesas no banco de dados.
        """
        try:
            #reports = self.api_service.get_reports()

            # Consultar a última data de atualização
            start_date = self.get_last_updated_date()
            start_date = '2025-09-01'
            # Definir a data atual como end_date
            # end_date = '2024-12-31'
            end_date = datetime.now().strftime('%Y-%m-%d')  # Obter a data atual no formato YYYY-MM-DD
            print(f"Buscando os relatórios entre {start_date} e {end_date}...")

            # Chamar a API para buscar os relatórios nesse intervalo
            reports = self.api_service.get_reports(start_date, end_date)

            if not reports:
                print("Nenhuma despesa encontrada.")
                return

            for report in reports:
                report_id = report['id']    # Assumindo que o ID do report é 'id'

                # Verificar se o ID já existe no banco de dados
                existing_report = self.report_model.get_report_by_id(report_id)


                if existing_report:
                    # Comparar a data atual com a última data de atualização (updated_at)
                    if existing_report['updated_at'] != None and datetime.now() > existing_report['updated_at']:
                        # Fazer o update se a data atual for maior que a última data de atualização
                        #print(f"Atualizando Relatório com ID {report_id}...")
                        self.report_model.update_report(report)
                else:
                    # Inserir um novo relatório
                    try:
                        #print(f"Inserindo novo Relatório com ID {report_id}...")
                        self.report_model.insert_report(report)
                    except Exception as e:
                        print(f"Erro ao inserir Relatório {report_id}: {e}")

                self.report_object_controller.get_report_with_expenses(report_id)

            print(f"Total de {len(reports)} relatórios processados com sucesso.")

        except Exception as e:
            print(f"Erro ao importar relatórios: {e}")


    def get_last_updated_date(self):
        """
        Consulta no banco de dados a última data registrada na coluna updated_at.
        Retorna a última data encontrada ou '2020-01-01' como valor padrão.
        """
        try:
            print("Consultando a última data de atualização no banco de dados...")
            last_updated_date = self.report_model.get_last_updated_date()

            if last_updated_date:
                print(f"Última data de atualização encontrada: {last_updated_date}")
                return last_updated_date
            else:
                # Se não houver dados, retornar a data padrão de início
                print("Nenhuma data encontrada, usando data padrão '2020-01-01'.")
                return '2024-10-01'

        except Exception as e:
            print(f"Erro ao consultar a última data de atualização: {e}")
            # Em caso de erro, também retornar a data padrão
            return '2024-10-01'
