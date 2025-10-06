import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import {
  ContinueMessageRequestSchema,
  ContinueMessageResponse,
  ContinueMessageResponseSchema,
} from "@/types/schemas/chat.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function POST(
  req: NextRequest,
  context: { params: Promise<{ module: string; idAgent: string; idSession: string }> }
) {
  try {
    const { module, idAgent, idSession } = await context.params;

    const body = await req.json();
    const parsed = ContinueMessageRequestSchema.safeParse(body);
    if (!parsed.success) {
      return NextResponse.json({ error: parsed.error.format() }, { status: 400 });
    }

    const data = await serverFetch<ContinueMessageResponse>(
      `/api/${module}/chat/${idAgent}/${idSession}`,
      "POST",
      parsed.data
    );

    return NextResponse.json(ContinueMessageResponseSchema.parse(data), { status: 201 });
  } catch (err) {
    return handleApiError(err);
  }
}
