import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import {
  ContinueMessageRequestSchema,
  ContinueMessageResponse,
  ContinueMessageResponseSchema,
} from "@/types/schemas/chat.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

type Params = { idAgent: string; idSession: string };

export async function POST(
  req: NextRequest,
  context: { params: Promise<Params> }
): Promise<Response> {
  try {
    const { idAgent, idSession } = await context.params;

    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");

    if (!currentModule) {
      return NextResponse.json(
        { error: "Module é obrigatório" },
        { status: 400 }
      );
    }

    const body = await req.json();
    const parsed = ContinueMessageRequestSchema.safeParse(body);

    if (!parsed.success) {
      return NextResponse.json(
        { error: parsed.error.format() },
        { status: 400 }
      );
    }
    const data = await serverFetch<ContinueMessageResponse>(
      `/api/${currentModule}/chat/${idAgent}/${idSession}`,
      "POST",
      parsed.data
    );

    data.sessionId = idSession;

    return NextResponse.json(ContinueMessageResponseSchema.parse(data), {
      status: 201,
    });
  } catch (err) {
    return handleApiError(err);
  }
}
