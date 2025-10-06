// src\app\api\file\proxy\route.ts
import { NextRequest, NextResponse } from "next/server";

export async function GET(req: NextRequest) {
  const { searchParams } = req.nextUrl;

  const url = searchParams.get("url");
  const filename = searchParams.get("filename") ?? "arquivo";
  const disposition = (searchParams.get("disposition") ?? "inline").toLowerCase();

  if (!url) {
    return NextResponse.json({ error: "Parâmetro 'url' é obrigatório." }, { status: 400 });
  }

  try {
    const upstream = await fetch(url);

    if (!upstream.ok || !upstream.body) {
      return NextResponse.json(
        { error: `Falha ao obter arquivo (HTTP ${upstream.status})` },
        { status: upstream.status || 500 }
      );
    }

    const contentType =
      upstream.headers.get("content-type") ?? "application/octet-stream";
    const headers = new Headers();
    headers.set("Content-Type", contentType);
    headers.set(
      "Content-Disposition",
      `${disposition}; filename="${encodeURIComponent(filename)}"`
    );
    headers.set("Cache-Control", "private, max-age=60");
    return new Response(upstream.body, {
      status: 200,
      headers,
    });
  } catch {
    return NextResponse.json({ error: "Erro ao proxyficiar arquivo." }, { status: 500 });
  }
}
