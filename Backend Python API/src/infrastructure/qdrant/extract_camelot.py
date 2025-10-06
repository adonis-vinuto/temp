import os
import io
import tempfile
from typing import List, Dict

import camelot
import pandas as pd

# opcional, mas recomendado p/ fallback de OCR:
import pypdfium2 as pdfium
from PIL import Image
import pytesseract


def _to_text_blocks(tables, page_idx: int) -> List[Dict]:
    """Converte as tabelas do Camelot para a sua saída (text + page_number)."""
    pages: List[Dict] = []
    if not tables or getattr(tables, "n", 0) == 0:
        return pages

    for table in tables:
        df = table.df.copy()

        # Conversões numéricas (mantendo sua lógica)
        # — ignora a primeira coluna (como no seu código original)
        for col in df.columns[1:]:
            if df[col].dtype == "object":
                df[col] = (
                    df[col]
                    .astype(str)
                    .str.replace(".", "", regex=False)   # remove milhar
                    .str.replace(",", ".", regex=False) # vírgula -> ponto
                )
                df[col] = pd.to_numeric(df[col], errors="ignore")

        table_text = df.to_string(index=False)
        pages.append({"text": table_text, "page_number": page_idx + 1})
    return pages


def _camelot_read_page(path: str, page_idx: int, flavor: str):
    """Envolve camelot.read_pdf com parâmetros mais tolerantes e evita ValueError vago."""
    page_str = str(page_idx + 1)

    if flavor == "stream":
        # parâmetros mais robustos para textos desalinhados
        return camelot.read_pdf(
            path,
            pages=page_str,
            flavor="stream",
            edge_tol=500,
            row_tol=10,
            strip_text="\n",
        )
    else:
        # lattice requer ghostscript e opencv
        return camelot.read_pdf(
            path,
            pages=page_str,
            flavor="lattice",
            line_scale=40,
        )


def _page_has_embedded_text(pdf: pdfium.PdfDocument, page_idx: int) -> bool:
    """Verifica rapidamente se a página tem texto embutido (não apenas imagem)."""
    try:
        page = pdf.get_page(page_idx)
        txt = page.get_textpage().get_text_range()
        return bool(txt and txt.strip())
    except Exception:
        return False


def _ocr_single_page_pdf(pdf: pdfium.PdfDocument, page_idx: int) -> bytes:
    """Renderiza a página em imagem e retorna um PDF com texto via Tesseract."""
    page = pdf.get_page(page_idx)
    pil_img = page.render(scale=2.0).to_pil()  # upscale ajuda o OCR
    buf = io.BytesIO()
    pil_img.save(buf, format="PNG")
    buf.seek(0)
    return pytesseract.image_to_pdf_or_hocr(Image.open(buf), extension="pdf")


def extract_camelot(file_content: bytes, filename: str = "temp.pdf") -> List[Dict]:
    """
    Extrai tabelas de um PDF (bytes) com fallbacks:
    1) Camelot stream
    2) Camelot lattice
    3) OCR -> PDF de 1 página -> Camelot novamente
    Retorna lista de dicts {'text': str, 'page_number': int}
    """
    # grava o PDF temporário
    with tempfile.NamedTemporaryFile(delete=False, suffix=".pdf") as tmp_file:
        tmp_file.write(file_content)
        tmp_path = tmp_file.name

    out: List[Dict] = []

    try:
        # tenta abrir com pdfium para iterar páginas; se falhar, tenta como 1 página mesmo
        try:
            pdf = pdfium.PdfDocument(io.BytesIO(file_content))
            total_pages = len(pdf)
        except Exception:
            pdf = None
            total_pages = 1  # fallback best-effort

        for p in range(total_pages):
            # 1) Tenta STREAM
            try:
                tables = _camelot_read_page(tmp_path, p, flavor="stream")
                if tables and getattr(tables, "n", 0) > 0:
                    out.extend(_to_text_blocks(tables, p))
                    continue
            except ValueError:
                # evita o crash "max() arg is an empty sequence"
                pass
            except Exception:
                pass

            # 2) Tenta LATTICE
            try:
                tables = _camelot_read_page(tmp_path, p, flavor="lattice")
                if tables and getattr(tables, "n", 0) > 0:
                    out.extend(_to_text_blocks(tables, p))
                    continue
            except Exception:
                pass

            # 3) Se não achou nada, tenta OCR -> PDF 1 página -> Camelot de novo
            try:
                needs_ocr = True
                if pdf is not None:
                    needs_ocr = not _page_has_embedded_text(pdf, p)

                if needs_ocr and pdf is not None:
                    ocr_pdf_bytes = _ocr_single_page_pdf(pdf, p)
                    with tempfile.NamedTemporaryFile(delete=False, suffix=f".p{p+1}.pdf") as ocr_tmp:
                        ocr_tmp.write(ocr_pdf_bytes)
                        ocr_path = ocr_tmp.name

                    try:
                        # tenta stream no PDF com OCR
                        try:
                            tables = camelot.read_pdf(
                                ocr_path,
                                pages="1",
                                flavor="stream",
                                edge_tol=500,
                                row_tol=10,
                                strip_text="\n",
                            )
                            if tables and getattr(tables, "n", 0) > 0:
                                out.extend(_to_text_blocks(tables, p))
                                continue
                        except Exception:
                            pass

                        # tenta lattice no PDF com OCR
                        try:
                            tables = camelot.read_pdf(
                                ocr_path,
                                pages="1",
                                flavor="lattice",
                                line_scale=40,
                            )
                            if tables and getattr(tables, "n", 0) > 0:
                                out.extend(_to_text_blocks(tables, p))
                                continue
                        except Exception:
                            pass
                    finally:
                        try:
                            os.remove(ocr_path)
                        except Exception:
                            pass
            except Exception:
                # não deixa uma página ruim quebrar tudo
                pass

        return out

    finally:
        # limpa o PDF temporário
        try:
            if os.path.exists(tmp_path):
                os.remove(tmp_path)
        except Exception:
            pass
