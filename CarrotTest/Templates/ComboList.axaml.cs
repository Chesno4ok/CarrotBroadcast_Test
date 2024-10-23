using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using CarrotTest.Models;
using DynamicData.Kernel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CarrotTest.Templates;

public class ComboList : TemplatedControl
{
    public User User
    {
        get => GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }

    public static readonly StyledProperty<User> UserProperty =
        AvaloniaProperty.Register<ComboList, User>(nameof(User));

    public AvaloniaList<Role> Roles
    {
        get => GetValue(RolesProperyt);
        set => SetValue(RolesProperyt, value);
    }

    public static readonly StyledProperty<AvaloniaList<Role>> RolesProperyt =
        AvaloniaProperty.Register<ComboList, AvaloniaList<Role>>(nameof(User));

    public override void BeginInit()
    {
        using var dbContext = new DatabaseContext();

        Roles = new(dbContext.Roles);

        base.BeginInit();
    }

    public override void EndInit()
    {
        base.EndInit();
    }

}