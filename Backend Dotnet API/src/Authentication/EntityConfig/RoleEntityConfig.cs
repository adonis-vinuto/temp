using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GemelliApi.Domain.Enums;

namespace Authentication.EntityConfig;

public class RoleEntityConfig : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = RoleIds.Administrador,
                Name = UserRoles.Administrador.ToString(),
                NormalizedName = UserRoles.Administrador.ToString().ToUpper(CultureInfo.InvariantCulture)
            },
            new IdentityRole
            {
                Id = RoleIds.Gerente,
                Name = UserRoles.Gerente.ToString(),
                NormalizedName = UserRoles.Gerente.ToString().ToUpper(CultureInfo.InvariantCulture)
            },
            new IdentityRole
            {
                Id = RoleIds.Desenvolvedor,
                Name = UserRoles.Desenvolvedor.ToString(),
                NormalizedName = UserRoles.Desenvolvedor.ToString().ToUpper(CultureInfo.InvariantCulture)
            },
            new IdentityRole
            {
                Id = RoleIds.Analista,
                Name = UserRoles.Analista.ToString(),
                NormalizedName = UserRoles.Analista.ToString().ToUpper(CultureInfo.InvariantCulture)
            },
            new IdentityRole
            {
                Id = RoleIds.Estagiario,
                Name = UserRoles.Estagiario.ToString(),
                NormalizedName = UserRoles.Estagiario.ToString().ToUpper(CultureInfo.InvariantCulture)
            }
        );
    }
}
