from controllers.ApiService import ApiService
from models.ApprovalFlowModel import ApprovalFlowsModel

class ApprovalFlowsController:
    def __init__(self, api_service, dbconnector):
        self.api_service = api_service
        self.approval_flow_model = ApprovalFlowsModel(dbconnector)
        print("\nFLUXOS DE APROVAÇÃO")

    def import_approval_flows(self):
        """
        Busca os Fluxos de Aprovação via API e insere ou atualiza os cadastros no BD.
        """

        try:
            # Chamada de API p/ buscar os Fluxos de Aprovação
            approval_flows = self.api_service.get_approval_flows()

            #print("Fluxos d Aprovação recebidos: ", approval_flows)    # Imprimir os Fluxos de Aprovação recebidos

            if not approval_flows:
                print("Nenhum Fluxo de Aprovação recebido.")
                return
            
            # Inserir ou atualizar Fluxos de Aprovação no BD
            for approval_flow in approval_flows:
                approval_flow_id = approval_flow['id']                  # Assumindo que o ID de Fluxo de Aprovação é 'id'

                # Verificar se o id já existe no BD
                existing_approval_flow = self.approval_flow_model.get_approval_flow_by_id(approval_flow_id)

                if existing_approval_flow:
                    #print(f"Atualizando Fluxo de Aprovação com ID {approval_flow_id}...")
                    self.approval_flow_model.update_approval_flows(approval_flow)
                else:
                    # Inserindo novo Fluxo de Aprovação
                    try:
                        #print(f"Inserindo novo Fluxo de Aprovação com ID {approval_flow_id}...")
                        self.approval_flow_model.insert_approval_flows(approval_flow)
                    except Exception as e:
                        print(f"Erro ao inserir Fluxo de Aprovação {approval_flow_id}: {e}")

            print(f"Total de {len(approval_flows)} Fluxos de Aprovação processados com sucesso.")

        except Exception as e:
            print(f"Erro ao importar Fluxos de Aprovação: {e}")
