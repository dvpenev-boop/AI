using System;
using System.Windows;
using System.Diagnostics;

namespace EE.Doklad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("[App] APPLICATION STARTING");
            Console.WriteLine("========================================");
            Debug.WriteLine("========================================");
            Debug.WriteLine("[App] APPLICATION STARTING");
            Debug.WriteLine("========================================");
            
            // Add global exception handler
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            base.OnStartup(e);
            Console.WriteLine("[App] OnStartup completed");
            Debug.WriteLine("[App] OnStartup completed");
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine("========================================");
            Console.WriteLine($"[App] DISPATCHER UNHANDLED EXCEPTION");
            Console.WriteLine($"[App] Type: {e.Exception.GetType().Name}");
            Console.WriteLine($"[App] Message: {e.Exception.Message}");
            Console.WriteLine($"[App] Stack trace:");
            Console.WriteLine(e.Exception.StackTrace);
            
            if (e.Exception.InnerException != null)
            {
                Console.WriteLine($"[App] Inner exception: {e.Exception.InnerException.GetType().Name}");
                Console.WriteLine($"[App] Inner message: {e.Exception.InnerException.Message}");
                Console.WriteLine($"[App] Inner stack trace:");
                Console.WriteLine(e.Exception.InnerException.StackTrace);
            }
            Console.WriteLine("========================================");
            
            Debug.WriteLine("========================================");
            Debug.WriteLine($"[App] DISPATCHER UNHANDLED EXCEPTION: {e.Exception.Message}");
            Debug.WriteLine(e.Exception.StackTrace);
            Debug.WriteLine("========================================");
            
            MessageBox.Show(
                $"Критична грешка:\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}",
                "Грешка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Console.WriteLine("========================================");
            Console.WriteLine($"[App] APPDOMAIN UNHANDLED EXCEPTION");
            Console.WriteLine($"[App] Exception: {ex?.Message}");
            Console.WriteLine($"[App] Stack trace: {ex?.StackTrace}");
            Console.WriteLine("========================================");
        }
    }
}

