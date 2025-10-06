// src\types\interfaces\auth.intf.ts
import { JWT } from "next-auth/jwt";

export interface IAuthUser {
  id?: string;
  email?: string | null;
  name?: string | null;
  image?: string | null;
  role?: string;
  permissions?: string[];
}

export interface ICustomJWT extends JWT {
  accessToken?: string;
  refreshToken?: string;
  expires?: number;
  error?: string;
  user?: IAuthUser;
}

export interface ICustomSession {
  user: IAuthUser;
  accessToken?: string;
  refresh_token?: string;
  error?: string;
  expires?: string;
}

export interface IKeycloakTokenResponse {
  access_token: string;
  refresh_token?: string;
  expires_in: number;
  token_type: string;
  scope?: string;
}

export interface IAuthError {
  error: string;
  error_description?: string;
}
