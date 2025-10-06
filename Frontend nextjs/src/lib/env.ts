// src/lib/env.ts
import { createEnv } from "@t3-oss/env-nextjs"
import { z } from "zod"

export const env = createEnv({
  server: {
    KEYCLOAK_CLIENT_ID: z.string().min(1),
    KEYCLOAK_CLIENT_SECRET: z.string().min(1),
    KEYCLOAK_BASE_URL: z.string().url(),
    KEYCLOAK_REALM: z.string().min(1),

    NEXTAUTH_URL: z.string().url(),
    NEXTAUTH_SECRET: z.string().min(32),
    
    API_URL: z.string().url(),
    
    NODE_ENV: z.enum(["development", "production", "test"]).default("development"),
  },

  runtimeEnv: {
    KEYCLOAK_CLIENT_ID: process.env.KEYCLOAK_CLIENT_ID,
    KEYCLOAK_CLIENT_SECRET: process.env.KEYCLOAK_CLIENT_SECRET,
    KEYCLOAK_BASE_URL: process.env.KEYCLOAK_BASE_URL,
    KEYCLOAK_REALM: process.env.KEYCLOAK_REALM,
    NEXTAUTH_URL: process.env.NEXTAUTH_URL,
    NEXTAUTH_SECRET: process.env.NEXTAUTH_SECRET,
    API_URL: process.env.API_URL,
    NODE_ENV: process.env.NODE_ENV,
    
  },

  skipValidation: !!process.env.SKIP_ENV_VALIDATION,

  emptyStringAsUndefined: true,
})

export function getBaseUrl() {
  if (typeof window !== "undefined") {
    return ""
  }
  
  if (env.NEXTAUTH_URL) {
    return env.NEXTAUTH_URL
  }
  return `http://localhost:${process.env.PORT ?? 3000}`
}

export function getApiUrl(path: string) {
  const baseUrl = env.API_URL
  const cleanPath = path.startsWith('/') ? path : `/${path}`
  return `${baseUrl}${cleanPath}`
}
