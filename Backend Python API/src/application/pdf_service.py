"""Simple PDF text extraction utilities.

This module provides a single function :func:`extract_text_from_pdf_bytes` that
attempts to read embedded text from each page of a PDF using ``pypdfium2``.  If a
page has no embedded text (commonly the case for scanned documents), the page is
rendered to an image and processed with ``pytesseract`` for optical character
recognition.
"""

from __future__ import annotations

import io
from typing import List, Tuple, Dict, Any

import pypdfium2 as pdfium
import pytesseract

from src.domain.extract_text_schema import FileSchema, PageSchema

from ..infrastructure.qdrant.text_refiner import text_resume, text_file_name
from ..infrastructure.simple_usage_callback import SimpleUsageCallback

def _extract_file_type(filename: str) -> str:
        if '.' in filename:
            return filename.split('.')[-1].lower()
    
        return "unknown"

def extract_text_from_pdf_bytes(pdf_bytes: bytes, filename: str) -> Tuple[FileSchema, Dict[str, Any]]:
    """Extract text from PDF bytes.

    The function first attempts to extract embedded text using ``pypdfium2``. If
    no text is found on a page (e.g., when the PDF is a scanned image), it falls
    back to optical character recognition with ``pytesseract``.

    Args:
        pdf_bytes: Raw bytes of a PDF file.

    Returns:
        Tuple with FileSchema and usage dict.
    """

    file_type = _extract_file_type(filename)
    usage_callback = SimpleUsageCallback()

    pdf = pdfium.PdfDocument(io.BytesIO(pdf_bytes))
    raw_pages: List[dict] = []
    # Helper: try OCR with a sequence of language fallbacks and handle errors
    def _ocr_with_fallback(pil_image) -> str:
        # Preferred languages to try (Portuguese then English, then default)
        candidates = ["por", "eng", None]
        last_exc: Exception | None = None
        for lang in candidates:
            try:
                if lang:
                    text = pytesseract.image_to_string(
                        pil_image, lang=lang, config="--oem 3 --psm 6"
                    )
                else:
                    # No language specified (let tesseract choose default)
                    text = pytesseract.image_to_string(pil_image, config="--oem 3 --psm 6")
                return text.strip()
            except pytesseract.pytesseract.TesseractError as e:
                last_exc = e
                continue
        if last_exc:
            raise last_exc
        return ""

    for i, page in enumerate(pdf, start=1):
        text_page = page.get_textpage()
        page_text = text_page.get_text_range().strip()
        if not page_text:
            pil_image = page.render(scale=300 / 72).to_pil()
            page_text = _ocr_with_fallback(pil_image)
        cleaned = " ".join(page_text.split())
        raw_pages.append({"text": cleaned, "page": i})

    # Build PageSchema objects from the raw dicts (do not append into the same list)
    page_schemas = [
        PageSchema(text=p["text"], page_number=p["page"])
        for p in raw_pages
    ]

    resume = text_resume(page_schemas, usage_callback, tamanho_maximo=1000)

    filename_res = text_file_name(page_schemas, usage_callback, tamanho_maximo=100)

    file_schema = FileSchema(
            file_name=filename_res,
            file_type=file_type,
            resume=resume,
            pages=page_schemas
        )

    return file_schema, usage_callback.get_usage_dict()

