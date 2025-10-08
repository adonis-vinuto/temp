import configparser
import os

class Config:
    def __init__(self, config_file_path):
        self.config_file_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), config_file_path)

        self.config = configparser.ConfigParser()
        self.config.read(self.config_file_path)

        if not self.config.sections():
            raise FileNotFoundError(f"O arquivo de configuração {self.config_file_path} não foi encontrado ou está vazio.")

    def get_api_token(self):
        return self.config['API']['token']
