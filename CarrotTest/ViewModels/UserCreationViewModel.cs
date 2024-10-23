using Avalonia.Collections;
using Avalonia.Metadata;
using CarrotTest.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotTest.ViewModels
{
    class UserCreationViewModel : ViewModelBase
    {
        private User _user;
        public User User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }


        public string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        AvaloniaList<Role> Roles
        {
            get
            {
                return new(new DatabaseContext().Roles.AsEnumerable());
            }
        }

        public ReactiveCommand<Unit, Unit> SaveUserCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; set; }

        public bool IsUserFilled
        {
            get
            {
                if (User == null)
                    return false;

                return string.IsNullOrWhiteSpace(User.Name)
                || string.IsNullOrWhiteSpace(User.LastName)
                || string.IsNullOrWhiteSpace(User.Login)
                || string.IsNullOrWhiteSpace(User.Password)
                || string.IsNullOrWhiteSpace(User.Email);
            }
        }
        

        public UserCreationViewModel()
        {
            _user = new User()
            {
                Role = 0
            };
        }
    }
}
