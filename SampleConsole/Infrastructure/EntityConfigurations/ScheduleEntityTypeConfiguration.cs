using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Infrastructure.EntityConfigurations
{
    class ScheduleEntityTypeConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("Schedules", SampleDbContext.DEFAULT_SCHEMA);

            builder.HasKey(s => s.Id);

            builder.Ignore(s => s.DomainEvents);

            builder.Property(s => s.Id)
                .UseHiLo("schedule_hilo", SampleDbContext.DEFAULT_SCHEMA);

            //builder.Property(s => s.ScheduleId)
            //    .IsRequired(true);

            //builder.Property(s => s.ScheduleDateTime)
            //    .IsRequired(true);

            //builder.Property(s => s.ScheduleContent)
            //    .HasMaxLength(200)
            //    .IsRequired(false);

            builder
                .Property<int>("_scheduleId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ScheduleId")
                .IsRequired(true);

            builder
                .Property<DateTime>("_scheduleDateTime")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ScheduleDateTime")
                .IsRequired(true);

            builder
                .Property<string?>("_scheduleContent")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ScheduleContent")
                .HasMaxLength(200)
                .IsRequired(false);

        }
    }
}
