﻿using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Modules.Customers.Errors;

namespace MusicStore.Modules.Customers.Models;

internal sealed class Customer : Aggregate<Guid>
{
    private readonly List<Address> _addresses = [];
    
    public string Email { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public string? FullName { get; private set; }
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    public static Customer Create(Guid id, string email, string? phoneNumber = null, string? fullName = null)
    {
        var customer = new Customer
        {
            Id = id,
            Email = email,
            PhoneNumber = phoneNumber,
            FullName = fullName
        };

        return customer;
    }

    public void AddAddress(string city, string street, string buildingNumber, string apartment)
    {
        _addresses.Add(new Address(city, street, buildingNumber, apartment, Id));
    }

    public void UpdateAddress(Guid addressId, string? city, string? street, string? buildingNumber, string? apartment)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        
        if (address is null)
        {
            throw new ProblemDetailsException(AddressErrors.NotFound(addressId));
        }

        address.Update(city, street, buildingNumber, apartment);
    }
}

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers")
            .HasKey(x => x.Id);
        
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Email)
            .HasMaxLength(50);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);
        
        builder.Property(x => x.FullName)
            .HasMaxLength(50);
    }
}
