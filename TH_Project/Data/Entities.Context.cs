﻿

//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace TH_Project.Data
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class QLBANSACHEntities : DbContext
{
    public QLBANSACHEntities()
        : base("name=QLBANSACHEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public DbSet<CHITIETDONTHANG> CHITIETDONTHANGs { get; set; }

    public DbSet<DONDATHANG> DONDATHANGs { get; set; }

    public DbSet<KHACHHANG> KHACHHANGs { get; set; }

    public DbSet<HANGSANXUAT> HANGSANXUATs { get; set; }

    public DbSet<LOAIXE> LOAIXEs { get; set; }

    public DbSet<NHAPHANPHOI> NHAPHANPHOIs { get; set; }

    public DbSet<SANXUATXE> SANXUATXEs { get; set; }

    public DbSet<XEGANMAY> XEGANMAYs { get; set; }

    public DbSet<Admin> Admins { get; set; }

}

}

