import pyodbc
import configparser
import os

class DatabaseConnector:
    def __init__(self, config_file='bd.config'):
        self.config = configparser.ConfigParser()
        self.config.read(os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', config_file))
        self.connection = None


    def connect(self):
        try:
            db_config = self.config['DATABASE']
            self.connection = pyodbc.connect(
                f"DRIVER={db_config['DRIVER']};"
                f"SERVER={db_config['HOST']};"
                f"DATABASE={db_config['DATABASE']};"
                f"UID={db_config['USERNAME']};"
                f"PWD={db_config['PASSWORD']};"
                f"PORT={db_config['PORT']};"
            )
            # print("Conexão bem-sucedida ao Banco de Dados!")
        except pyodbc.Error as e:
            print(f"Erro ao conectar ao Banco de Dados: {e}")
            raise

    
    def get_connection(self):
        if self.connection is None:
            self.connect()
        return self.connection
    

    def close(self):
        if self.connection:
            self.connection.close()
            print("\nConexão ao BD encerrada.")