﻿using System.Data.Entity.Migrations.Design;
using System.Data.Entity.SqlServer;

namespace Zidium.Core.Common
{
    public class NonClusteredPrimaryKeyCSharpMigrationCodeGenerator : CSharpMigrationCodeGenerator
    {
        protected override void Generate(System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation addPrimaryKeyOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {
            addPrimaryKeyOperation.IsClustered = false;
            base.Generate(addPrimaryKeyOperation, writer);
        }
        protected override void GenerateInline(System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation addPrimaryKeyOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {
            addPrimaryKeyOperation.IsClustered = false;
            base.GenerateInline(addPrimaryKeyOperation, writer);
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.CreateTableOperation createTableOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {
            createTableOperation.PrimaryKey.IsClustered = false;
            base.Generate(createTableOperation, writer);
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.MoveTableOperation moveTableOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {
            moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = false;
            base.Generate(moveTableOperation, writer);
        }
    }

    public class NonClusteredPrimaryKeySqlMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation addPrimaryKeyOperation)
        {
            addPrimaryKeyOperation.IsClustered = false;
            base.Generate(addPrimaryKeyOperation);
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.CreateTableOperation createTableOperation)
        {
            createTableOperation.PrimaryKey.IsClustered = false;
            base.Generate(createTableOperation);
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.MoveTableOperation moveTableOperation)
        {
            moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = false;
            base.Generate(moveTableOperation);
        }
    }
}
