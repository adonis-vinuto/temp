import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { SessionHistory, SessionHistorySchema } from "@/types/schemas/chat.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function GET(
  _req: NextRequest,
  context: { params: Promise<{ module: string; idAgent: string; idUser: string }> }
) {
  try {
    const { module, idAgent, idUser } = await context.params;
    const data = await serverFetch<SessionHistory[]>(
      `/api/${module}/session-history/${idAgent}/${idUser}`,
      "GET"
    );

    return NextResponse.json(SessionHistorySchema.array().parse(data), { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}

