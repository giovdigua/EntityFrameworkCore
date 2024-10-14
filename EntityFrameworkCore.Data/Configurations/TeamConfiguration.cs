﻿using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Data.Configurations
{
    internal class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasData(
                    new Team
                    {
                        Id = 1,
                        Name = "Tivoli Gardens FC",
                        CreatedDate = new DateTime(2024,10,10),
                    },
                    new Team
                    {
                        Id = 2,
                        Name = "Waterhouse F.C.",
                        CreatedDate = new DateTime(2024, 10, 10),
                    }, new Team
                    {
                        Id = 3,
                        Name = "Humble Lions F.C.",
                        CreatedDate = new DateTime(2024, 10, 10),
                    }
                );
        }
    }
}
