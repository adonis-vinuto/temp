// src\app\(auth)\login\page.tsx
"use client";
import { useEffect } from "react";
import { signIn, signOut, useSession } from "next-auth/react";
import { useRouter } from "next/navigation";

export default function LoginPage() {
  const { data: session, status } = useSession();
  const router = useRouter();

  useEffect(() => {
    if (session && status === "authenticated") {
      signOut({ redirect: false }).then(() => {});
      return;
    }
    if (status === "unauthenticated") {
      signIn("keycloak", { callbackUrl: "/agents" });
    }
  }, [session, status, router]);

  return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      <div className="bg-white p-8 rounded-2xl shadow-xl max-w-md w-full mx-4">
        <div className="text-center mb-8">
          <div className="w-16 h-16 bg-blue-500 rounded-2xl mx-auto mb-4 flex items-center justify-center">
            <svg
              className="animate-spin h-8 w-8 text-white"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                className="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                strokeWidth="4"
              />
              <path
                className="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
              />
            </svg>
          </div>
          <h1 className="text-2xl font-bold text-blue-950 mb-2">
            Bem-vindo
          </h1>
          <p className="text-gray-600">
            {status === "loading" 
              ? "Verificando autenticação..." 
              : "Redirecionando para login seguro..."}
          </p>
        </div>
        <div className="w-full bg-gray-200 rounded-full h-2">
          <div className="bg-blue-500 h-2 rounded-full animate-pulse" style={{width: '70%'}}></div>
        </div>
        <p className="text-center text-sm text-gray-500 mt-6">
          Você será redirecionado automaticamente
        </p>
      </div>
    </div>
  );
}
