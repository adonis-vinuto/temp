// src\app\api\file\[id]\attach\route.ts
import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function PUT(
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
    const body = await req.json();
    await serverFetch<void>(`/api/${currentModule}/file/${id}/attach`, "PUT", body);
    return NextResponse.json(null, { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}
