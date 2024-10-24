﻿using Avalonia.Controls;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using System.Collections.Generic;
using CarrotTest.ViewModels;
using System.Threading.Tasks;
using System;
using Avalonia.Threading;
using MsBox.Avalonia.Base;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

namespace CarrotTest.Views;

public partial class MainWindow : Window
{
    private bool ForceExit = false;
    public MainWindow()
    {
        InitializeComponent();

        Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (ForceExit)
        {
            e.Cancel = false;
            return;
        }
        else
        {
            e.Cancel = true;
        }


        var box = MessageBoxManager.GetMessageBoxCustom(new MsBox.Avalonia.Dto.MessageBoxCustomParams()
        {
            ButtonDefinitions = new List<ButtonDefinition>
                    {
                        new ButtonDefinition { Name = "Да", },
                        new ButtonDefinition { Name = "Нет", },
                        new ButtonDefinition { Name = "Отмена", }
                    },
            ContentTitle = "Выход",
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            ShowInCenter = true,
            ContentMessage = "Сохранить изменения?"
        });

        var res = box.ShowWindowDialogAsync(this);

        Task.Run(() => ClosingAwait(e, res));


    }

    private async Task ClosingAwait(WindowClosingEventArgs e, Task<string> task)
    {
        var answ = task.Result;
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

        switch (answ)
        {
            case "Да":
                ForceExit = true;
                Dispatcher.UIThread.Post(() =>
                {
                    var vm = View.DataContext as MainViewModel;
                    vm.SaveChanges();
                    mainWindow.Close();
                });
                break;

            case "Нет":
                ForceExit = true;
                Dispatcher.UIThread.Post(() => mainWindow.Close());
                break;

            default:
                break;
        }
    }

    //protected override void OnClosing(WindowClosingEventArgs e)
    //{
//        if(ForceExit)
//        {
//            base.OnClosing(e);
//            return;
//        }
//        else
//        {
//            e.Cancel = true;
//        }


//        var box = MessageBoxManager.GetMessageBoxCustom(new MsBox.Avalonia.Dto.MessageBoxCustomParams()
//        {
//            ButtonDefinitions = new List<ButtonDefinition>
//                    {
//                        new ButtonDefinition { Name = "Да", },
//                        new ButtonDefinition { Name = "Нет", },
//                        new ButtonDefinition { Name = "Отмена", }
//                    },
//            ContentTitle = "Выход",
//            WindowStartupLocation = WindowStartupLocation.CenterOwner,
//            CanResize = false,
//            ShowInCenter = true,
//            ContentMessage = "Сохранить изменения?"
//        });

//var res = box.ShowAsPopupAsync(this);

//Task.Run(() => ClosingAwait(e, res));

//base.OnClosing(e);

    //}
}
