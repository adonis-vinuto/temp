import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { Agent, AgentSchema } from "@/types/schemas/agent.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function GET(req: NextRequest) {
  try {
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    const pagina = searchParams.get("pagina") ?? "1";
    const tamanhoPagina = searchParams.get("tamanhoPagina") ?? "10";

    const query = new URLSearchParams({
      pagina,
      tamanhoPagina,
    });

    const data = await serverFetch(
      `/api/${currentModule}/agents?${query.toString()}`,
      "GET"
    );

    return NextResponse.json(data, { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}

export async function POST(req: NextRequest) {
  try {
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    const body = await req.json();
    const parsed = AgentSchema.omit({ id: true }).safeParse(body);
    if (!parsed.success) {
      return NextResponse.json({ error: parsed.error.format() }, { status: 400 });
    }

    const data = await serverFetch<Agent>(
      `/api/${currentModule}/agents`,
      "POST",
      parsed.data
    );

    return NextResponse.json(data, { status: 201 });
  } catch (err) {
    return handleApiError(err);
  }
}
