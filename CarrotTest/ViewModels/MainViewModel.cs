using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CarrotTest.Models;
using CarrotTest.Views;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
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
    public ReactiveCommand<Unit, Unit> ExitCommand { get; set; }

    public MainViewModel()
    {
        using var dbContext = new DatabaseContext();

        if (!File.Exists("Database.db"))
        {
            AddMockUsers();
        }


        // Добавляем в таблицу пользователей из БД
        Roles = new(dbContext.Roles.AsEnumerable());
        // Также добавляем существующие роли
        Users = new(dbContext.Users.AsEnumerable());

        
        SaveCommand = ReactiveCommand.CreateFromTask(SaveChanges);
        OpenUserCreationCommand = ReactiveCommand.CreateFromTask(OpenUserCreation);
        DeleteCommand = ReactiveCommand.CreateFromTask<User, Unit>(DeleteUser);
        ExitCommand = ReactiveCommand.CreateFromTask(Exit);

        
    }

    public void AddMockUsers()
    {
        using var dbContext = new DatabaseContext();
        dbContext.Database.Migrate();

        var users = new User[]
        {
            new User()
            {
                Name = "Екатерина",
                LastName = "Пучкова",
                Login = "8gsaf76d",
                Password = "Hudf7833",
                Email = "example@mail.org",
                Role = 1,
                Notes = "Хорший сотрудник"
            },
            new User()
            {
                Name = "Алексей",
                LastName = "Иванов",
                Login = "ivanov123",
                Password = "Pass123!",
                Email = "alexey@mail.org",
                Role = 2,
                Notes = "Отличный программист"
            },
            new User()
            {
                Name = "Марина",
                LastName = "Козлова",
                Login = "marina88",
                Password = "Zxcvbnm12",
                Email = "marina.kozlova@mail.ru",
                Role = 3,
                Notes = "Работает с клиентами"
            }
        };

        var roles = new Role[]
        {
            new Role()
            {
                Id = 0,
                Name = "Гость"
            },
            new Role()
            {
                Id = 1,
                Name = "Пользователь"
            },
            new Role()
            {
                Id = 2,
                Name = "Модератор"
            },
            new Role()
            {
                Id = 3,
                Name = "Администратор"
            }
        };

        

        dbContext.Users.AddRange(users);
        dbContext.Roles.AddRange(roles);

        dbContext.SaveChanges();
    }
    

    public async Task OpenUserCreation()
    {
        // Создаем диалоговое окно
        var dialog = new DialogWindow();

        // Получаем главное окно, чтобы к нему прикрепить диалог
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

        dialog.ShowDialog(mainWindow);


        // Создаем VM и распределяем комманды
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

        // Редактируем записи из БД
        foreach(var user in Users)
        {
            var editUser = dbContext.Users.FirstOrDefault(i => i.Id == user.Id);

            if (editUser == null)
                continue;

            editUser.CopyPropertiesFrom(user);
        }

        // Удаление пользователей
        var removedUsers = dbContext.Users.Where(i => !Users.Contains(i));
        dbContext.RemoveRange(removedUsers);

        // Добавлени новых
        var newUsers = Users.Where(i => i.Id == 0);
        dbContext.Users.AddRange(newUsers);

        await dbContext.SaveChangesAsync();
    }

    public async Task Exit()
    {
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

        mainWindow.Close();
    }
}
