import { DefaultSession } from "next-auth";

export declare module "next-auth" {
  interface Session {
    refresh_token?: string;
    accessToken?: string;
    error?: string;
    user?: {
      id: string;
    } & DefaultSession["user"];
  }

  interface User {
    id: string;
  }
}

export declare module "next-auth/jwt" {
  interface JWT {
    accessToken?: string;
    refreshToken?: string;
    expires?: number;
    error?: string;
    user?: DefaultSession["user"];
    id?: string;
  }
}
