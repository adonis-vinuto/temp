from controllers.ApiService import ApiService
from models.CostCenterModel import CostCentersModel
from controllers.ExpenseObjectController import ExpenseObjectController

class CostsCenterController:
    def __init__(self, api_service, db_connector):
        self.api_service = api_service
        self.cost_center_model = CostCentersModel(db_connector)
        self.expense_object_controller = ExpenseObjectController(api_service, db_connector)
        print("\nCENTROS DE CUSTO")

    def import_cost_centers(self):
        '''
        Busca os Centros de Custo via API e insere ou atualiza os cadastros no BD.
        '''
        try:
            # Chamada de API p/ buscar os Centros de Custo
            cost_centers = self.api_service.get_cost_centers()

            if not cost_centers:
                print("Nenhum Centro de Custo recebido.")
                return
            
            # Inserir ou atualizar os Centros de Custo no BD
            for cost_center in cost_centers:
                cost_center_id = cost_center['id']  # Assumindo que o ID de Centro de Custo é 'id'

                # Verificar se o id já existe no BD
                existing_cost_center = self.cost_center_model.get_cost_center_by_id(cost_center_id)

                if existing_cost_center:
                    self.cost_center_model.update_cost_center(cost_center)
                else:
                    try:
                        self.cost_center_model.insert_cost_centers(cost_center)
                    except Exception as e:
                        print(f"Erro ao inserir Centro de Custo {cost_center_id}: {e}")

            print(f"Total de {len(cost_centers)} Centros de Custo processados com sucesso.")
        
        except Exception as e:
            print(f"Erro ao importar Centros de Custo: {e}")
    

    # Nova função integrada com ExpenseObjectController
    def import_cost_centers_from_expenses(self):
        """
        Busca as despesas existentes no banco de dados e os Centros de Custo associados a essas despesas.
        Insere ou atualiza os Centros de Custo no BD.
        """
        try:
            print("\n🔄 Iniciando o processamento de Centros de Custo a partir das despesas...")

            # Buscar todas as despesas existentes no banco de dados
            expense_ids = self.expense_object_controller.get_all_expenses_ids()
            
            print(f"Total de despesas encontradas: {len(expense_ids)}")


            if not expense_ids:
                print("Nenhuma despesa encontrada para processar.")
                return

            # Processa os Centros de Custo das despesas usando ExpenseObjectController
            self.expense_object_controller.import_costs_center_from_expenses(expense_ids)
            self.expense_object_controller.import_reports_from_expenses(expense_ids)

            print("✅ Processamento de Centros de Custo concluído com sucesso.")
        
        except Exception as e:
            print(f"❌ Erro ao importar Centros de Custo das despesas: {e}")
