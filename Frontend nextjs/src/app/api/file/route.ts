// src\app\api\file\route.ts
import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { FileResponse } from "@/types/schemas/file.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function GET(req: Request) {
  const { searchParams } = new URL(req.url);
  const currentModule = searchParams.get("module");
  const pagina = searchParams.get("pagina") ?? "1";
  const tamanhoPagina = searchParams.get("tamanhoPagina") ?? "10";

  const query = new URLSearchParams({
    pagina,
    tamanhoPagina,
  });

  const data = await serverFetch(
    `/api/${currentModule}/file?${query.toString()}`,
    "GET"
  );

  return NextResponse.json(data);
}

export async function POST(req: NextRequest) {
  try {
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json(
        { error: "Module é obrigatório" },
        { status: 400 }
      );
    }

    const formData = await req.formData();
    const file = formData.get("file") as File | null;

    if (!file) {
      return NextResponse.json(
        { error: "Nenhum arquivo enviado." },
        { status: 400 }
      );
    }
    
    const allowedTypes = [
      "application/pdf",
      "application/msword",
      "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
      "application/vnd.ms-excel",
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "text/csv",
      "text/plain",
    ];

    if (!allowedTypes.includes(file.type)) {
      return NextResponse.json(
        {
          error:
            "Formato não permitido. Apenas PDF, DOCX, XLSX, CSV e TXT são aceitos.",
        },
        { status: 400 }
      );
    }

    const form = new FormData();
    form.append("arquivo", file);

    const data = await serverFetch<FileResponse>(
      `/api/${currentModule}/file`,
      "POST",
      form,
      true
    );
    return NextResponse.json(data, { status: 201 });
  } catch (err) {
    return handleApiError(err);
  }
}
