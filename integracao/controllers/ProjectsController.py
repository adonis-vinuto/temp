from controllers.ApiService import ApiService
from models.ProjectModel import ProjectsModel

class ProjectsController:
    def __init__(self, api_service, dbconnector):
        self.api_service = api_service
        self.project_model = ProjectsModel(dbconnector)
        print("\nPROJETOS")

    def import_projects(self):
        """
        Busca os Projetos via API e insere ou atualiza os cadastros no BD.
        """

        try:
            # Chamada de API p/ buscar os Projetos
            projects = self.api_service.get_projects()

            #print("Projetos recebidos: ", projects)    # Imprimir os Projetos recebidos

            if not projects:
                print("Nenhum Projeto recebido.")
                return
            
            # Inserir ou atualizar os Projetos no BD
            for project in projects:
                project_id = project['id']              # Assumindo que o ID de Projeto é a coluna 'id'

                # Verificar se o id já existe no BD
                existing_project = self.project_model.get_project_by_id(project_id)

                if existing_project:
                    #print(f"Atualizando Projeto com ID {project_id}...")
                    self.project_model.update_project(project)
                else:
                    # Inserindo novo Projeto
                    try:
                        #print(f"Inserindo novo Projeto com ID {project_id}...")
                        self.project_model.insert_projects(project)
                    except Exception as e:
                        print(f"Erro ao inserir Projeto {projects}: {e}")

            print(f"Total de {len(projects)} Projetos processados com sucesso.")

        except Exception as e:
            print(f"Erro ao importar Projetos: {e}")