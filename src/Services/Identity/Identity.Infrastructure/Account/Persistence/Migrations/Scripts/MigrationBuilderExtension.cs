using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Identity.Infrastructure.Account.Persistence.Migrations.Scripts {
    public static class MigrationBuilderExtension {
        public static OperationBuilder<SqlOperation> SqlResource(
            this MigrationBuilder builder, string filename
        ) {
            using var stream = Assembly.GetAssembly(typeof(MigrationBuilderExtension))
                .GetManifestResourceStream(filename);

            using var ms = new MemoryStream();

            stream.CopyTo(ms);
            var data = ms.ToArray();
            var text = Encoding.UTF8.GetString(data, 3, data.Length - 3);

            return builder.Sql(text);
        }
    }
}
