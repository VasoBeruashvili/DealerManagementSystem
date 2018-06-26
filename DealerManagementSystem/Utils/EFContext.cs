using DealerManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Utils //TODO change namespace
{
    public class EFContext : DbContext
    {
        public EFContext() : base("ComtekConnection") //TODO change connection string name in .config file
        {
            this.Configuration.LazyLoadingEnabled = false; //TODO change to TRUE and assign VIRTUAL to classes if you want automatic generation of Navigation Properties
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        //public DbSet<Contragent> Contragents { get; set; }
    }
}