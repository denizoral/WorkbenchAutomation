using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

class Program
{
    static void Main()
    {
        string result = CommandRunner.Run("cmd.exe", "/c echo oof");
        Console.WriteLine(result);
        Console.WriteLine("Launching MySQL Workbench...");

        var proc = Process.Start(@"C:\\Program Files\\MySQL\\MySQL Workbench 8.0 CE\\MySQLWorkbench.exe");
        proc.WaitForInputIdle();
        Thread.Sleep(3000); // wait for UI to settle

        var mainWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children,
            new PropertyCondition(AutomationElement.NameProperty, "MySQL Workbench"));

        if (mainWindow == null)
        {
            Console.WriteLine("Could not find main Workbench window.");
            return;
        }

        Console.WriteLine("Workbench found.");

        // Click "Add Connection"
        InvokeByName(mainWindow, "Add Connection");
        Thread.Sleep(1500);

        Console.WriteLine("Filling connection fields...");
        SetText(mainWindow, ControlType.Edit, "DEV_DB", "Connection Name");
        SetText(mainWindow, ControlType.Edit, "amednhosskgdksdf", "Host Name");
        SetText(mainWindow, ControlType.Edit, "3131", "Port");
        SetText(mainWindow, ControlType.Edit, "yuarraq", "User Name");

        InvokeByName(mainWindow, "Store in Vault ...");
        Thread.Sleep(1000);

        var popup = mainWindow.FindFirst(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.NameProperty, "Store Password For Connection"));

        if (popup != null)
        {
            SetText(popup, ControlType.Edit, "somepasswordherebro");
            InvokeByName(popup, "OK");
            Console.WriteLine("✅ Password entered and confirmed.");
        }

        SelectTab(mainWindow, "SSL");
        SetText(mainWindow, ControlType.Edit, @"C:\\path\\to\\ssl-cert.pem", "Path to Client Certificate file for SSL.");

        SelectTab(mainWindow, "Advanced");
        ToggleCheckbox(mainWindow, "Enable Cleartext Authentication Plugin");

        InvokeByName(mainWindow, "OK");

        Console.WriteLine("Done.");
    }

    static void SetText(AutomationElement parent, ControlType type, string value, string name = null)
    {
        AutomationElement element = name != null ?
            parent.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, type),
                    new PropertyCondition(AutomationElement.NameProperty, name))) :
            parent.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, type));

        if (element != null && element.TryGetCurrentPattern(ValuePattern.Pattern, out var pattern))
        {
            ((ValuePattern)pattern).SetValue(value);
        }
    }

    static void InvokeByName(AutomationElement parent, string name)
    {
        var element = parent.FindFirst(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.NameProperty, name));

        if (element != null && element.TryGetCurrentPattern(InvokePattern.Pattern, out var pattern))
        {
            ((InvokePattern)pattern).Invoke();
        }
    }

    static void SelectTab(AutomationElement parent, string name)
    {
        var tab = parent.FindFirst(TreeScope.Descendants,
            new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem),
                new PropertyCondition(AutomationElement.NameProperty, name)));

        if (tab != null && tab.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var pattern))
        {
            ((SelectionItemPattern)pattern).Select();
            Console.WriteLine($"✅ {name} tab selected.");
        }
    }

    static void ToggleCheckbox(AutomationElement parent, string name)
    {
        var checkbox = parent.FindFirst(TreeScope.Descendants,
            new AndCondition(
                new PropertyCondition(AutomationElement.NameProperty, name),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox)));

        if (checkbox != null && checkbox.TryGetCurrentPattern(TogglePattern.Pattern, out var pattern))
        {
            ((TogglePattern)pattern).Toggle();
        }
    }
}
