import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { AgentDetail } from "@/types/schemas/agent.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function GET(
  req: NextRequest,
  context: { params: Promise<{ id: string }> }
) {
  try {
    const { id } = await context.params; // precisa do await
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    const data = await serverFetch<AgentDetail>(
      `/api/${currentModule}/agents/${id}`,
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
    const { id } = await context.params;
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    await serverFetch<void>(`/api/${currentModule}/agents/${id}`, "DELETE");

    return NextResponse.json(null, { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}
