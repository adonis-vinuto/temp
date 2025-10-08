import requests
import time

class ApiService:
    def __init__(self, api_token):
        self.base_url = 'https://api.vexpenses.com/v2/'
        self.api_token = api_token
        self.headers = {
            'Accept': 'application/json',
            'Authorization': f'{self.api_token}'  # Formato correto do cabeçalho Authorization
        }


    def rate_limited_request(self, method, url, **kwargs):
        """Limitar requisições a 500 a cada 3 minutos"""
        max_requests = 500
        interval_seconds = 180  # 3 minutos

        for _ in range(max_requests):
            response = method(url, headers=self.headers, **kwargs)
            if response.status_code == 200:
                return response
            elif response.status_code == 401:
                raise Exception("Erro de autenticação: Verifique o token da API.")
            elif response.status_code == 429:  # Código de status 429: Too Many Requests
                print("Limite de requisições atingido. Aguardando 3 minutos...")
                time.sleep(interval_seconds)
            else:
                print(f"Erro: {response.status_code} - {response.text}")
                time.sleep(interval_seconds)

        raise Exception("Limite de requisições excedido. Tente novamente após 3 minutos.")


    def get_team_members(self):
        url = f'{self.base_url}team-members'
        response = requests.get(url, headers=self.headers)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar membros da equipe: {response.status_code} - {response.text}')

        return response.json().get('data', [])


    def get_reports(self, start_date, end_date):
        """
        Busca relatórios da API.
        Parâmetros opcionais podem ser passados via kwargs (ex: filtros como status, user_id, etc).
        """
        url = f'{self.base_url}reports'
        #response = self.rate_limited_request(requests.get, url, params=kwargs)

        params = {
            'search': f'created_at:{start_date},{end_date}',    # Filtrar despesas entre as datas
            'searchFields': 'created_at:between',
            'searchJoin': 'and'
        }

        response = requests.get(url, headers=self.headers, params=params)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")
        #print(f"Parâmetros: {params}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar relatórios: {response.status_code} - {response.text}')

        return response.json().get('data', [])
    

    def get_expenses(self, start_date, end_date):
        """
        Busca as despesas com filtro de data entre start_date e end_date.
        """
        url = f'{self.base_url}expenses'
        params = {
            'search': f'created_at:{start_date},{end_date}',  # Filtrar despesas entre as datas
            'searchFields': 'created_at:between',
            'searchJoin': 'and'
        }

        response = requests.get(url, headers=self.headers, params=params)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")
        #print(f"Parâmetros: {params}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar despesas: {response.status_code} - {response.text}')

        return response.json().get('data', [])
    
    
    def get_expense_types(self, start_date, end_date):
        """
        Busca os tipos de despesas com filtro de data entre start_date e end_date.
        """
        url = f'{self.base_url}expenses-type'
        params = {
            'search': f'created_at:{start_date},{end_date}',  # Filtrar despesas entre as datas
            'searchFields': 'created_at:between',
            'searchJoin': 'and'
        }

        response = requests.get(url, headers=self.headers, params=params)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")
        #print(f"Parâmetros: {params}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar despesas: {response.status_code} - {response.text}')

        return response.json().get('data', [])
        

    def get_cost_centers(self):
        """
        Busca os Centros de Custos cadastrados
        """
        url = f'{self.base_url}costs-centers'
        response = requests.get(url, headers=self.headers)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar membros da equipe: {response.status_code} - {response.text}')
        
        return response.json().get('data', [])


    def get_projects(self):
        """
        Busca os Projetos cadastrados
        """
        url = f'{self.base_url}projects'
        response = requests.get(url, headers=self.headers)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar membros da equipe: {response.status_code} - {response.text}')
        
        return response.json().get('data', [])


    def get_approval_flows(self):
        """
        Busca os Fluxos de Aprovação cadastrados
        """
        url = f'{self.base_url}approval-flows'
        response = requests.get(url, headers=self.headers)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar membros da equipe: {response.status_code} - {response.text}')
        
        return response.json().get('data', [])


    def get_currencies(self):
        """
        Busca as Moedas cadastradas
        """
        url = f'{self.base_url}currencies'
        response = requests.get(url, headers=self.headers)
        #print(f"URL: {url}")
        #print(f"Headers: {self.headers}")

        if response.status_code == 401:
            raise Exception("Erro de autenticação: Verifique o token da API.")
        elif response.status_code != 200:
            raise Exception(f'Erro ao buscar membros da equipe: {response.status_code} - {response.text}')
        
        return response.json().get('data', [])


    def get_report_with_expenses(self, report_id):
        """Obtém um relatório específico, incluindo as despesas associadas"""
        url = f"{self.base_url}reports/{report_id}?include=expenses"
        
        # Faz a requisição GET usando a função de requisição com limitação
        response = self.rate_limited_request(requests.get, url)
        
        # Trata a resposta
        if response.status_code == 200:
            data = response.json()
            return data  # Retorna o conteúdo JSON do relatório e das despesas
        else:
            raise Exception(f"Erro ao obter o relatório: {response.status_code} - {response.text}")


    def get_expense_details(self, expense_id, include=None):
        """
        Obtém os detalhes de uma despesa específica usando o ID da despesa.
        Parâmetros:
            - expense_id (str): ID da despesa a ser consultada.
            - include (str): Nome dos objetos associados que deseja incluir na resposta (ex: 'costs_center').
        """
        # Monta a URL com o parâmetro `include` se fornecido
        url = f"{self.base_url}expenses/{expense_id}"
        if include:
            url += f"?include={include}"
        
        # Faz a requisição GET usando a função de requisição com limitação
        response = self.rate_limited_request(requests.get, url)
        
        # Trata a resposta
        if response.status_code == 200:
            return response.json()  # Retorna o conteúdo JSON com os detalhes da despesa
        else:
            raise Exception(f"Erro ao obter detalhes da despesa: {response.status_code} - {response.text}")
