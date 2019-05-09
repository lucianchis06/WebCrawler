using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler.Database
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            IConfigurationRoot configuration = new ConfigurationBuilder().Build();
            //builder.UseSqlServer("Data Source=95.110.201.144;Initial Catalog=DigitalDeliveryNew;User id=mauro1;Password=nac%mau$0046@Aa;Connect Timeout=30;Encrypt=False;");
            builder.UseSqlServer("Data Source=ISS0;Database=WebCrawler;User ID=sa;PWD=Afpftcb1td;Trusted_Connection=False;");
            //builder.UseSqlServer(configuration["App::ConnectionStrings:DigitalDeliveryContext"]);
            return new DatabaseContext(builder.Options);
        }
    }
}
