// src\app\page.tsx
import { KeycloakToken } from "@/types/interfaces/keycloak-token.interface"
import { ModuleSelectorGrid } from "./_components/module-selector-grid"
import { PageHeader } from "@/components/page-header"
import { decodeAccessToken } from "@/lib/utils/decode-access-token.utils"

export default async function ModulesPage() {
  const token: KeycloakToken = await decodeAccessToken()
  const moduleList:string[] = token.realm_access.roles
  return (
    <div className="min-h-screen ">
      <div className="p-6 space-y-6">
        <PageHeader
          title="Modulos Inteligentes"
          description="Selecione e visualize seus modulos"
        />

        <section className="py-20 px-4">
          <div className="container max-w-7xl">
            <ModuleSelectorGrid modulesList={moduleList}/>
          </div>
        </section>
      </div>
    </div>
  )
}
