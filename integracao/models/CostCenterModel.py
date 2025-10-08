from datetime import datetime

class CostCentersModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def insert_cost_centers(self, cost_center):
        query="""
        INSERT INTO VEXP_CostCenters(id, integration_id, name, company_group_id, costcenter_on)
        VALUES (?, ?, ?, ?, ?)
        """
        values = (
            cost_center['id'],
            cost_center.get('integration_id'),
            cost_center['name'],
            cost_center.get('company_group_id'),
            cost_center.get('on')
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
            #print(f"Centro de Custo{cost_center['id']} inserido com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"Erro ao inserir Centro de Custo {cost_center['id']}: {e}")
        finally:
            cursor.close()


    def get_cost_center_by_id(self, cost_center_id):
        """
        Verifica se o Centro de Custo com o ID fornecido já existe no BD.
        Retorna o Centro de Custo encontrado ou None se não houver correspondência.
        """
        query = "SELECT * FROM VEXP_CostCenters WHERE id = ?"   # Marcador de posição
        cursor = self.connection.cursor()

        try:
            cursor.execute(query, (cost_center_id,))
            result = cursor.fetchone()

            if result:
                # Retornar o Centro de Custo encontrado
                return{
                    'id': result[0]                             # Assumindo que o ID seja o primeiro campo da tabela
                }
            
        except Exception as e:
            print(f"Erro ao consultar Centro de Custo por ID: {e}")
            return None
        
        finally:
            cursor.close()


    def update_cost_center(self, cost_center):
        """
        Atualiza um centro de custo existente no BD, com base no ID.
        """

        columns_to_update = [
            "id = ?",
            "integration_id = ?",
            "name = ?",
            "company_group_id = ?",
            "costcenter_on = ?"
        ]

        values_to_update = [
            cost_center.get('id'),                      # ID do tipo de despesa (INTEGER)
            cost_center.get('integration_id'),          # ID da integração (INTEGER)
            cost_center['name'],                        # Nome do Centro de Custo (VARCHAR(200))
            cost_center.get('company_group_id'),        # ID do cadastro da empresa (INTEGER)
            cost_center['on'],                          # Se o centro de custos está ativo (BOOLEAN, no caso BIT para SQLServer)
        ]

        query = f"""
        UPDATE VEXP_CostCenters
        SET {",".join(columns_to_update)}
        WHERE id = ?
        """

        cursor = self.connection.cursor()

        try:
            # Adiciona o id de Centro de Custo no final dos valores
            values_to_update.append(cost_center['id'])

            # Executar o update com os valores de Centro de Custo
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()
            #print(f"Centro de Custo com ID {cost_center['id']} atualizado com sucesso.")

        except Exception as e:
            print(f"Erro ao atualizar Centro de Custo com ID {cost_center['id']}: {e}")
            self.connection.rollback()

        finally:
            cursor.close()


    def get_all_expenses_ids(self):
        """
        Busca todas as IDs de despesas na tabela VEXP_Expenses.
        Retorna uma lista com os IDs das despesas.
        """
        query = "SELECT id FROM VEXP_Expenses"  # Ajuste o nome da tabela se necessário
        cursor = self.connection.cursor()
        
        try:
            cursor.execute(query)
            result = cursor.fetchall()
            # Retorna uma lista de IDs de despesas
            return [row[0] for row in result] if result else []
        
        except Exception as e:
            print(f"Erro ao buscar todas as IDs de despesas: {e}")
            return []
        
        finally:
            cursor.close()

    def cost_center_exists(self, cost_center_id):
        query = "SELECT 1 FROM VEXP_CostCenters WHERE id = ?"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query, (cost_center_id,))
            return cursor.fetchone() is not None
        except Exception as e:
            print(f"Erro ao verificar existência do Centro de Custo: {e}")
            return False
        finally:
            cursor.close()
