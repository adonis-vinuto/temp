// src\app\api\keycloak\profile\route.ts
import { NextResponse } from "next/server";

export async function GET() {
  const base = process.env.KEYCLOAK_BASE_URL;
  const realm = process.env.KEYCLOAK_REALM;

  if (!base || !realm) {
    return NextResponse.redirect(new URL("/", process.env.NEXT_PUBLIC_APP_URL || "http://localhost:3000"));
  }

  const url = `${base.replace(/\/$/, "")}/realms/${realm}/account/`;
  return NextResponse.redirect(url, { status: 302 });
}
