// src/components/under-construction.tsx
import { Construction, ArrowLeft } from "lucide-react";
import Link from "next/link";

interface IUnderConstructionProps {
  title: string;
  description?: string;
  showBackButton?: boolean;
  backButtonHref?: string;
  backButtonText?: string;
}

export default function UnderConstruction({
  title,
  description = "Esta página está em desenvolvimento. Em breve novidades!",
  showBackButton = true,
  backButtonHref = "/",
  backButtonText = "Voltar ao início",
}: IUnderConstructionProps) {
  return (
    <section className="min-h-screen flex items-center justify-center p-6">
      <div className="w-full max-w-lg">
        <div className="rounded-2xl border border-blue-600/20 bg-card text-card-foreground shadow-sm p-8 md:p-10 text-center space-y-6 animate-in fade-in-50 duration-500">
          <div className="flex justify-center">
            <Construction className="h-50 w-50 text-foreground" strokeWidth={0.8} />
          </div>
          <div className="space-y-3">
            <h1 className="text-2xl md:text-3xl font-bold tracking-tight text-foreground">
              {title}
            </h1>
            <p className="text-foreground/80 text-sm md:text-base leading-relaxed">
              {description}
            </p>
          </div>
          {showBackButton && (
            <Link 
              href={backButtonHref}
              className="inline-flex items-center gap-2 text-sm font-medium text-muted-foreground hover:text-foreground transition-colors group"
            >
              <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
              {backButtonText}
            </Link>
          )}
        </div>
        <div className="flex justify-center gap-1 mt-8">
          <div className="h-1.5 w-1.5 rounded-full bg-foreground/80  animate-pulse" />
          <div className="h-1.5 w-1.5 rounded-full bg-foreground/80 animate-pulse delay-150" />
          <div className="h-1.5 w-1.5 rounded-full bg-foreground/80 animate-pulse delay-300" />
        </div>
      </div>
    </section>
  );
}
