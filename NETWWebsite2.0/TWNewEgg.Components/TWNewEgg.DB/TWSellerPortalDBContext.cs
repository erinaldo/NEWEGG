using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace TWNewEgg.DB
{
    public class TWSellerPortalDBContext : DbContext
    {
        public TWSellerPortalDBContext()
            : base("name=TWSellerPortalDBConnection")
        {
        }

        public DbSet<Seller_Action> Seller_Action { get; set; }
        public DbSet<Seller_BasicInfo> Seller_BasicInfo { get; set; }
        //public DbSet<Seller_BasicInfo_log> Seller_BasicInfo_log { get; set; }  //excluded by Ron 
        public DbSet<Seller_Charge> Seller_Charge { get; set; }
        public DbSet<Seller_ContactInfo> Seller_ContactInfo { get; set; }
        public DbSet<Seller_ContactType> Seller_ContactType { get; set; }
        public DbSet<Seller_Country> Seller_Country { get; set; }
        public DbSet<Seller_Currency> Seller_Currency { get; set; }
        public DbSet<Seller_DelvTrack> Seller_DelvTrack { get; set; }
        public DbSet<Seller_Financial> Seller_Financial { get; set; }
        public DbSet<Seller_ManufactureInfo> Seller_ManufactureInfo { get; set; }
        public DbSet<Seller_ManufactureInfo_Edit> Seller_ManufactureInfo_Edit { get; set; }
        public DbSet<Seller_Notification> Seller_Notification { get; set; }
        public DbSet<Seller_ProductDetail> Seller_ProductDetail { get; set; }
        public DbSet<Seller_ProductSpec> Seller_ProductSpec { get; set; }
        public DbSet<Seller_Purview> Seller_Purview { get; set; }
        public DbSet<Seller_ReturnInfo> Seller_ReturnInfo { get; set; }
        public DbSet<Seller_Settlements> Seller_Settlements { get; set; }
        public DbSet<Seller_User> Seller_User { get; set; }
        public DbSet<User_Purview> User_Purview { get; set; }
        public DbSet<User_Group> User_Group { get; set; }
        public DbSet<Group_Purview> Group_Purview { get; set; }
        public DbSet<EDI_Seller_Function> EDI_Seller_Function { get; set; }
        public DbSet<EDI_Seller_Function_LocalizedRes> EDI_Seller_Function_LocalizedRes { get; set; }
        public DbSet<EDI_Seller_FunctionCategory> EDI_Seller_FunctionCategory { get; set; }
        public DbSet<EDI_Seller_FunctionCategory_LocalizedRes> EDI_Seller_FunctionCategory_LocalizedRes { get; set; }
        public DbSet<EDI_Seller_FunctionPoint> EDI_Seller_FunctionPoint { get; set; }
        public DbSet<EDI_Seller_FunctionPointGroup> EDI_Seller_FunctionPointGroup { get; set; }
        public DbSet<EDI_Seller_FunctionPointGroupRelation> EDI_Seller_FunctionPointGroupRelation { get; set; }
        public DbSet<Seller_MailLog> Seller_MailLog { get; set; }
        public DbSet<Seller_ChangeToVendor> Seller_ChangeToVendor { get; set; }
        public DbSet<Seller_BulletinsMessage> Seller_BulletinsMessage { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Seller_Charge>()
                 .Property(m => m.Commission)
                 .HasPrecision(6, 4);               //2014.2.13 儲存費率至小數第四位 add by ice


            //modelBuilder.Entity<Seller_User>().HasKey(user => new { user.UserEmail, user.GroupID } );
            modelBuilder.Entity<Seller_BasicInfo>().HasKey(baseinfo => new { baseinfo.SellerEmail, baseinfo.AccountTypeCode });
            modelBuilder.Entity<Seller_Charge>().HasKey(charge => new { charge.SellerID, charge.CountryCode, charge.ChargeType, charge.CategoryID });
            modelBuilder.Entity<Seller_ContactInfo>().HasKey(contactInfo => new { contactInfo.SellerID, contactInfo.ContactTypeID, contactInfo.EmailAddress });
            modelBuilder.Entity<Seller_Notification>().HasKey(notification => new { notification.SellerID, notification.NotificationTypeCode });
            modelBuilder.Entity<Seller_Purview>().HasKey(sellerpurview => new { sellerpurview.FunctionID, sellerpurview.SellerID });
            modelBuilder.Entity<User_Purview>().HasKey(userpurview => new { userpurview.FunctionID, userpurview.UserID });
            modelBuilder.Entity<Seller_ProductDetail>().HasKey(ProductDetail => new { ProductDetail.SellerID, ProductDetail.ProductID, ProductDetail.SellerProductID });


            //modelBuilder.Entity<Post>()
            //  .HasRequired(p => p.BlogUser)
            // .WithMany(user => user.Posts)
            // .HasForeignKey(p => p.UserId);
            /*modelBuilder.Entity<Seller_Action>().Property(x => x.ActionID).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Seller_Action>().ToTable("Seller_Action");
            modelBuilder.Entity<Seller_BasicInfo>().ToTable("Seller_BasicInfo");
            modelBuilder.Entity<Seller_BasicInfo_log>().ToTable("Seller_BasicInfo_log");
            modelBuilder.Entity<Seller_Charge>().ToTable("Seller_Charge");
            modelBuilder.Entity<Seller_ContactAddress>().ToTable("Seller_ContactAddress");
            modelBuilder.Entity<Seller_Financial>().ToTable("Seller_Financial");
            modelBuilder.Entity<Seller_ManufactureInfo>().ToTable("Seller_ManufactureInfo");
            modelBuilder.Entity<Seller_Notification>().ToTable("Seller_Notification");
            modelBuilder.Entity<Seller_ProductSpec>().ToTable("Seller_ProductSpec");
            modelBuilder.Entity<Seller_Purview>().ToTable("Seller_Purview");
            modelBuilder.Entity<Seller_ReturnAddress>().ToTable("Seller_ReturnAddress");
            modelBuilder.Entity<Seller_User>().ToTable("Seller_User");
            modelBuilder.Entity<User_Purview>().ToTable("User_Purview");
            modelBuilder.Entity<User_Group>().ToTable("User_Group");
            modelBuilder.Entity<Group_Purview>().ToTable("Group_Purview");*/
        }
    }
}
