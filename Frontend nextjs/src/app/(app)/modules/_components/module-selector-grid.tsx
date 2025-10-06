'use client'

import { useModuleStore } from "@/lib/context/use-module-store.context";
import { Module } from "@/types/enums/module.enum";
import { BarChart3, Bot, Shield, Sparkles, TrendingUp, Users, Workflow } from "lucide-react"
import { useRouter } from "next/navigation";
import { toast } from "sonner"

interface IModuleProps {
    modulesList: string[]
}

export function ModuleSelectorGrid({ modulesList }: IModuleProps) {

    const normalizedSet = new Set(modulesList.map(m => m.toLowerCase()));
    const { module, setModule } = useModuleStore();
    const router = useRouter()


    return (
        <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">

            <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all"
                onClick={() => {
                    setModule(Module.People)
                    toast.success("Módulo alterado para Mind People")
                    router.push("/agents")

                }}
                disabled={!normalizedSet.has(Module.Finance.toLowerCase())}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <Users className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind People
                </h4>
                <p className="text-muted text-sm text-center">
                    Gestão estratégica de RH
                </p>
            </button>


            <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all     
                           disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:shadow-none disabled:hover:border-border"
                onClick={() => {
                    setModule(Module.Sales)
                    toast.success("Módulo alterado para Mind Sales")
                    router.push("/agents")
                }}
                disabled={!normalizedSet.has(Module.Sales.toLowerCase())}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <TrendingUp className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind Sales
                </h4>
                <p className="text-muted text-sm text-center">
                    Prospecção e qualificação de leads
                </p>
            </button>


            <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all     
                           disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:shadow-none disabled:hover:border-border"
                onClick={() => {
                    setModule(Module.Finance)
                    toast.success("Módulo alterado para Mind Finance")
                    router.push("/agents")
                }}
                disabled={!normalizedSet.has(Module.Finance.toLowerCase())}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <BarChart3 className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind Finance
                </h4>
                <p className="text-muted text-sm text-center">
                    Gestão de indicadores financeiros
                </p>
            </button>

            <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all     
                           disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:shadow-none disabled:hover:border-border"
                onClick={() => {
                    setModule(Module.Tax)
                    toast.success("Módulo alterado para Mind Tax")
                    router.push("/agents")
                }}
                disabled={!normalizedSet.has(Module.Tax.toLowerCase())}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <Shield className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind Tax
                </h4>
                <p className="text-muted text-sm text-center">
                    Inteligência fiscal e tributária
                </p>
            </button>

            <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all     
                           disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:shadow-none disabled:hover:border-border"
               onClick={() => {
                    setModule(Module.Support)
                    toast.success("Módulo alterado para Mind Finance")
                    router.push("/agents")
                }}
                disabled={!normalizedSet.has(Module.Support.toLowerCase())}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <Bot className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind Support
                </h4>
                <p className="text-muted text-sm text-center">
                    Atendimento e suporte automatizado
                </p>
            </button>

            {/* <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all"
                onClick={() => console.log("module:: marketing ; index:: 4")}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <Sparkles className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind Marketing
                </h4>
                <p className="text-muted text-sm text-center">
                    Campanhas e análise de mercado
                </p>
            </button>

            <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all"
                onClick={() => console.log("module:: ops ; index:: 5")}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <Workflow className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind Ops
                </h4>
                <p className="text-muted text-sm text-center">
                    Automação de processos internos
                </p>
            </button>

            <button
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all"
                onClick={() => console.log("module:: cx ; index:: 6")}
            >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                    <Users className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                    Mind CX
                </h4>
                <p className="text-muted text-sm text-center">
                    Experiência do cliente otimizada
                </p>
            </button> */}

        </div>
    )
}