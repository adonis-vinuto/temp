import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { ChatHistoryItem, ChatHistoryItemSchema } from "@/types/schemas/chat.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function GET(
  req: NextRequest,
  context: { params: Promise<{ idSession: string }> }
) {
  try {
    const { idSession } = await context.params;
    const { searchParams } = new URL(req.url);
    const currentModule = searchParams.get("module");
    if (!currentModule) {
      return NextResponse.json({ error: "Module é obrigatório" }, { status: 400 });
    }

    const data = await serverFetch<ChatHistoryItem[]>(
      `/api/${currentModule}/chat-history/${idSession}`,
      "GET"
    );

    return NextResponse.json(ChatHistoryItemSchema.array().parse(data), { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}
