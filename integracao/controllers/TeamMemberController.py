from controllers.ApiService import ApiService
from models.TeamMemberModel import TeamMemberModel

class TeamMemberController:
    def __init__(self, api_service, dbconnector, dbconnector2):
        self.api_service = api_service
        self.team_member_model = TeamMemberModel(dbconnector,dbconnector2)
        print("\nMEMBROS DE EQUIPE")

    def import_team_members(self):
        """
        Busca os membros de equipe via API e insere ou atualiza os cadastros no BD.
        """

        try:
            # Chamada de API p/ buscar os membros de equipe
            team_members = self.api_service.get_team_members()

            #print("Membros de equipe recebidos: ", team_members)   # Imprimir os membros de equipe recebidos

            if not team_members:
                print("Nenhum Membro de Equipe recebido.")
                return
            
            # Inserir ou atualizar os membros de equipe no BD
            for team_member in team_members:
                team_member_id = team_member['id']  # Assumindo que o ID de membro da equipe é 'id'

                # Verificar se o id já existe no BD
                existing_team_member = self.team_member_model.get_team_member_by_id(team_member_id)

                if existing_team_member:
                    #print(f"Atualizando Membro de Equipe com ID {team_member_id}...")
                    self.team_member_model.update_team_member(team_member)
                else:
                    # Inserindo novo Membro de Equipe
                    try:
                        #print(f"Inserindo novo Membro de Equipe com ID {team_member_id}...")
                        self.team_member_model.insert_team_member(team_member)
                    except Exception as e:
                        print(f"Erro ao inserir Membro de Equipe {team_member_id}: {e}")
            
            print(f"Total de {len(team_members)} membros de equipe processados com sucesso.")
            
        except Exception as e:
            print(f"Erro ao importar membros da equipe: {e}")