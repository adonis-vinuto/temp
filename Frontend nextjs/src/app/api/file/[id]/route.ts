// src/app/api/file/[id]/route.ts
import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import type { FileWithRelations } from "@/types/schemas/file.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function GET(
  req: NextRequest,
  context: { params: Promise<{ id: string }> }
) {
  try {
    const { id } = await context.params;
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    const data = await serverFetch<FileWithRelations>(
      `/api/${currentModule}/file/${id}`,
      "GET"
    );

    return NextResponse.json(data, { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}

export async function DELETE(
  req: NextRequest,
  context: { params: Promise<{ id: string }> }
) {
  try {
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }
    const { id } = await context.params;
    await serverFetch<void>(`/api/${currentModule}/file/${id}`, "DELETE");
    return NextResponse.json(null, { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}
