// src\app\(app)\_components\sidebar\logout-button.tsx
"use client";

import { LogOut } from "lucide-react";
import { Button } from "@/components/ui/button";
import { signOut } from "next-auth/react";
import logout from "@/lib/utils/logout.utils";
import { useState } from "react";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { ILogoutButtonProps } from "@/types/interfaces/sidebar.interface";

export function LogoutButton({ 
  showText = true, 
  variant = "ghost", 
  size = "sm",
  className = ""
}: ILogoutButtonProps) {
  const [showDialog, setShowDialog] = useState(false);
  const [isLoggingOut, setIsLoggingOut] = useState(false);

  const handleLogout = async () => {
    setIsLoggingOut(true);
    try {
      if (typeof window !== 'undefined') {
        localStorage.clear();
        sessionStorage.clear();
        document.cookie.split(";").forEach((c) => {
          document.cookie = c
            .replace(/^ +/, "")
            .replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/");
        });
      }
      await logout();
      await signOut({ redirect: false });
      window.location.replace("/login");
    } catch {
      window.location.replace("/login");
    }
  };

  return (
    <>
      <Button
        variant={variant}
        size={size}
        className={`${showText ? 'justify-start' : ''} text-destructive hover:text-destructive hover:bg-destructive/10 ${className}`}
        onClick={() => setShowDialog(true)}
        disabled={isLoggingOut}
      >
        <LogOut className={showText ? "mr-2 h-4 w-4" : "h-4 w-4"} />
        {showText && (isLoggingOut ? "Saindo..." : "Sair")}
      </Button>
      <AlertDialog open={showDialog} onOpenChange={setShowDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar saída</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza de que deseja sair? Você será redirecionado para a página de login.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isLoggingOut}>
              Cancelar
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleLogout}
              disabled={isLoggingOut}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {isLoggingOut ? "Saindo..." : "Sair"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
