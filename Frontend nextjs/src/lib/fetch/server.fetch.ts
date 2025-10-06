// src\lib\api\server-fetch.ts
import { getServerSession } from "next-auth/next";
import { authOptions } from "../auth";
import { env } from "@/lib/env";

export default async function serverFetch<T = unknown>(
  url: string,
  method?: string,
  body?: object,
  multipart: boolean = false,
  params?: { [key: string]: string }
): Promise<T> {
  const apiUrl = new URL(`${env.API_URL}${url}`);

  if (params) {
    Object.keys(params).forEach((key) => {
      if (Array.isArray(params[key])) {
        (params[key] as unknown as string[]).forEach((value) => {
          apiUrl.searchParams.append(key, value);
        });
      } else {
        apiUrl.searchParams.append(key, params[key] as string);
      }
    });
  }
  const res = await fetch(apiUrl.toString(), {
    method,
    headers: {
      Authorization: `Bearer ${(await getServerSession(authOptions))?.accessToken}`,
      ...(multipart ? {} : { "Content-Type": "application/json" }),
    },
    body: multipart ? (body as BodyInit) : body ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    throw new Error(`HTTP ${res.status}: ${await res.text()}`);
  }
  return (await res.json()) as T;
}
