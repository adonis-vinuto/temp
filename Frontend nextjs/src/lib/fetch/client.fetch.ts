// src\lib\api\client-fetch.ts
"use client";

import { getSession } from "next-auth/react";
import { env } from "@/lib/env";

export default async function clientFetch(
  url: string,
  method?: string,
  body?: object
) {
  try {
    const res = await fetch(`${env.API_URL}${url}`, {
      method: method ?? "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${(await getSession())?.accessToken}`,
      },
      body: JSON.stringify(body),
    });
    if (!res.ok) {
      throw new Error(`HTTP error! Status: ${res.status}`);
    }
    return res.json();
  } catch {
  }
}

export async function clientFetchMultipart(
  url: string,
  method?: string,
  body?: BodyInit
) {
  try {
    const res = await fetch(`${env.API_URL}${url}`, {
      method: method ?? "GET",
      headers: {
        Authorization: `Bearer ${(await getSession())?.accessToken}`,
      },
      body: body,
    });
    if (!res.ok) {
      throw new Error(`HTTP error! Status: ${res.status}`);
    }
    return res.json();
  } catch {
  }
}
