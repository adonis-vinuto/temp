// src\lib\utils\api-error-handler.ts
import { NextResponse } from "next/server";
import { parseErrorMessage } from "./error-parser.utils";

export function handleApiError(error: unknown, defaultStatus = 500) {
  let status = defaultStatus;

  if (error instanceof Error) {
    const httpMatch = error.message.match(/HTTP (\d+)/);
    if (httpMatch) {
      status = parseInt(httpMatch[1], 10);
    }
  }

  const message = parseErrorMessage(error);

  return NextResponse.json(
    {
      error: message,
      timestamp: new Date().toISOString(),
    },
    { status }
  );
}
