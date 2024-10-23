using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CarrotTest.Models;
using CarrotTest.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarrotTest.ViewModels;

public class MainViewModel : ViewModelBase
{
    private AvaloniaList<User> _users = new();
    public AvaloniaList<User> Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }
    public AvaloniaList<Role> Roles { get; set; } = new();

    public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenUserCreationCommand { get; set; }
    public ReactiveCommand<User, Unit> DeleteCommand { get; set; }

    public MainViewModel()
    {
        using var dbContext = new DatabaseContext();

        Roles = new(dbContext.Roles.AsEnumerable());
        Users = new(dbContext.Users.AsEnumerable());

        SaveCommand = ReactiveCommand.CreateFromTask(SaveChanges);
        OpenUserCreationCommand = ReactiveCommand.CreateFromTask(OpenUserCreation);
        DeleteCommand = ReactiveCommand.CreateFromTask<User, Unit>(DeleteUser);
    }

    public async Task OpenUserCreation()
    {
        var dialog = new DialogWindow();

        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

        dialog.ShowDialog(mainWindow);

        var vm = new UserCreationViewModel();

        vm.ExitCommand = ReactiveCommand.CreateFromTask(() =>
        {
            dialog.Close();
            return Task.CompletedTask;
        });

        vm.SaveUserCommand = ReactiveCommand.CreateFromTask(() =>
        {
            if (vm.IsUserFilled)
            {
                vm.ErrorMessage = "Все поля должны быть заполнены!";
                return Task.CompletedTask;
            }

            using var dbContext = new DatabaseContext();

            Users.Add(vm.User);

            dialog.Close();
            return Task.CompletedTask;
        });


        dialog.UserCreation.DataContext = vm;
    }

    public async Task<Unit> DeleteUser(User user)
    {
        using var dbContext = new DatabaseContext();

        Users.Remove(user);

        return Unit.Default;
    }

    public async Task SaveChanges()
    {
        var dbContext = new DatabaseContext();

        foreach(var user in Users)
        {
            var newUser = dbContext.Users.FirstOrDefault(i => i.Id == user.Id);

            if (newUser == null)
                continue;

            newUser.CopyPropertiesFrom(user);

            dbContext.SaveChanges();
        }

        var removedUsers = dbContext.Users.Where(i => !Users.Contains(i));

        dbContext.RemoveRange(removedUsers);


        var newUsers = Users.Where(i => i.Id == 0);

        dbContext.Users.AddRange(newUsers);

        dbContext.SaveChanges();

    }
}
