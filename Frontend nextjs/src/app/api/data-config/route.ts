// src/app/api/data-config/route.ts
import { NextRequest, NextResponse } from "next/server";
import serverFetch from "@/lib/fetch/server.fetch";
import { DataConfig } from "@/types/schemas/data-config.schema";
import { handleApiError } from "@/lib/utils/api-error-handler.utils";

export async function GET() {
  try {
    const data = await serverFetch<DataConfig[]>("/api/data-config", "GET");
    return NextResponse.json(data, { status: 200 });
  } catch (err) {
    return handleApiError(err);
  }
}

export async function POST(req: NextRequest) {
  try {
    const body = await req.json();
    const data = await serverFetch<DataConfig>("/api/data-config", "POST", body);
    return NextResponse.json(data, { status: 201 });
  } catch (err) {
    return handleApiError(err);
  }
}
