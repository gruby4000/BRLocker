using System;
using Microsoft.Xrm.Sdk;

public class PreventStatusChangePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        var service = serviceFactory.CreateOrganizationService(context.UserId);
        var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        try
        {
            if (context.MessageName != "Update" || !context.InputParameters.Contains("Target"))
                return;

            var target = (Entity)context.InputParameters["Target"];
            if (target.LogicalName != "pan_trainingedition")
                return;

            if (!target.Attributes.Contains("pan_status"))
                return;

            var traceLog = "";

            if (!context.PreEntityImages.Contains("PreImage"))
            {
                traceLog += "Brak PreImage.\n";
                return;
            }

            var preImage = (Entity)context.PreEntityImages["PreImage"];
            var oldStatus = preImage.GetAttributeValue<OptionSetValue>("pan_status")?.Value;
            var newStatus = target.GetAttributeValue<OptionSetValue>("pan_status")?.Value;

            traceLog += $"OldStatus: {oldStatus}, NewStatus: {newStatus}\n";

            string GetStatusName(int? status)
            {
                if (!status.HasValue)
                    return "Brak";

                switch (status.Value)
                {
                    case 889730000: return "Planowane";
                    case 889730001: return "W trakcie rekrutacji";
                    case 889730002: return "Potwierdzone";
                    case 889730003: return "W trakcie";
                    case 889730004: return "Zakończone";
                    case 889730005: return "Anulowane";
                    default: return "Nieznany";
                }
            }

            traceLog += $"OldStatusName: {GetStatusName(oldStatus)}\n";
            traceLog += $"NewStatusName: {GetStatusName(newStatus)}\n";

            if (oldStatus == 889730004 && newStatus == 889730000)
            {
                traceLog += "Blokowana zmiana z Zakończone na Planowane.\n";
                target["pan_traceinfo"] = traceLog;
                throw new InvalidPluginExecutionException("Chwila, nie można tak po prostu przestawić statusu z 'Zakończone' na 'Planowane'.");
            }

            target["pan_traceinfo"] = traceLog;
        }
        catch (Exception ex)
        {
            tracingService.Trace("Błąd pluginu: " + ex.ToString());
            throw new InvalidPluginExecutionException("Wystąpił błąd w pluginie: " + ex.Message);
        }
    }
}
