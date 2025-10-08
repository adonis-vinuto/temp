from datetime import datetime

class ProjectsModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def insert_projects(self, project):
        query = """
        INSERT INTO VEXP_Projects (
            id, name, company_name, cnpj, address, neighborhood, city, state_uf, 
            zip_code, phone1, phone2, project_on
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        values = (
            project.get('id') or None,
            project.get('name') or None,
            project.get('company_name') or None,
            project.get('cnpj') or None,
            project.get('address') or None,
            project.get('neighborhood') or None,
            project.get('city') or None,
            project.get('state_uf') or None,
            project.get('zip_code') or None,
            project.get('phone1') or None,
            project.get('phone2') or None,
            int(project.get('project_on', 0))  # garante BIT no SQLServer
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
            # print(f"✅ Projeto {project.get('id')} inserido com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao inserir Projeto {project.get('id')}: {e}")
        finally:
            cursor.close()

    def get_project_by_id(self, project_id):
        query = "SELECT * FROM VEXP_Projects WHERE id = ?"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query, (project_id,))
            result = cursor.fetchone()
            if result:
                return {
                    'id': result[0],
                    'name': result[1],
                    'company_name': result[2],
                    'cnpj': result[3],
                    'address': result[4],
                    'neighborhood': result[5],
                    'city': result[6],
                    'state_uf': result[7],
                    'zip_code': result[8],
                    'phone1': result[9],
                    'phone2': result[10],
                    'project_on': result[11]
                }
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar Projeto {project_id}: {e}")
            return None
        finally:
            cursor.close()

    def update_project(self, project):
        columns_to_update = [
            "name = ?",
            "company_name = ?",
            "cnpj = ?",
            "address = ?",
            "neighborhood = ?",
            "city = ?",
            "state_uf = ?",
            "zip_code = ?",
            "phone1 = ?",
            "phone2 = ?",
            "project_on = ?"
        ]

        values_to_update = [
            project.get('name') or None,
            project.get('company_name') or None,
            project.get('cnpj') or None,
            project.get('address') or None,
            project.get('neighborhood') or None,
            project.get('city') or None,
            project.get('state_uf') or None,
            project.get('zip_code') or None,
            project.get('phone1') or None,
            project.get('phone2') or None,
            int(project.get('project_on', 0))
        ]

        query = f"""
        UPDATE VEXP_Projects
        SET {", ".join(columns_to_update)}
        WHERE id = ?
        """

        cursor = self.connection.cursor()
        try:
            values_to_update.append(project.get('id'))
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()
            # print(f"✅ Projeto {project.get('id')} atualizado com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao atualizar Projeto {project.get('id')}: {e}")
        finally:
            cursor.close()
