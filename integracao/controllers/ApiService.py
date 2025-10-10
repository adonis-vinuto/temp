# ApiService.py
import requests
import time
import os
import sys
import json
from datetime import datetime
from typing import Any, Dict, List, Optional, Union

JSON = Union[Dict[str, Any], List[Any]]

class ApiService:
    """
    Serviço de integração com a API VExpenses.

    Para cada chamada:
      - Cria um snapshot JSON (timestampado) em logs/api/<tipo>/<tipo>_YYYYmmdd_HHMMSS.json
      - Acumula em logs/api/<tipo>.jsonl (um registro por linha), "por tipo"

    Ex.: tipo = "expenses", "reports", "team_members", etc.
    """

    def __init__(self, api_token: str):
        self.base_url = 'https://api.vexpenses.com/v2/'
        self.api_token = api_token
        self.headers = {
            'Accept': 'application/json',
            'Authorization': f'{self.api_token}'  # conforme seu backend exige
        }

    # =======================
    # Infra: paths & arquivos
    # =======================
    def _base_dir(self) -> str:
        """Retorna o diretório base, compatível com PyInstaller (.exe) e dev."""
        if getattr(sys, 'frozen', False) and hasattr(sys, '_MEIPASS'):
            return os.path.dirname(sys.executable)
        return os.path.dirname(os.path.abspath(__file__))

    def _ensure_dir(self, *parts: str) -> str:
        path = os.path.join(self._base_dir(), *parts)
        os.makedirs(path, exist_ok=True)
        return path

    def _ts(self) -> str:
        return datetime.now().strftime("%Y%m%d_%H%M%S")

    def _snapshot_path(self, tipo: str) -> str:
        """Caminho do arquivo snapshot JSON (timestampado)."""
        subdir = self._ensure_dir("logs", "api", tipo)
        return os.path.join(subdir, f"{tipo}_{self._ts()}.json")

    def _rolling_jsonl_path(self, tipo: str) -> str:
        """Caminho do arquivo acumulado por tipo (jsonl)."""
        out_dir = self._ensure_dir("logs", "api")
        return os.path.join(out_dir, f"{tipo}.jsonl")

    def _error_snapshot_path(self, tipo: str) -> str:
        subdir = self._ensure_dir("logs", "api", "errors", tipo)
        return os.path.join(subdir, f"{tipo}_ERROR_{self._ts()}.json")

    def _dump_snapshot(self, tipo: str, payload: JSON, meta: Optional[Dict[str, Any]] = None) -> str:
        """Grava snapshot JSON (um arquivo por chamada)."""
        envelope = {
            "fetched_at": datetime.now().isoformat(),
            "type": tipo,
            "meta": meta or {},
            "payload": payload
        }
        path = self._snapshot_path(tipo)
        with open(path, "w", encoding="utf-8") as f:
            json.dump(envelope, f, ensure_ascii=False, indent=2)
        return path

    def _append_jsonl(self, tipo: str, items: JSON, meta: Optional[Dict[str, Any]] = None) -> str:
        """
        Acrescenta ao arquivo acumulado .jsonl do tipo.
        - Se 'items' for lista: grava 1 linha por item
        - Se 'items' for dict (objeto único): grava 1 linha
        """
        path = self._rolling_jsonl_path(tipo)
        meta = meta or {}
        now_iso = datetime.now().isoformat()

        def _write_line(obj: Dict[str, Any]):
            line = {
                "fetched_at": now_iso,
                "type": tipo,
                "meta": meta,
                "data": obj
            }
            with open(path, "a", encoding="utf-8") as f:
                f.write(json.dumps(line, ensure_ascii=False) + "\n")

        if isinstance(items, list):
            for it in items:
                _write_line(it if isinstance(it, dict) else {"value": it})
        elif isinstance(items, dict):
            _write_line(items)
        else:
            # qualquer outra coisa serializa dentro de "value"
            _write_line({"value": items})
        return path

    def _dump_error(self, tipo: str, status_code: int, text: str, meta: Optional[Dict[str, Any]] = None) -> str:
        payload = {
            "error": True,
            "status_code": status_code,
            "text": text,
        }
        envelope = {
            "fetched_at": datetime.now().isoformat(),
            "type": tipo,
            "meta": meta or {},
            "payload": payload
        }
        path = self._error_snapshot_path(tipo)
        with open(path, "w", encoding="utf-8") as f:
            json.dump(envelope, f, ensure_ascii=False, indent=2)
        # também loga no acumulado geral de erros:
        self._append_jsonl("errors", {"type": tipo, "status_code": status_code, "text": text}, meta)
        return path

    # =======================
    # Rate limiting
    # =======================
    def rate_limited_request(self, method, url, **kwargs) -> requests.Response:
        """Limitar requisições a 500 a cada 3 minutos."""
        max_requests = 500
        interval_seconds = 180  # 3 minutos

        for _ in range(max_requests):
            response = method(url, headers=self.headers, **kwargs)
            if response.status_code == 200:
                return response
            elif response.status_code == 401:
                raise Exception("Erro de autenticação: Verifique o token da API.")
            elif response.status_code == 429:  # Too Many Requests
                print("Limite de requisições atingido. Aguardando 3 minutos...")
                time.sleep(interval_seconds)
            else:
                print(f"Erro: {response.status_code} - {response.text}")
                time.sleep(interval_seconds)

        raise Exception("Limite de requisições excedido. Tente novamente após 3 minutos.")

    # =======================
    # Helpers de request GET
    # =======================
    def _get_and_record(
        self,
        tipo: str,
        url: str,
        *,
        params: Optional[Dict[str, Any]] = None,
        include_rate_limit: bool = False,
        meta: Optional[Dict[str, Any]] = None,
        dump: bool = True,
    ) -> List[Dict[str, Any]]:
        """
        Executa GET, valida e grava snapshot + jsonl (se dump=True).
        Retorna lista de dicts (data) quando presente; se a API devolver um objeto único,
        retorna [obj] para manter consistência.
        """
        try:
            if include_rate_limit:
                resp = self.rate_limited_request(requests.get, url, params=params)
            else:
                resp = requests.get(url, headers=self.headers, params=params)

            if resp.status_code == 401:
                # também salva o erro
                if dump:
                    self._dump_error(tipo, resp.status_code, resp.text, meta=meta)
                raise Exception("Erro de autenticação: Verifique o token da API.")
            if resp.status_code != 200:
                if dump:
                    self._dump_error(tipo, resp.status_code, resp.text, meta=meta)
                raise Exception(f"Erro na chamada {tipo}: {resp.status_code} - {resp.text}")

            body = resp.json()

            # Muitas rotas usam {"data": [...]} – normalizamos:
            data = body.get("data") if isinstance(body, dict) else None
            if data is None:
                # se não há "data", tratamos o corpo inteiro como um objeto
                data = body

            # Snapshot e Acúmulo
            if dump:
                # meta default inclui URL e params para auditoria
                full_meta = {"url": url, "params": params or {}}
                if meta:
                    full_meta.update(meta)

                # snapshot
                self._dump_snapshot(tipo, data, meta=full_meta)

                # acumula
                self._append_jsonl(tipo, data, meta=full_meta)

            # retorno consistente (lista)
            if isinstance(data, list):
                return data
            else:
                return [data]  # objeto único vira lista com 1 item

        except Exception as e:
            # Falha geral de rede/parse/etc -> registra erro (se possível)
            if dump:
                self._dump_error(tipo, -1, str(e), meta={"url": url, "params": params or {}, **(meta or {})})
            raise

    # =======================
    # Endpoints
    # =======================
    def get_team_members(self, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}team-members"
        return self._get_and_record("team_members", url, dump=dump)

    def get_reports(self, start_date: str, end_date: str, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}reports"
        params = {
            'search': f'created_at:{start_date},{end_date}',
            'searchFields': 'created_at:between',
            'searchJoin': 'and'
        }
        return self._get_and_record("reports", url, params=params, dump=dump, meta={"start_date": start_date, "end_date": end_date})

    def get_expenses(self, start_date: str, end_date: str, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}expenses"
        params = {
            'search': f'created_at:{start_date},{end_date}',
            'searchFields': 'created_at:between',
            'searchJoin': 'and'
        }
        return self._get_and_record("expenses", url, params=params, dump=dump, meta={"start_date": start_date, "end_date": end_date})

    def get_expense_types(self, start_date: str, end_date: str, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}expenses-type"
        params = {
            'search': f'created_at:{start_date},{end_date}',
            'searchFields': 'created_at:between',
            'searchJoin': 'and'
        }
        return self._get_and_record("expense_types", url, params=params, dump=dump, meta={"start_date": start_date, "end_date": end_date})

    def get_cost_centers(self, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}costs-centers"
        return self._get_and_record("cost_centers", url, dump=dump)

    def get_projects(self, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}projects"
        return self._get_and_record("projects", url, dump=dump)

    def get_approval_flows(self, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}approval-flows"
        return self._get_and_record("approval_flows", url, dump=dump)

    def get_currencies(self, *, dump: bool = True) -> List[Dict[str, Any]]:
        url = f"{self.base_url}currencies"
        return self._get_and_record("currencies", url, dump=dump)

    def get_report_with_expenses(self, report_id: str, *, dump: bool = True) -> List[Dict[str, Any]]:
        """
        Obtém um relatório específico, incluindo despesas associadas.
        Retorna lista com 1 item (o relatório) por consistência.
        """
        url = f"{self.base_url}reports/{report_id}?include=expenses"
        # usa rate limit por segurança nesse endpoint
        return self._get_and_record(
            "report_with_expenses",
            url,
            include_rate_limit=True,
            dump=dump,
            meta={"report_id": report_id}
        )

    def get_expense_details(self, expense_id: str, include: Optional[str] = None, *, dump: bool = True) -> List[Dict[str, Any]]:
        """
        Obtém detalhes de uma despesa.
        Retorna lista com 1 item (o objeto de detalhe) por consistência.
        """
        url = f"{self.base_url}expenses/{expense_id}"
        if include:
            url += f"?include={include}"
        return self._get_and_record(
            "expense_details",
            url,
            include_rate_limit=True,
            dump=dump,
            meta={"expense_id": expense_id, "include": include}
        )
