# models/data_extractor.py
from controllers.DatabaseConnector import DatabaseConnector

class DataExtractor:
    def __init__(self, config_file='bd.config'):
        self.connector = DatabaseConnector(config_file)
        print("✅ Conectado ao BD Origem")

    def fetch_data(self):
        """
        Executa a query SELECT * FROM vw_integracao e retorna os resultados.
        Apenas registros onde [Centro de Custo] não é nulo.
        """
        query = """
        SELECT * 
        FROM [vw_integracao] 
        WHERE [Centro de Custo] IS NOT NULL
        """
        conn = None
        cursor = None
        try:
            conn = self.connector.get_connection()
            cursor = conn.cursor()
            cursor.execute(query)

            columns = [column[0] for column in cursor.description]
            results = [dict(zip(columns, row)) for row in cursor.fetchall()]

            print(f"📊 Dados extraídos: {len(results)} registros.")
            if results:
                print(f"🔎 Exemplo primeiro registro: {results[0]}")
            return results

        except Exception as e:
            print(f"❌ Erro ao extrair dados: {e}")
            return []

        finally:
            if cursor:
                cursor.close()
