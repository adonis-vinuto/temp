// src\app\layout.tsx
import type { Metadata, Viewport } from "next"
import { Inter } from "next/font/google"
import "@/styles/globals.css"
import "@/styles/sidebar-outline.css"
import { cn } from "@/lib/utils"
import { Providers } from "./providers"

const inter = Inter({ 
  subsets: ["latin"],
  variable: "--font-sans",
})

export const metadata: Metadata = {
  metadataBase: new URL(
    process.env.NEXT_PUBLIC_APP_URL || "http://localhost:3000"
  ),
  title: {
    default: "Komvos Mind - Plataforma Inteligente de Agentes Empresariais",
    template: "%s | Statum | Komvos Mind",
  },
  description:
    "Gerencie agentes empresariais, automatize processos e transforme sua operação com IA avançada",
  keywords: [
    "komvos",
    "mind",
    "agentes",
    "automação",
    "inteligência artificial",
    "IA",
    "gestão",
  ],
  authors: [{ name: "Komvos Team" }],
  creator: "Komvos",
  icons: {
    icon: [{ url: "/favicon.ico" }],
  },
}

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  maximumScale: 1,
  userScalable: false,
  themeColor: [
    { media: "(prefers-color-scheme: light)", color: "#007AA5" },
    { media: "(prefers-color-scheme: dark)", color: "#007AA5" },
  ],
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="pt-BR" suppressHydrationWarning>
      <head>
        <link rel="preconnect" href="https://fonts.googleapis.com" />
        <link
          rel="preconnect"
          href="https://fonts.gstatic.com"
          crossOrigin="anonymous"
        />
      </head>
      <body
        className={cn(
          "min-h-screen bg-background font-sans antialiased text-foreground",
          inter.className,
          inter.variable
        )}
      >
        <div id="toast-container" />
        <Providers>{children}</Providers>
      </body>
    </html>
  )
}
