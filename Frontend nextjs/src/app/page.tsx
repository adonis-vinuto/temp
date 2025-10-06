// src\app\page.tsx
import Link from "next/link"
import { 
  Brain, 
  Shield, 
  TrendingUp,
  Users,
  Workflow,
  BarChart3,
  Bot,
  Sparkles
} from "lucide-react"

export default function HomePage() {
  return (
    <div className="min-h-screen bg-background">
      <section className="relative overflow-hidden">
        <div className="absolute top-0 right-0 w-[600px] h-[600px] bg-accent/5 rounded-full blur-3xl" />
        <div className="absolute bottom-0 left-0 w-[600px] h-[600px] bg-primary/5 rounded-full blur-3xl" />
        <div className="container mx-auto px-4 py-20 relative z-10">
          <div className="max-w-4xl mx-auto text-center">
            <div className="flex justify-center mb-8">
              <div className="flex items-center space-x-3">
                <div className="p-3 bg-gradient-primary rounded-xl">
                  <Brain className="h-10 w-10 text-foreground" />
                </div>
                <h1 className="text-3xl md:text-4xl font-bold text-foreground">
                  KOMVOS MIND
                </h1>
              </div>
            </div>
            <p className="text-lg text-muted mb-8 font-medium">
              A PLATAFORMA DE INTELIGÊNCIA CORPORATIVA
            </p>
            <h2 className="text-2xl md:text-3xl font-bold text-foreground mb-6">
              Transforme sua operação com agentes de IA
              <span className="block text-accent mt-2">personalizados e evolutivos</span>
            </h2>
            <p className="text-lg text-muted max-w-3xl mx-auto mb-12">
              Uma plataforma que conecta dados, interpreta cenários, antecipa necessidades 
              e impulsiona decisões com eficiência, segurança e visão estratégica.
            </p>
            <Link
              href="/login"
              className="inline-block px-8 py-4 bg-blue-300 rounded-lg hover:shadow-lg hover:-translate-y-0.5 transition-all font-medium text-lg"
            >
              Acessar Plataforma
            </Link>
          </div>
        </div>
      </section>
      <section className="py-20 px-4 bg-card/30">
        <div className="container mx-auto max-w-6xl">
          <div className="text-center mb-12">
            <h3 className="text-2xl font-bold text-foreground mb-4">
              Módulos Inteligentes
            </h3>
            <p className="text-muted max-w-2xl mx-auto">
              Agentes especializados para cada área estratégica da sua empresa
            </p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            {[
              { icon: Users, title: "Mind People", desc: "Gestão estratégica de RH" },
              { icon: TrendingUp, title: "Mind Sales", desc: "Prospecção e qualificação de leads" },
              { icon: BarChart3, title: "Mind Finance", desc: "Gestão de indicadores financeiros" },
              { icon: Bot, title: "Mind Support", desc: "Atendimento e suporte automatizado" },
              { icon: Sparkles, title: "Mind Marketing", desc: "Campanhas e análise de mercado" },
              { icon: Workflow, title: "Mind Ops", desc: "Automação de processos internos" },
              { icon: Users, title: "Mind CX", desc: "Experiência do cliente otimizada" },
              { icon: Shield, title: "Mind Tax", desc: "Inteligência fiscal e tributária" },
              
            ].map((module, index) => (
              <div 
                key={index}
                className="p-6 bg-card rounded-xl border border-border hover:border-accent/30 hover:shadow-md transition-all"
              >
                <div className="p-3 bg-gradient-primary rounded-lg w-fit mb-4 mx-auto">
                  <module.icon className="h-6 w-6 text-foreground" />
                </div>
                <h4 className="text-foreground font-semibold mb-2 text-center">
                  {module.title}
                </h4>
                <p className="text-muted text-sm text-center">
                  {module.desc}
                </p>
              </div>
            ))}
          </div>
        </div>
      </section>
      <section className="py-20 px-4">
        <div className="container mx-auto max-w-6xl">
          <div className="text-center mb-12">
            <h3 className="text-2xl font-bold text-foreground mb-4">
              Diferenciais Estratégicos
            </h3>
            <p className="text-muted max-w-2xl mx-auto">
              Tecnologia que se adapta ao seu negócio e evolui continuamente
            </p>
          </div>
          <div className="grid md:grid-cols-3 gap-8">
            {[
              {
                title: "Inteligência Corporativa Aplicada",
                description: "Agentes que interpretam cenários, conectam dados e apoiam decisões estratégicas"
              },
              {
                title: "Personalização Evolutiva",
                description: "Cada agente é moldado à cultura da empresa e evolui com aprendizado contínuo"
              },
              {
                title: "Conectividade Ampliada",
                description: "Integrações diretas com plataformas operacionais, comerciais e financeiras"
              },
              {
                title: "Governança e Compliance",
                description: "Conformidade com LGPD, trilhas de auditoria e total transparência"
              },
              {
                title: "Framework Modular",
                description: "Expansão progressiva com novos módulos sem reimplementações complexas"
              },
              {
                title: "Experiência Premium",
                description: "Interfaces sofisticadas e painéis executivos em tempo real"
              }
            ].map((item, index) => (
              <div key={index} className="text-center">
                <h4 className="text-lg font-semibold text-foreground mb-3">
                  {item.title}
                </h4>
                <p className="text-muted text-sm">
                  {item.description}
                </p>
              </div>
            ))}
          </div>
        </div>
      </section>
      <section className="py-20 px-4">
        <div className="container mx-auto max-w-4xl text-center text-foreground">
          <h3 className="text-2xl md:text-3xl font-bold mb-4">
            Lidere o futuro com inteligência artificial corporativa
          </h3>
          <p className="text-muted text-lg mb-8">
            Transforme sua operação e ganhe vantagem competitiva real
          </p>
        </div>
      </section>
      <footer className="py-8 px-4 border-t border-border">
        <div className="container mx-auto max-w-6xl">
          <div className="flex flex-col md:flex-row items-center justify-between text-sm text-muted">
            <div className="flex items-center space-x-2 mb-4 md:mb-0">
              <Brain className="h-5 w-5 text-primary" />
              <span className="font-semibold">KOMVOS MIND</span>
            </div>
            <div className="text-center md:text-right">
              <p>Av. Sumaré, 295 - Jardim Sumaré</p>
              <p>Ribeirão Preto / SP</p>
            </div>
            <div className="mt-4 md:mt-0">
              <p>+55 (16) 3515-1600</p>
              <p>grupokomvos.com.br</p>
            </div>
          </div>
          <div className="text-center mt-6 text-xs text-muted">
            © 2025 Grupo KOMVOS. Todos os direitos reservados.
          </div>
        </div>
      </footer>
    </div>
  )
}
