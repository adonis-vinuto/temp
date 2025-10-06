// src\lib\utils\logout.ts
"use server";

import { getServerSession } from "next-auth";
import { authOptions } from "../auth";
import { cookies } from "next/headers";
import { revalidatePath } from "next/cache";

export default async function logout(): Promise<boolean> {
  try {
    const session = await getServerSession(authOptions);
    
    if (session?.refresh_token) {
      try {
        await fetch(
          `${process.env.KEYCLOAK_BASE_URL}/realms/${process.env.KEYCLOAK_REALM}/protocol/openid-connect/logout`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/x-www-form-urlencoded",
            },
            body: new URLSearchParams({
              client_id: process.env.KEYCLOAK_CLIENT_ID!,
              client_secret: process.env.KEYCLOAK_CLIENT_SECRET || "",
              refresh_token: session.refresh_token,
            }),
          }
        );
        
      } catch  {
      }
    }
    
    const cookieStore = await cookies();
    
    const authCookies = [
      "next-auth.session-token",
      "__Secure-next-auth.session-token",
      "next-auth.csrf-token", 
      "__Secure-next-auth.csrf-token",
      "next-auth.callback-url",
      "__Secure-next-auth.callback-url",
    ];
    
    authCookies.forEach(cookieName => {
      cookieStore.delete({
        name: cookieName,
        path: "/",
      });
    });
    
    revalidatePath("/", "layout");
    
    return true;
  } catch {
    return false;
  }
}
