// src\app\api\file\[id]\attach\[agentId]\route.ts
import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function DELETE(
  req: NextRequest,
  context: { params: Promise<{ id: string; agentId: string }> }
) {
  try {
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");

    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    const { id, agentId } = await context.params;

    await serverFetch<void>(
      `/api/${currentModule}/file/${id}/attach/${agentId}`,
      "DELETE"
    );

    return NextResponse.json(null, { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}
