// src\lib\auth.ts
import { NextAuthOptions } from "next-auth";
import KeycloakProvider from "next-auth/providers/keycloak";
import type { ICustomJWT, IKeycloakTokenResponse } from "@/types/interfaces/auth.interface";

async function refreshAccessToken(token: ICustomJWT): Promise<ICustomJWT> {
  try {
    const url = `${process.env.KEYCLOAK_BASE_URL}/realms/${process.env.KEYCLOAK_REALM}/protocol/openid-connect/token`;
    
    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      body: new URLSearchParams({
        client_id: process.env.KEYCLOAK_CLIENT_ID!,
        client_secret: process.env.KEYCLOAK_CLIENT_SECRET || "",
        grant_type: "refresh_token",
        refresh_token: token.refreshToken || "",
      }),
    });

    const data: IKeycloakTokenResponse = await response.json();
    
    if (!response.ok) throw data;

    return {
      ...token,
      accessToken: data.access_token,
      refreshToken: data.refresh_token ?? token.refreshToken,
      expires: Date.now() + data.expires_in * 1000,
    };
  } catch {
    return { 
      ...token, 
      error: "RefreshAccessTokenError" 
    };
  }
}

export const authOptions: NextAuthOptions = {
  providers: [
    KeycloakProvider({
      clientId: process.env.KEYCLOAK_CLIENT_ID!,
      clientSecret: process.env.KEYCLOAK_CLIENT_SECRET || "",
      issuer: `${process.env.KEYCLOAK_BASE_URL}/realms/${process.env.KEYCLOAK_REALM}`,
      httpOptions: { timeout: 10000 },
    }),
  ],
  pages: {
    signIn: "/login",
    error: "/login",
  },
  session: {
    strategy: "jwt",
  },
  callbacks: {
    async jwt({ token, user, account }) {
      if (account && user) {
        return {
          accessToken: account.access_token,
          refreshToken: account.refresh_token,
          expires: Date.now() + (account.expires_in as number) * 1000,
          user,
        } as ICustomJWT;
      }
      
      const customToken = token as ICustomJWT;
      
      if (Date.now() < (customToken.expires || 0)) {
        return customToken;
      }
      
      return await refreshAccessToken(customToken);
    },

    async session({ session, token }) {
      const customToken = token as ICustomJWT;
      
      session.user = customToken.user as typeof session.user;
      session.refresh_token = customToken.refreshToken as string;
      session.accessToken = customToken.accessToken as string;
      session.error = customToken.error as string | undefined;
      
      return session;
    },
  },
};
