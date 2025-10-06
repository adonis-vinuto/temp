// src\app\(app)\layout.tsx
import type { Metadata } from "next";
import { SidebarProvider } from "@/components/ui/sidebar";
import { AppSidebar } from "@/app/(app)/_components/app-sidebar";
import { cn } from "@/lib/utils";
import { Inter } from "next/font/google";

import "@/styles/globals.css";
import "@/styles/sidebar-outline.css";

const inter = Inter({ 
  subsets: ["latin"],
  variable: "--font-sans",
})

export const metadata: Metadata = {
  title: "Komvos",
  description: "App",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="pt-BR" suppressHydrationWarning>
      <body
        className={cn(
          "min-h-screen bg-background font-sans antialiased text-foreground",
          inter.className,
          inter.variable
        )}
      >
        <div className="h-screen flex overflow-hidden">
          <SidebarProvider>
            <AppSidebar />
            <main className="flex-1 overflow-y-auto">
              {children}
            </main>
          </SidebarProvider>
        </div>
      </body>
    </html>
  );
}
