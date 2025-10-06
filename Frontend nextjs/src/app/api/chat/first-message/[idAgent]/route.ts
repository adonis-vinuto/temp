import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import {
  FirstMessageRequestSchema,
  FirstMessageResponse,
  FirstMessageResponseSchema,
} from "@/types/schemas/chat.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

type Params = { idAgent: string };

export async function POST(
  req: NextRequest,
  context: { params: Promise<Params> }
): Promise<Response> {
  try {
    const { idAgent } = await context.params;

    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");

    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    const body = await req.json();
    const parsed = FirstMessageRequestSchema.safeParse(body);

    if (!parsed.success) {
      return NextResponse.json({ error: parsed.error.format() }, { status: 400 });
    }

    const data = await serverFetch<FirstMessageResponse>(
      `/api/${currentModule}/chat/${idAgent}/first-message`,
      "POST",
      parsed.data
    );

    return NextResponse.json(FirstMessageResponseSchema.parse(data), { status: 201 });
  } catch (err) {
    return handleApiError(err);
  }
}
