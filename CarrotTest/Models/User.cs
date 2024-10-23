using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CarrotTest.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? Role { get; set; }

    public string? Notes { get; set; }

    public Role RoleNav
    {
        get => new DatabaseContext().Roles.FirstOrDefault(i => i.Id == Role);
    }

    public Role? RoleNavigation;

    public void CopyPropertiesFrom(object source)
    {
        foreach (PropertyInfo property in source.GetType().GetProperties())
        {
            if (property.CanWrite)
            {
                property.SetValue(this, property.GetValue(source));
            }
        }
    }
}
